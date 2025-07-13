using System;
using System.IO;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    [System.Serializable]
    public class RawPackageExportOptions : PackageExportOptions
    {
        public string directoryName = "RawFilesPackage";
        public override string GetTargetPath() =>
            Path.Combine(targetDirectory, directoryName);

        public override Type GetRequiredProcessorType() => 
            typeof(RawExportProcessor);
    }
}
