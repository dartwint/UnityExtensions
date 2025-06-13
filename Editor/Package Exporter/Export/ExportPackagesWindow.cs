using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportPackagesWindow : EditorWindow
    {
        private static readonly string _dataEditorPrefsKey;
        static ExportPackagesWindow()
        {
            _dataEditorPrefsKey = typeof(ExportPackagesWindow).FullName;
        }

        private PackagePresetsDatabase _presetsDatabase;
        private ExportPackagesWindowData _data;
        
        [MenuItem("Tools/Package Exporter/Export window")]
        private static void ShowExportWindow()
        {
            GetWindow<ExportPackagesWindow>();
        }

        private ExportPackagesWindowData CreateDataAsset()
        {
            if (string.IsNullOrEmpty(_dataEditorPrefsKey))
            {
                throw new Exception($"{nameof(_dataEditorPrefsKey)} was not initialized");
            }

            ExportPackagesWindowData data = CreateInstance<ExportPackagesWindowData>();

            string defaultDir = $"Assets";
            if (!Directory.Exists(defaultDir))
                Directory.CreateDirectory(defaultDir);

            string assetPath = $"{defaultDir}/{nameof(ExportPackagesWindowData)}.asset";
            AssetDatabase.CreateAsset(data, assetPath);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssetIfDirty(data);
            AssetDatabase.Refresh();

            string guid = AssetDatabase.GUIDFromAssetPath(
                AssetDatabase.GetAssetPath(data)).ToString();

            EditorPrefs.SetString(_dataEditorPrefsKey, guid);

            return data;
        }

        private void OnEnable()
        {
            Load();
        }

        private void OnDisable()
        {
            Save();
        }

        public void Load()
        {
            string dataPath = AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString(_dataEditorPrefsKey));
            if (string.IsNullOrEmpty(dataPath) || !File.Exists(dataPath))
            {
                _data = CreateDataAsset();
            }
            else
            {
                _data = AssetDatabase.LoadAssetAtPath<ExportPackagesWindowData>(
                    AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString(_dataEditorPrefsKey)));
            }

            _presetsDatabase = _data.presetsDatabase;
        }

        public void Save()
        {
            if (_data == null)
            {
                Load();
            }

            _data.presetsDatabase = _presetsDatabase;

            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssetIfDirty(_data);
            //AssetDatabase.SaveAssets();
        }

        private void HandleDbControls()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Create database"))
            {
                _presetsDatabase = PackagePresetsDatabase.GetOrCreate();
                Save();
            }
            EditorGUI.BeginChangeCheck();
            var selectedDatabase = EditorGUILayout.ObjectField("Select database",
                _presetsDatabase, typeof(PackagePresetsDatabase), false) as PackagePresetsDatabase;
            if (EditorGUI.EndChangeCheck())
            {
                _presetsDatabase = selectedDatabase;
                Save();
            }
            GUILayout.EndVertical();
        }

        private void HandleDbEditor()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Edit Database"))
            {
                PackagePresetsDatabaseEditorWindow.ShowEditor(_presetsDatabase);
            }
            GUILayout.EndHorizontal();
        }

        private void HandleExportControls()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pack mode", GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            _data.packMode = (PackMode) EditorGUILayout.EnumPopup(_data.packMode);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Change output destination"))
            {
                string selectedFolder = EditorUtility.OpenFolderPanel("Select output directory", _data.destinationFolder, "");
                if (!string.IsNullOrEmpty(selectedFolder) || Directory.Exists(selectedFolder))
                    _data.destinationFolder = selectedFolder;
            }
            EditorGUILayout.LabelField("Export destination", _data.destinationFolder);
            GUILayout.FlexibleSpace();

            if (_data.packMode == PackMode.Standalone && !UnityPackagePresetsHasUniqueNames())
            {
                EditorGUILayout.HelpBox("Current database has presets with duplicate names!\n" +
                    "Conflicting packages will be overwritten", MessageType.Warning);
            }

            if (string.IsNullOrEmpty(_data.destinationFolder))
            {
                EditorGUILayout.HelpBox("You must specify package export destination!", 
                    MessageType.Warning);
            }
            else if (GUILayout.Button("EXPORT"))
            {
                PerformExport();
            }
        }

        private bool UnityPackagePresetsHasUniqueNames()
        {
            var presets = _presetsDatabase.Presets.ToArray();
            if (presets == null || presets.Length == 0)
                return true;

            HashSet<string> names = new HashSet<string>();
            foreach (var preset in presets)
            {
                var exportInfo = preset.exportInfo as UnityPackageExportInfo;
                if (exportInfo == null || !names.Add(exportInfo.fileName))
                    break;
            }

            return names.Count == presets.Length;
        }

        private void PerformExport()
        {
            ExportFactory exportFactory = new ExportFactory();
            ExportManager exportManager = new ExportManager(exportFactory);

            var presets = _presetsDatabase.Presets.ToArray();
            int presetsCount = _presetsDatabase.GetPresetsCount();

            // aggregate for unitypackage
            UnityPackageExportInfo totalUnityPackageExportInfo = new UnityPackageExportInfo();
            totalUnityPackageExportInfo.targetDirectory = _data.destinationFolder;
            totalUnityPackageExportInfo.fileName = _data.totalPackageName;
            totalUnityPackageExportInfo.exportPackageOptions = _data.exportUnityPackageOptions;
            for (int i = 0; i < presetsCount; i++)
            {
                var unityPackageExportInfo = presets[i].exportInfo as UnityPackageExportInfo;
                if (unityPackageExportInfo != null)
                {
                    if (string.IsNullOrEmpty(unityPackageExportInfo.fileName))
                    {
                        unityPackageExportInfo.fileName = $"UnityPackage{i}";
                    }
                }
            }
            //

            if (_data.packMode == PackMode.Batch)
            {
                Dictionary<Type, PackagePreset> groupedPresets = GetGroupedPresets(presets);
                foreach (var presetPair in groupedPresets)
                {
                    if (presetPair.Key.IsAssignableFrom(typeof(UnityPackageExportInfo)) &&
                        presetPair.Value.exportInfo is UnityPackageExportInfo unityPackageExportInfo)
                    {
                        presetPair.Value.exportInfo = totalUnityPackageExportInfo;
                    }

                    bool exportSuccesful = exportManager.Export(presetPair.Value);

                    if (exportSuccesful)
                        Debug.Log($"Export ({nameof(PackMode.Batch)}): Succesful exported {presetPair.Value.name} " +
                            $"to {presetPair.Value.exportInfo.GetTargetPath()}");
                    else
                        Debug.LogError($"Export ({nameof(PackMode.Batch)}): Error while exporting {presetPair.Value.name} " +
                            $"to {presetPair.Value.exportInfo.GetTargetPath()}");
                }
            }
            else if (_data.packMode == PackMode.Standalone)
            {
                foreach (var preset in presets)
                {
                    if (preset.exportInfo is UnityPackageExportInfo unityPackageExportInfo)
                    {
                        unityPackageExportInfo.exportPackageOptions = totalUnityPackageExportInfo.exportPackageOptions;
                        unityPackageExportInfo.targetDirectory = totalUnityPackageExportInfo.targetDirectory;
                    }

                    bool exportSuccesful = exportManager.Export(preset);

                    if (exportSuccesful)
                        Debug.Log($"Export ({nameof(PackMode.Standalone)}): Succesful exported {preset.name} " +
                            $"to {preset.exportInfo.GetTargetPath()}");
                    else
                        Debug.LogError($"Export ({nameof(PackMode.Standalone)}): Error while exporting {preset.name} " +
                            $"to {preset.exportInfo.GetTargetPath()}");
                }
            }
        }

        private Dictionary<Type, PackagePreset> GetGroupedPresets(PackagePreset[] presets)
        {
            Dictionary<Type, PackagePreset> groupedPresets = new();
            HashSet<Type> presetTypes = new();
            foreach (PackagePreset preset in presets)
            {
                if (preset.exportInfo is not PackageExportInfo)
                {
                    Debug.Log($"Preset type: {preset.exportInfo.GetType()}\nRequried type: {typeof(PackageExportInfo)}");
                    throw new NotSupportedException($"Provided type {preset.exportInfo.GetType()} is not correct");
                }

                presetTypes.Add(preset.exportInfo.GetType());
            }

            foreach (Type t in presetTypes)
            {
                if (!groupedPresets.ContainsKey(t))
                {
                    PackagePreset newPreset = ScriptableObject.CreateInstance<PackagePreset>();
                    newPreset.name = t.Name;
                    newPreset.exportInfo = (PackageExportInfo) Activator.CreateInstance(t);
                    newPreset.exportInfo.targetDirectory = _data.destinationFolder;
                    groupedPresets.Add(t, newPreset);
                }

                foreach (PackagePreset preset in presets.Where(p => p.exportInfo.GetType() == t))
                {
                    groupedPresets[t].packageInfo.AddFiles(preset.packageInfo.GetFiles());
                }
            }

            Type unityT = typeof(UnityPackageExportInfo);
            if (groupedPresets.ContainsKey(unityT))
            {
                foreach (PackagePreset preset in presets.Where(p => p.exportInfo.GetType() == unityT))
                {
                    ((UnityPackageExportInfo) groupedPresets[unityT].exportInfo).exportPackageOptions
                        |= ((UnityPackageExportInfo) preset.exportInfo).exportPackageOptions;
                }
            }

            return groupedPresets;
        }

        private void OnGUI()
        {
            HandleDbControls();
            if (_presetsDatabase == null)
                return;
            if (_data == null) 
                return;

            //GUILayout.FlexibleSpace();
            HandleDbEditor();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();

            if (_data.packMode == PackMode.Batch)
                _data.totalPackageName = EditorGUILayout.TextField("Total pack name", _data.totalPackageName);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Export options (only for .unitypackage)", GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            _data.exportUnityPackageOptions = (ExportPackageOptions) 
                EditorGUILayout.EnumFlagsField(_data.exportUnityPackageOptions);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            //GUILayout.FlexibleSpace();

            HandleExportControls();
            GUILayout.FlexibleSpace();

            // misc
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Total presets to export: {_presetsDatabase.GetPresetsCount()}");
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
    }

    public class PackagePresetsDatabaseEditorWindow : EditorWindow
    {
        private PackagePresetsDatabase _presetsDatabase;

        private bool _presetPickerShown = false;
        private int _pickerControlID;

        public static void ShowEditor(PackagePresetsDatabase presetsDatabase)
        {
            GetWindow<PackagePresetsDatabaseEditorWindow>()._presetsDatabase = presetsDatabase;
        }

        private void OnEnable()
        {
            _pickerControlID = (int) (DateTime.UtcNow.Ticks % int.MaxValue);
        }

        private void OnDisable()
        {
            
        }

        private void DrawAdditionButton()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add preset"))
            {
                _presetPickerShown = true;
            }
            GUILayout.EndHorizontal();

            if (_presetPickerShown)
            {
                _presetPickerShown = false;
                EditorGUIUtility.ShowObjectPicker<PackagePreset>(null, false, "", _pickerControlID);
            }
        }

        private void HandlePresetAddition()
        {
            if (Event.current.commandName == "ObjectSelectorUpdated" &&
                EditorGUIUtility.GetObjectPickerControlID() == _pickerControlID)
            {
                PackagePreset selected = EditorGUIUtility.GetObjectPickerObject() as PackagePreset;
                if (selected != null)
                {
                    _presetsDatabase.AddPreset(selected);
                    _presetsDatabase.Save();
                }
            }
        }

        private Vector2 _presetsListScrollPos;

        private void HandlePresetRemoval()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginScrollView(_presetsListScrollPos);

            var presets = _presetsDatabase.Presets;
            for (int i = 0; i < presets.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.SelectableLabel(AssetDatabase.GetAssetPath(presets[i].GetInstanceID()));
                if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                {
                    presets.RemoveAt(i);
                    i--;

                    _presetsDatabase.Save();
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                _presetsDatabase.Save();
            }
        }

        private void OnGUI()
        {
            if (_presetsDatabase == null)
                return;

            DrawAdditionButton();
            HandlePresetAddition();
            EditorGUILayout.Space();

            HandlePresetRemoval();
        }
    }
}
