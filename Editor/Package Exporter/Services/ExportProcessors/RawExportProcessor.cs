using System;
using System.IO;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class RawExportProcessor : IExportProcessor
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
                if (packagePreset.exportInfo is not RawExportInfo info)
                    throw new ExportProcessorTypeMismatchException(packagePreset.exportInfo.GetType(), GetType(), packagePreset);

                if (!Directory.Exists(info.targetDirectory))
                    return false;

                string[] files = packagePreset.packageInfo.GetFiles();
                foreach (string file in files)
                {
                    if (Directory.Exists(file))
                    {
                        Directory.CreateDirectory(Path.Combine(info.GetTargetPath(), file));
                    }
                    else if (File.Exists(file))
                    {
                        File.Copy(file, Path.Combine(info.GetTargetPath(), Path.GetFileName(file)), true);
                    }
                }

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
