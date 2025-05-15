using System;
using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class UnityPackageExportProcessor : IExportProcessor
    {
        public bool CanProccess(PackagePresetNEW packagePreset)
        {
            if (packagePreset.exportInfo.GetRequiredProcessorType() != GetType())
                return false;

            return true;
        }

        public bool Export(PackagePresetNEW packagePreset)
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
            catch (Exception)
            {
                return false;
            }
        }
    }
}
