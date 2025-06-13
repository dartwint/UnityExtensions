using System;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportProcessorTypeMismatchException : Exception
    {
        public Type exportInfoType;
        public Type exportProcessorType;

        public PackagePreset packagePreset;

        public ExportProcessorTypeMismatchException(Type exportInfoType, Type exportProcessorType)
        {
            this.exportInfoType = exportInfoType;
            this.exportProcessorType = exportProcessorType;
        }

        public ExportProcessorTypeMismatchException(Type exportInfoType, Type exportProcessorType, PackagePreset packagePreset)
        {
            this.exportInfoType = exportInfoType;
            this.exportProcessorType = exportProcessorType;
            this.packagePreset = packagePreset;
        }

        public override string ToString()
        {
            if (packagePreset == null)
                return string.Concat($"Attempt to use {exportProcessorType} by {exportInfoType}");

            return string.Concat($"Attempt to use {exportProcessorType} by {exportInfoType} from package preset: {packagePreset.name}");
        }
    }
}
