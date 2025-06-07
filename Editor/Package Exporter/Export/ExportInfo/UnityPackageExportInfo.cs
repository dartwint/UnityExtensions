using System;
using System.IO;
using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class UnityPackageExportInfo : ExportInfo
    {
        public string fileName = "UnityPackage";
        public ExportPackageOptions exportPackageOptions;

        public override string GetTargetPath() =>
           Path.Combine(directoryPath, $"{fileName}.unitypackage");

        public override Type GetRequiredProcessorType() => typeof(UnityPackageExportProcessor);
    }
}
