using System;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class ExportProcessorTypeMismatchException : Exception
    {
        public Type packageExportType;
        public Type exportProcessorType;

        public Package package;

        public ExportProcessorTypeMismatchException(Type exportOptionsType, Type exportProcessorType)
        {
            this.packageExportType = exportOptionsType;
            this.exportProcessorType = exportProcessorType;
        }

        public ExportProcessorTypeMismatchException(Type exportOptionsType, Type exportProcessorType, Package package)
        {
            this.packageExportType = exportOptionsType;
            this.exportProcessorType = exportProcessorType;
            this.package = package;
        }

        public override string ToString()
        {
            if (package == null)
                return string.Concat($"Attempt to use {exportProcessorType} by {packageExportType}");

            return string.Concat($"Attempt to use {exportProcessorType} by {packageExportType} from package preset: {package.name}");
        }
    }
}
