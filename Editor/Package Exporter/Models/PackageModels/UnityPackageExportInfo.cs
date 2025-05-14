using System.IO;
using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class UnityPackageExportInfo
    {
        public string targetDirectory;
        public ExportPackageOptions exportPackageOptions;

        public string GetTargetPath(string fileName) =>
           Path.Combine(targetDirectory, $"{fileName}.unitypackage");
    }
}
