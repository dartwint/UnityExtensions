using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportPackagesWindow : EditorWindow
    {
        private PackagePresetsDatabase _presetsDatabase;
        //private ExportPresetsViewModel _viewModel;

        private ExportPackagesWindowData _data;
        private const string _dataDir = "Assets/PackageExporter";
        private readonly string _dataAssetPath = 
            string.Concat(_dataDir, "/", typeof(ExportPackagesWindowData).Name, ".asset");

        private string _destinationFolder;
        
        [MenuItem("Tools/Package Exporter/Export window")]
        private static void ShowExportWindow()
        {
            GetWindow<ExportPackagesWindow>();
        }

        private ExportPackagesWindowData CreateDataAsset()
        {
            var data = CreateInstance<ExportPackagesWindowData>();

            if (!Directory.Exists(_dataDir))
                Directory.CreateDirectory(_dataDir);

            AssetDatabase.CreateAsset(data, _dataAssetPath);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssetIfDirty(data);

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
            _data = AssetDatabase.LoadAssetAtPath<ExportPackagesWindowData>(_dataAssetPath);
            if (_data == null)
            {
                _data = CreateDataAsset();
            }
            _presetsDatabase = _data.presetsDatabase;
            _destinationFolder = _data.destinationFolder;
        }

        public void Save()
        {
            if (_data == null)
            {
                Load();
            }

            _data.presetsDatabase = _presetsDatabase;
            _data.destinationFolder = _destinationFolder;

            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssetIfDirty(_data);
            AssetDatabase.SaveAssets();
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

        private void HandleDbOverrides()
        {
            GUILayout.BeginHorizontal();
            _data.overrideExportOptions = EditorGUILayout.Toggle("Override DB options", _data.overrideExportOptions);
            GUILayout.EndHorizontal();
            if (_data.overrideExportOptions)
            {
                GUILayout.BeginVertical();
                _data.packMode = (PackMode) EditorGUILayout.EnumPopup("Pack mode", _data.packMode);
                if (_data.packMode == PackMode.Batch)
                    _data.totalPackageName = EditorGUILayout.TextField("Total pack name", _data.totalPackageName);
                GUILayout.EndVertical();
            }
        }

        private void HandleExportControls()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Change output destination"))
            {
                _destinationFolder = EditorUtility.OpenFolderPanel("Select output directory", _destinationFolder, "");
            }
            EditorGUILayout.LabelField("Export destination", _destinationFolder);
            GUILayout.EndVertical();
        }

        private void OnGUI()
        {
            HandleDbControls();
            if (_presetsDatabase == null)
                return;
            //GUILayout.FlexibleSpace();
            HandleDbEditor();
            GUILayout.FlexibleSpace();

            HandleDbOverrides();
            GUILayout.FlexibleSpace();

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
                EditorGUIUtility.ShowObjectPicker<PackagePresetNEW>(null, false, "", _pickerControlID);
            }
        }

        private void HandlePresetAddition()
        {
            if (Event.current.commandName == "ObjectSelectorUpdated" &&
                EditorGUIUtility.GetObjectPickerControlID() == _pickerControlID)
            {
                PackagePresetNEW selected = EditorGUIUtility.GetObjectPickerObject() as PackagePresetNEW;
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
