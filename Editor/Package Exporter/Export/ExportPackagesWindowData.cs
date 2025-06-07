using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportPackagesWindowData : ScriptableObject
    {
        [SerializeField]
        public PackagePresetsDatabase presetsDatabase;

        [SerializeField]
        public string destinationFolder;

        #region For override DB fields

        [SerializeField]
        public bool overrideExportOptions = false;

        [SerializeField]
        public PackMode packMode;

        [SerializeField]
        public string totalPackageName = "NewTotalPackage from " + Helper.GetProjectName();

        #endregion
    }
}
