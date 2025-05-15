using System;
using System.IO;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class RawExportProcessor : IExportProcessor
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
                if (packagePreset.exportInfo is not RawExportInfo info)
                    throw new ExportProcessorTypeMismatchException(packagePreset.exportInfo.GetType(), GetType(), packagePreset);

                if (!Directory.Exists(info.directoryPath))
                    return false;

                foreach (string file in packagePreset.packageInfo.files)
                {
                    File.Copy(file, Path.Combine(info.directoryPath, Path.GetFileName(file)), true);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
