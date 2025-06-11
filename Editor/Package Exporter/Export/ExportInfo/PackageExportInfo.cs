using System;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public abstract class PackageExportInfo
    {
        public string targetDirectory;

        public abstract string GetTargetPath();
        public abstract Type GetRequiredProcessorType();
    }
}
