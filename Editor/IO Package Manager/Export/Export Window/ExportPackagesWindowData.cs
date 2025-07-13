using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class ExportPackagesWindowData : ScriptableObject
    {
        [SerializeField]
        public PackagesDatabase presetsDatabase;

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
