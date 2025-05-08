using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetFinder
{
    public static string ProjectRootDir
    {
        get => Directory.GetParent(Application.dataPath).FullName.UnifyDirPath();
    }

    public static string AssetsRelativePath
    {
        get => Path.GetRelativePath(
            Directory.GetParent(Application.dataPath).FullName, 
            Application.dataPath)
            .UnifyDirPath();
    }

    public static IEnumerable<string> GetAssetsPath(string filter)
    {
        var guids = AssetDatabase.FindAssets(filter);
        if (!guids.Any())
            yield break;

        foreach (var guid in guids)
        {
            yield return AssetDatabase.GUIDToAssetPath(guid);
        }
    }

    public static string GetMonoScriptPath(MonoScript monoScript)
    {
        if (monoScript == null)
            return null;

        return AssetDatabase.GetAssetPath(monoScript);
    }

    public static string GetAssetDirPath(string path) => 
        path.UnifyDirSeparator().Substring(0, path.LastIndexOf('/'));

    public static string GetAssetRelativePath(string absolutePath)
    {
        if (string.IsNullOrEmpty(absolutePath)) return string.Empty;

        return Path.GetRelativePath(ProjectRootDir, absolutePath).UnifyDirSeparator();
    }
}