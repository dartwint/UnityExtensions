using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// View
public class ExportUnityPackageWindow : EditorWindow
{
    private ExportWindowData _data;
    private string _dataLocalDir = "Window data/";
    private string _dataAssetPath = "ExportWindowData.asset";

    private string _currentDir;

    private List<PackagePreset> _presets = new();

    private string _outputDirectory = "";

    private const string outputDirectoryKey = "ExportUnityPackageConfigOutputDirectory";

    private enum PackModes { Together, Separately }
    private PackModes _packMode = PackModes.Separately;
    private const string packModeKey = "ExportUnityPackageConfigPackMode";
    private string totalPackName = string.Empty;

    private void Awake()
    {
        var monoScript = MonoScript.FromScriptableObject(this);
        _currentDir = AssetFinder.GetMonoScriptPath(monoScript);
        _currentDir = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));

        _dataAssetPath = _dataAssetPath.Insert(0, _currentDir + _dataLocalDir);
    }

    private void OnEnable()
    {
        LoadData();

        if (totalPackName == string.Empty) totalPackName = Application.productName + " Pack";
    }


    [MenuItem("Tools/Export/Export Unity Package")]
    public static void ShowWindow()
    {
        GetWindow<ExportUnityPackageWindow>("Create Unity Package");
    }

    private void OnGUI()
    {
        GUILayout.Label("Unity Package Export Config", EditorStyles.boldLabel);

        PackagePreset newConfig = (PackagePreset) EditorGUILayout.ObjectField(
            "New Config",
            null,
            typeof(PackagePreset),
            false);

        if (newConfig != null && !_presets.Contains(newConfig))
        {
            _presets.Add(newConfig);

            SaveData();
        }

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < _presets.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            _presets[i] = (PackagePreset) EditorGUILayout.ObjectField(_presets[i], typeof(PackagePreset), false);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                _presets.RemoveAt(i);
                i--;

                SaveData();
            }

            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            SaveData();
        }

        GUILayout.Space(15);

        GUILayout.Label("Output", EditorStyles.label);

        GUILayout.BeginHorizontal();
        _outputDirectory = EditorGUILayout.TextField("Directory", _outputDirectory);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Output Directory", _outputDirectory, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                _outputDirectory = selectedPath;
            }
        }
        GUILayout.EndHorizontal();

        _packMode = (PackModes) EditorGUILayout.EnumPopup("Pack mode", _packMode);

        if (_packMode == PackModes.Together)
        {
            totalPackName = EditorGUILayout.TextField("Total pack name", totalPackName);
        }

        if (GUILayout.Button("Export Packages") && _presets.Count != 0)
        {
            ExportPackage();
        }
    }

    private void LoadData()
    {
        _packMode = (PackModes) EditorPrefs.GetInt(packModeKey, (int) PackModes.Separately);

        _outputDirectory = EditorPrefs.GetString(outputDirectoryKey, "");

        if (AssetDatabase.LoadAssetAtPath<ExportWindowData>(_dataAssetPath) == null)
        {
            CreateNewData();
            return;
        }

        if (_data == null)
        {
            _data = AssetDatabase.LoadAssetAtPath<ExportWindowData>(_dataAssetPath);

            _presets.Clear();
            _presets.AddRange(_data.configs);
        }
    }

    private void SaveData()
    {
        EditorPrefs.SetInt(packModeKey, (int) _packMode);
        EditorPrefs.SetString(outputDirectoryKey, _outputDirectory);

        _data.configs = _presets.Where(c => c != null).ToArray();

        EditorUtility.SetDirty(_data);
        AssetDatabase.SaveAssetIfDirty(_data);
    }

    private void CreateNewData()
    {
        _data = CreateInstance<ExportWindowData>();
        if (!Directory.Exists(_currentDir + _dataLocalDir)) Directory.CreateDirectory(_currentDir + _dataLocalDir);
        AssetDatabase.CreateAsset(_data, _dataAssetPath);

        EditorUtility.SetDirty(_data);
        AssetDatabase.SaveAssetIfDirty(_data);

        EditorUtility.FocusProjectWindow();
    }

    private void ExportPackage()
    {
        if (_presets.Count == 0)
        {
            Debug.LogAssertion("UnityPackagesConfigs not found. You have to create it.");
            return;
        }

        List<string> assetPaths = new();
        ExportPackageOptions options;
        for (int i = 0; i < _presets.Count; i++)
        {
            options = _presets[i].ExportPackageOptions;
            if (options.HasFlag(ExportPackageOptions.Interactive) && i < _presets.Count - 1)
            {
                options &= ~ExportPackageOptions.Interactive;
            }

            assetPaths.AddRange(_presets[i].assetPaths);

            string packagePath;
            if (_packMode == PackModes.Together && i == _presets.Count - 1)
            {
                packagePath = Path.Combine(_outputDirectory, totalPackName + ".unitypackage");
            }
            else
            {
                packagePath = Path.Combine(_outputDirectory, _presets[i].packageName + ".unitypackage");
            }

            try
            {
                if (_packMode == PackModes.Together && i < _presets.Count - 1) continue;

                AssetDatabase.ExportPackage(assetPaths.ToArray(), packagePath, options);
                Debug.Log("Unity Package created at: " + packagePath);
                assetPaths.Clear();
            }
            catch
            {
                Debug.LogAssertion($"Assets in config {_presets[i]} does not exist");
                assetPaths.Clear();
            }
        }
    }
}
