using System.IO;
using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class FileInfo
    {
        public string FullPath { get; private set; }
        public bool IsDirectory { get; private set; }
        public bool IsUnityAsset { get; private set; }

        //private bool _insideProject;

        public FileInfo(string path, bool isDirectory, bool isUnityAsset)
        {
            FullPath = path;
            IsDirectory = isDirectory;
            IsUnityAsset = isUnityAsset;
        }

        public static FileInfo ConvertFromPath(string path)
        {
            if (!FileExists(path))
            {
                return null;
            }

            bool isDirectory = Directory.Exists(path);
            bool isUnityAsset = UnityAssetExist(path);
            return new FileInfo(path, isDirectory, isUnityAsset);
        }

        public static bool UnityAssetExist(string path)
        {
            if (!FileExists(path))
            {
                return false;
            }

            return !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path));
        }

        public static bool FileExists(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
                return true;

            return false;
        }

        public static string GetMetaFile(FileInfo fileInfo)
        {
            if (fileInfo == null || !fileInfo.IsUnityAsset)
                return null;

            string metaFile = fileInfo.FullPath.Substring(0, fileInfo.FullPath.LastIndexOf('.')) + "meta";
            if (File.Exists(metaFile))
                return metaFile;

            return null;
        }
    }
}
