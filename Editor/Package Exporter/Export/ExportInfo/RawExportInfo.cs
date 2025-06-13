using System;
using System.IO;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class RawExportInfo : PackageExportInfo
    {
        public string directoryName = "RawFilesPackage";
        public override string GetTargetPath() =>
            Path.Combine(targetDirectory, directoryName);

        public override Type GetRequiredProcessorType() => typeof(RawExportProcessor);
    }
}
