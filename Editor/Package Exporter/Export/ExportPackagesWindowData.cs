using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportPackagesWindowData : ScriptableObject
    {
        [SerializeField]
        public PackagePresetsDatabase presetsDatabase;

        [SerializeField]
        public PackMode packMode;

        [SerializeField]
        public string destinationFolder;

        [SerializeField]
        public ExportPackageOptions exportUnityPackageOptions;

        [SerializeField]
        public string totalPackageName = "TotalPackage from project: " + 
            Helper.GetProjectName();
    }
}
