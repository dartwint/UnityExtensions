using System;
using System.IO;
using UnityEditor;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    [System.Serializable]
    public class UnityPackageExportOptions : PackageExportOptions
    {
        public string fileName = "UnityPackage";
        public ExportPackageOptions exportPackageOptions;

        public override string GetTargetPath() => 
            Path.Combine(targetDirectory, $"{fileName}.unitypackage");

        public override Type GetRequiredProcessorType() => 
            typeof(UnityPackageExportProcessor);
    }
}
