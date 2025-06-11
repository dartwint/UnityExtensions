using System;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class RawExportInfo : PackageExportInfo
    {
        public override string GetTargetPath() => targetDirectory;

        public override Type GetRequiredProcessorType() => typeof(RawExportProcessor);
    }
}
