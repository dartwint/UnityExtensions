using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AssetPathRetriever
{
    public static string ProjectDirectoryPath { get; private set; } =
        Application.dataPath.Substring(0, Application.dataPath.Length - 6);

    public static string GetRelativeAssemblyDir<T>(Assembly assembly)
    {
        return Path.GetRelativePath(Application.dataPath, assembly.Location).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static string GetScriptPath(Type type)
    {
        string[] guids = AssetDatabase.FindAssets(type.Name + " t:Script");

        if (guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return assetPath;
        }

        return null;
    }

    public static string GetScriptRootDir(Type type)
    {
        string assetPath = GetScriptPath(type);

        if (assetPath == null) return null;

        string[] parts = assetPath.Split('/');
        return assetPath.Replace(parts[parts.Length - 1], "");
    }

    public static string GetUnityObjectPath<T>(T scriptableObjectInstance) where T : UnityEngine.Object
    {
        string assetPath = AssetDatabase.GetAssetPath(scriptableObjectInstance);

        if (string.IsNullOrEmpty(assetPath)) return null;

        string[] parts = assetPath.Split('/');

        return assetPath.Replace(parts[parts.Length - 1], "");
    }

    public static string GetAssetPath(string absolutePath, bool setAltSeparator = true)
    {
        if (string.IsNullOrEmpty(absolutePath)) return string.Empty;

        absolutePath = Path.GetRelativePath(ProjectDirectoryPath, absolutePath);

        if (setAltSeparator)
            absolutePath = absolutePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return absolutePath;
    }
}
