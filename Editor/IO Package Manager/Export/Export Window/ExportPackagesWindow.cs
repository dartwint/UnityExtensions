using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class ExportPackagesWindow : EditorWindow
    {
        private static readonly string _dataEditorPrefsKey;
        static ExportPackagesWindow()
        {
            _dataEditorPrefsKey = typeof(ExportPackagesWindow).FullName;
        }

        private PackagesDatabase _presetsDatabase;
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
                _presetsDatabase = PackagesDatabase.GetOrCreate();
                Save();
            }
            EditorGUI.BeginChangeCheck();
            var selectedDatabase = EditorGUILayout.ObjectField("Select database",
                _presetsDatabase, typeof(PackagesDatabase), false) as PackagesDatabase;
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
            var presets = _presetsDatabase.Packages.ToArray();
            if (presets == null || presets.Length == 0)
                return true;

            HashSet<string> names = new HashSet<string>();
            foreach (var preset in presets)
            {
                var exportInfo = preset.exportOptions as UnityPackageExportOptions;
                if (exportInfo == null || !names.Add(exportInfo.fileName))
                    break;
            }

            return names.Count == presets.Length;
        }

        private void PerformExport()
        {
            ExportProcessorFactory exportFactory = new ExportProcessorFactory();
            ExportManager exportManager = new ExportManager(exportFactory);

            var presets = _presetsDatabase.Packages.ToArray();
            int presetsCount = _presetsDatabase.GetPresetsCount();

            // aggregate for unitypackage
            UnityPackageExportOptions totalUnityPackageExportInfo = new UnityPackageExportOptions();
            totalUnityPackageExportInfo.targetDirectory = _data.destinationFolder;
            totalUnityPackageExportInfo.fileName = _data.totalPackageName;
            totalUnityPackageExportInfo.exportPackageOptions = _data.exportUnityPackageOptions;
            for (int i = 0; i < presetsCount; i++)
            {
                var unityPackageExportInfo = presets[i].exportOptions as UnityPackageExportOptions;
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
                Dictionary<Type, Package> groupedPresets = GetGroupedPresets(presets);
                foreach (var presetPair in groupedPresets)
                {
                    if (presetPair.Key.IsAssignableFrom(typeof(UnityPackageExportOptions)) &&
                        presetPair.Value.exportOptions is UnityPackageExportOptions unityPackageExportInfo)
                    {
                        presetPair.Value.exportOptions = totalUnityPackageExportInfo;
                    }

                    bool exportSuccesful = exportManager.Export(presetPair.Value);

                    if (exportSuccesful)
                        Debug.Log($"Export ({nameof(PackMode.Batch)}): Succesful exported {presetPair.Value.name} " +
                            $"to {presetPair.Value.exportOptions.GetTargetPath()}");
                    else
                        Debug.LogError($"Export ({nameof(PackMode.Batch)}): Error while exporting {presetPair.Value.name} " +
                            $"to {presetPair.Value.exportOptions.GetTargetPath()}");
                }
            }
            else if (_data.packMode == PackMode.Standalone)
            {
                foreach (var preset in presets)
                {
                    if (preset.exportOptions is UnityPackageExportOptions unityPackageExportInfo)
                    {
                        unityPackageExportInfo.exportPackageOptions = totalUnityPackageExportInfo.exportPackageOptions;
                        unityPackageExportInfo.targetDirectory = totalUnityPackageExportInfo.targetDirectory;
                    }

                    bool exportSuccesful = exportManager.Export(preset);

                    if (exportSuccesful)
                        Debug.Log($"Export ({nameof(PackMode.Standalone)}): Succesful exported {preset.name} " +
                            $"to {preset.exportOptions.GetTargetPath()}");
                    else
                        Debug.LogError($"Export ({nameof(PackMode.Standalone)}): Error while exporting {preset.name} " +
                            $"to {preset.exportOptions.GetTargetPath()}");
                }
            }
        }

        private Dictionary<Type, Package> GetGroupedPresets(Package[] packages)
        {
            Dictionary<Type, Package> groupedPresets = new();
            HashSet<Type> packagesTypes = new();
            foreach (Package preset in packages)
            {
                if (preset.exportOptions is not PackageExportOptions)
                {
                    Debug.Log($"Package type: {preset.exportOptions.GetType()}\nRequried type: {typeof(PackageExportOptions)}");
                    throw new NotSupportedException($"Provided type {preset.exportOptions.GetType()} is not correct");
                }

                packagesTypes.Add(preset.exportOptions.GetType());
            }

            foreach (Type t in packagesTypes)
            {
                if (!groupedPresets.ContainsKey(t))
                {
                    Package newPackage = ScriptableObject.CreateInstance<Package>();
                    newPackage.name = t.Name;
                    newPackage.exportOptions = (PackageExportOptions) Activator.CreateInstance(t);
                    newPackage.exportOptions.targetDirectory = _data.destinationFolder;
                    groupedPresets.Add(t, newPackage);
                }

                foreach (Package package in packages.Where(p => p.exportOptions.GetType() == t))
                {
                    groupedPresets[t].packageFiles.AddFiles(package.packageFiles.GetFiles());
                }
            }

            Type unityT = typeof(UnityPackageExportOptions);
            if (groupedPresets.ContainsKey(unityT))
            {
                foreach (Package package in packages.Where(p => p.exportOptions.GetType() == unityT))
                {
                    ((UnityPackageExportOptions) groupedPresets[unityT].exportOptions).exportPackageOptions
                        |= ((UnityPackageExportOptions) package.exportOptions).exportPackageOptions;
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
        private PackagesDatabase _packagesDatabase;

        private bool _presetPickerShown = false;
        private int _pickerControlID;

        public static void ShowEditor(PackagesDatabase presetsDatabase)
        {
            GetWindow<PackagePresetsDatabaseEditorWindow>()._packagesDatabase = presetsDatabase;
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
                EditorGUIUtility.ShowObjectPicker<Package>(null, false, "", _pickerControlID);
            }
        }

        private void HandlePresetAddition()
        {
            if (Event.current.commandName == "ObjectSelectorUpdated" &&
                EditorGUIUtility.GetObjectPickerControlID() == _pickerControlID)
            {
                var selected = EditorGUIUtility.GetObjectPickerObject() as Package;
                if (selected != null)
                {
                    _packagesDatabase.AddPackage(selected);
                    _packagesDatabase.Save();
                }
            }
        }

        private Vector2 _packagesListScrollPos;

        private void HandlePresetRemoval()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginScrollView(_packagesListScrollPos);

            var packages = _packagesDatabase.Packages;
            for (int i = 0; i < packages.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.SelectableLabel(AssetDatabase.GetAssetPath(packages[i].GetInstanceID()));
                if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                {
                    packages.RemoveAt(i);
                    i--;

                    _packagesDatabase.Save();
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                _packagesDatabase.Save();
            }
        }

        private void OnGUI()
        {
            if (_packagesDatabase == null)
                return;

            DrawAdditionButton();
            HandlePresetAddition();
            EditorGUILayout.Space();

            HandlePresetRemoval();
        }
    }
}
