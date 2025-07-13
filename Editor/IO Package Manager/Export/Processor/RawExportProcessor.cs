using System;
using System.IO;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class RawExportProcessor : IExportProcessor<RawPackageExportOptions>
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
                if (package.exportOptions is not RawPackageExportOptions options)
                    throw new ExportProcessorTypeMismatchException(package.exportOptions.GetType(), GetType(), package);

                if (!Directory.Exists(options.targetDirectory))
                    return false;

                string[] files = package.packageFiles.GetFiles();
                foreach (string file in files)
                {
                    if (Directory.Exists(file))
                    {
                        Directory.CreateDirectory(Path.Combine(options.GetTargetPath(), file));
                    }
                    else if (File.Exists(file))
                    {
                        File.Copy(file, Path.Combine(options.GetTargetPath(), Path.GetFileName(file)), true);
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
