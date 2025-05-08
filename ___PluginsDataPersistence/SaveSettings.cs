using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SaveSettings : ScriptableObject
{
    private static string _pluginsDataRootFolder = "PluginsData";

    public static string PluginsDataRootFolder
    {
        get => string.Concat(AssetFinder.AssetsRelativePath, _pluginsDataRootFolder);

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

    public virtual string GetTargetPath() =>
        Path.Combine(PluginsDataRootFolder, AssetName + ".asset").UnifyDirSeparator();

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        CheckFolders();
        CheckInstance<SaveSettings>();
    }

    private static bool HasInstanceOf<T>() where T : SaveSettings
    {
        var dummyInstance = CreateInstance<T>();
        if (dummyInstance == null)
            throw new System.Exception($"Settings asset with type: {typeof(T)} creation failed");

        if (AssetDatabase.FindAssets($"{dummyInstance.AssetName} t:ScriptableObject", new[] { dummyInstance.GetTargetPath() }).Length == 0)
            return false;

        return true;
    }

    protected static bool HasInstanceOf<T>(string assetPath) where T : SaveSettings =>
        AssetDatabase.AssetPathExists($"{assetPath}");

    private static void CheckInstance<T>() where T : SaveSettings
    {
        var newInstance = CreateInstance<T>();
        if (newInstance == null)
            throw new System.Exception($"Settings asset with type: {typeof(T)} creation failed");

        string targetDir = string.Concat(PluginsDataRootFolder);

        if (AssetDatabase.FindAssets($"{newInstance.AssetName} t:ScriptableObject", new[] { targetDir }).Length == 0)
        {
            AssetDatabase.CreateAsset(newInstance, targetDir + $"/{newInstance.AssetName}.asset");
            AssetDatabase.Refresh();
        }

        //var monoScript = MonoScript.FromScriptableObject(newInstance);
        //if (monoScript == null)
        //    throw new System.Exception($"Object {newInstance} with type: {typeof(T)} cannot be casted to MonoScript");
    }

    private static void CheckFolders()
    {
        if (AssetDatabase.AssetPathExists(PluginsDataRootFolder))
            return;

        string[] dirs = PluginsDataRootFolder.Split('/');
        if (dirs.Length < 2)
            return;

        StringBuilder parentFolderBuilder = new StringBuilder(dirs[0]);
        int i = 1;
        while (i < dirs.Length)
        {
            if (!AssetDatabase.AssetPathExists(parentFolderBuilder.ToString() + $"/{dirs[i]}"))
            {
                AssetDatabase.CreateFolder(string.Concat(parentFolderBuilder.ToString()), dirs[i]);
            }

            parentFolderBuilder.Append($"/{dirs[i]}");
            i++;
        }
    }
}