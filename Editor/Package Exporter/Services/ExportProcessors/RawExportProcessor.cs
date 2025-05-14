using System;
using System.IO;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class RawExportProcessor : IRawExportProcessor
    {
        public bool Export(PackageInfo packageInfo, string targetDirectory)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                    return false;

                foreach (string file in packageInfo.files)
                {
                    File.Copy(file, Path.Combine(targetDirectory, Path.GetFileName(file)), true);
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
