using System;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class UnityPackageExportProcessor : IExportProcessor
    {
        public bool CanProccess(PackagePreset packagePreset)
        {
            if (packagePreset.exportInfo.GetRequiredProcessorType() != GetType())
                return false;

            return true;
        }

        public bool Export(PackagePreset packagePreset)
        {
            try
            {
                if (packagePreset.exportInfo is not UnityPackageExportInfo info)
                    throw new ExportProcessorTypeMismatchException(packagePreset.exportInfo.GetType(), GetType(), packagePreset);

                AssetDatabase.ExportPackage(
                    packagePreset.packageInfo.GetFiles(),
                    packagePreset.exportInfo.GetTargetPath(),
                    info.exportPackageOptions);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Package export error.\n{e.Message}");

                return false;
            }
        }
    }
}
