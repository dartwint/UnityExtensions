using UnityEditor;
using UnityEngine;

public class BuildMonitorWindow : EditorWindow
{
    private static ScriptStatusMonitor _compilationStatusMonitor;
    private static BuildStatusSettings _settings;

    private static bool _autoReloadFlag = true;
    public static bool AutoReload
    {
        get => _autoReloadFlag;
        set 
        {
            if (_autoReloadFlag == value) 
                return;

            if (value == true)
            {
                AutoCompileSwitch.EnableDomainReload();
            }
            else
            {
                AutoCompileSwitch.DisableDomainReload();
            }

            _autoReloadFlag = value; 
        }
    }

    // FIX ME: there is only 2 statuses for scripts
    private enum BuildStatus { Updated, NotUpdated_WithScripts, NotUpdated_WithoutScripts, ReloadPending }
    private static BuildStatus _currentStatus = BuildStatus.Updated;

    //private Vector2 _scrollPos;

    private void ReloadAssemblies() => AssetDatabase.Refresh();

    public BuildStatusSettings LoadSettingsAsset()
    {
        string scriptPath = AssetDatabase.GetAssetOrScenePath(MonoScript.FromScriptableObject(this));
        string scriptDir = scriptPath.Substring(0, scriptPath.LastIndexOf('/') + 1);

        string settingsDir = string.Concat(scriptDir, BuildStatusSettings.settingsAssetDir);

        string[] guids = AssetDatabase.FindAssets("", new[] { settingsDir });
        if (guids.Length == 0)
        {
            //var newAsset = ScriptableObject.CreateInstance<BuildStatusSettings>();
            Debug.LogError("Settings asset not found");
            return null;
        }

        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

        BuildStatusSettings asset = AssetDatabase.LoadAssetAtPath<BuildStatusSettings>(assetPath);
        return asset;
    }

    private bool LoadData()
    {
        _settings = LoadSettingsAsset();
        if (_settings == null)
            return false;

        _autoReloadFlag = _settings.autoReload;

        return true;
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        
    }

    private void Awake()
    {
        if (!LoadData())
            Debug.LogError("BuildStatusSettings asset not found!");

        _compilationStatusMonitor = new ScriptStatusMonitor(
            new ScriptChangeMonitor(new FileSystemWatcherScriptChangeTracker()));
        _compilationStatusMonitor.DomainStatusChanged += OnAssembliesStatusChanged;
        _compilationStatusMonitor.Enabled = true;
    }

    private void OnAssembliesStatusChanged(ScriptStatus status)
    {
        UpdateStatus();
        UpdateIcon();
    }

    private Texture GetIcon(BuildStatus status)
    {
        if (_settings == null)
            return null;

        if (status == BuildStatus.Updated) return _settings.upToDateIcon;
        if (status == BuildStatus.NotUpdated_WithoutScripts) return _settings.outdatedAssetsWithoutScriptsIcon;
        if (status == BuildStatus.NotUpdated_WithScripts) return _settings.outdatedIcon;
        if (status == BuildStatus.ReloadPending) return _settings.outdatedIcon;

        return null;
    }

    [MenuItem("Tools/Assembly Reload Monitor")]
    public static void ShowWindow()
    {
        var window = GetWindow<BuildMonitorWindow>();
    }

    private void OnEnable()
    {
        UpdateStatus();
        UpdateIcon();
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
        Debug.Log($"OnDestroy() object:{this}. Has Monitor instance: {_compilationStatusMonitor != null}");

        _compilationStatusMonitor.DomainStatusChanged -= OnAssembliesStatusChanged;
        _compilationStatusMonitor.Dispose();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Build is up to date", EditorStyles.largeLabel);

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Auto reload: " + AutoReload, GUILayout.Width(200));
        EditorGUI.BeginChangeCheck();

        if (AutoReload)
        {
            if (GUILayout.Button("Disable auto reload", GUILayout.Width(200), GUILayout.Height(30)))
            {
                AutoReload = false;
                _settings.AutoCompile = false;
            }
        }
        else
        {
            if (GUILayout.Button("Enable auto reload", GUILayout.Width(200), GUILayout.Height(30)))
            {
                _settings.AutoCompile = true;
                AutoReload = true;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssetIfDirty(_settings);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void UpdateIcon()
    {
        Texture icon = GetIcon(_currentStatus);
        titleContent = new GUIContent("Assembly Reload Monitor", icon);
        Repaint();
    }

    private void UpdateStatus()
    {
        if (_compilationStatusMonitor == null)
            return;

        if (_compilationStatusMonitor.Status == ScriptStatus.Actual) _currentStatus = BuildStatus.Updated;
        else if (_compilationStatusMonitor.Status == ScriptStatus.Compiling) _currentStatus = BuildStatus.ReloadPending;
        else if (_compilationStatusMonitor.Status == ScriptStatus.HasChanges) _currentStatus = BuildStatus.NotUpdated_WithScripts;
    }
}