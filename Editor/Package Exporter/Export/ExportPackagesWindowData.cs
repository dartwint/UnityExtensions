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
        public string totalPackageName = "TotalPackage from project: " + 
            Helper.GetProjectName();
    }
}
