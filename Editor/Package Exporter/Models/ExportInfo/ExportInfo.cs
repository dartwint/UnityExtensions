using System;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public abstract class ExportInfo
    {
        public string directoryPath;
        public abstract string GetTargetPath();
        public abstract Type GetRequiredProcessorType();
    }
}
