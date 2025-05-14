using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PluginDataWrapper : ScriptableObject
{
    private static PluginDataWrapper _defaultInstance;

    private static string _pluginsDataRootFolder = "PluginsData";

    public static string PluginsDataRootFolder
    {
        get => string.Concat(AssetFinder.AssetsDirectoryRelativePath, _pluginsDataRootFolder);

        private set
        {
            _pluginsDataRootFolder = value;
        }
    }

    private string _assetName = "PluginData";
    public string AssetName
    {
        get => _assetName;
        protected set
        {
            _assetName = value;
        }
    }

    private string _pluginName = "__";
    public string PluginName
    {
        get => _pluginName;
        protected set
        {
            _pluginName = value;
        }
    }

    public string GetTargetPath() =>
        Path.Combine(PluginsDataRootFolder, PluginName, AssetName + ".asset").UnifyDirSeparator();

    public string GetTargetDir() =>
        Directory.GetParent(GetTargetPath()).FullName.UnifyDirSeparator();

/*    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        _defaultInstance = CreateInstance<PluginDataWrapper>();
        _defaultInstance.CreateFolders();
        SaveInstanceIfNotExist(_defaultInstance);
    }*/

    public static void InitializeDataAsset(PluginDataWrapper dataAsset, bool overwrite = false)
    {
        if (!AssetDatabase.AssetPathExists(dataAsset.GetTargetPath()))
        {
            SaveNewInstanceIfNotExist<PluginDataWrapper>();
            return;
        }

        if (!overwrite)
            return;

        AssetDatabase.DeleteAsset(dataAsset.GetTargetPath());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        SaveNewInstanceIfNotExist<PluginDataWrapper>();
    }

    private static bool HasInstanceOf<T>() where T : PluginDataWrapper
    {
        var dummyInstance = CreateInstance<T>();
        if (dummyInstance == null)
            throw new System.Exception($"Settings asset with type: {typeof(T)} creation failed");

        if (AssetDatabase.FindAssets($"{dummyInstance.AssetName} t:ScriptableObject", new[] { dummyInstance.GetTargetPath() }).Length == 0)
            return false;

        return true;
    }

    private static void SaveInstanceIfNotExist(PluginDataWrapper pluginData)
    {
        string targetDir = AssetFinder.GetRelativeToAssetsPath(pluginData.GetTargetDir());
        if (AssetDatabase.FindAssets($"{pluginData.AssetName} t:ScriptableObject", new[] { targetDir }).Length == 0)
        {
            string path = Path.Combine(targetDir, $"{pluginData.AssetName}.asset").UnifyDirSeparator();
            AssetDatabase.CreateAsset(pluginData, path);
            AssetDatabase.Refresh();
        }
    }
    //
    private static void SaveNewInstanceIfNotExist<T>() where T : PluginDataWrapper
    {
        var newInstance = CreateInstance<T>();
        if (newInstance == null)
            throw new System.Exception($"Settings asset with type: {typeof(T)} creation failed");

        string targetDir = AssetFinder.GetRelativeToAssetsPath(newInstance.GetTargetDir());
        if (AssetDatabase.FindAssets($"{newInstance.AssetName} t:ScriptableObject", new[] { targetDir }).Length == 0)
        {
            AssetDatabase.CreateAsset(newInstance, Path.Combine(targetDir, $"{newInstance.AssetName}.asset").UnifyDirSeparator());
            AssetDatabase.Refresh();
        }
    }
    
    private void CreateFolders()
    {
        if (AssetDatabase.AssetPathExists(AssetFinder.GetRelativeToAssetsPath(GetTargetDir())))
            return;

        string[] dirs = AssetFinder.GetRelativeToAssetsPath(GetTargetDir()).Split('/');
        if (dirs.Length < 2)
            return;

        StringBuilder parentFolderBuilder = new StringBuilder(dirs[0]);
        int i = 1;
        while (i < dirs.Length)
        {
            if (!AssetDatabase.AssetPathExists(parentFolderBuilder.ToString() + $"/{dirs[i]}"))
            {
                AssetDatabase.CreateFolder(parentFolderBuilder.ToString(), dirs[i]);
            }

            parentFolderBuilder.Append($"/{dirs[i]}");
            i++;
        }
    }
}