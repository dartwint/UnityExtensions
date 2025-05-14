using System;
using System.Linq;
using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class UnityPackageExportProcessor : IUnityPackageExportProcessor
    {
        public bool Export(PackageInfo packageInfo, UnityPackageExportInfo exportInfo)
        {
            try
            {
                AssetDatabase.ExportPackage(
                    packageInfo.files.ToArray(),
                    exportInfo.GetTargetPath(packageInfo.fileName),
                    exportInfo.exportPackageOptions);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
