using System;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class UnityPackageExportProcessor : IExportProcessor<UnityPackageExportOptions>
    {
        public bool CanProccess(Package package)
        {
            if (package.exportOptions.GetRequiredProcessorType() != GetType())
                return false;

            return true;
        }

        public bool Export(Package package)
        {
            try
            {
                if (package.exportOptions is not UnityPackageExportOptions info)
                    throw new ExportProcessorTypeMismatchException(package.exportOptions.GetType(), GetType(), package);

                AssetDatabase.ExportPackage(
                    package.packageFiles.GetFiles(),
                    package.exportOptions.GetTargetPath(),
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
