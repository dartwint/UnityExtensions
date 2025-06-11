using System;
using System.IO;
using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class UnityPackageExportInfo : PackageExportInfo
    {
        public string fileName = "UnityPackage";
        public ExportPackageOptions exportPackageOptions;

        public override string GetTargetPath() => 
            Path.Combine(targetDirectory, $"{fileName}.unitypackage");

        public override Type GetRequiredProcessorType() => typeof(UnityPackageExportProcessor);
    }
}
