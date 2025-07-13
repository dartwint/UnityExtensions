using System;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    [System.Serializable]
    public abstract class PackageExportOptions
    {
        public string targetDirectory;

        public abstract string GetTargetPath();
        public abstract Type GetRequiredProcessorType();
    }
}
