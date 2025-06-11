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

                if (!Directory.Exists(info.targetDirectory))
                    return false;

                string[] files = packagePreset.packageInfo.GetFiles();
                foreach (string file in files)
                {
                    File.Copy(file, Path.Combine(info.targetDirectory, Path.GetFileName(file)), true);
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
