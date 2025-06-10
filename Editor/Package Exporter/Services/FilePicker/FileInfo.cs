using System.IO;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class FileInfo
    {
        public string FullPath { get; private set; }
        public bool IsDirectory { get; private set; }
        public bool IsUnityAsset { get; private set; }

        //private bool _insideProject;

        public FileInfo(string path)
        {
            FullPath = path;
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
