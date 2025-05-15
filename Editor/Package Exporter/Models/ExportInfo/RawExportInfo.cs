using System;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class RawExportInfo : ExportInfo
    {
        public override string GetTargetPath() => directoryPath;
        public override Type GetRequiredProcessorType() => typeof(RawExportProcessor);
    }
}
