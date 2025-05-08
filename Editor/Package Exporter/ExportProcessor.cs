using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class PackageInfo
    {
        public string displayName;
        public string fileName;
        public HashSet<string> files;
    }

    public class UnityPackageExportInfo
    {
        public string targetDirectory;
        public ExportPackageOptions exportPackageOptions;

        public string GetTargetPath(string fileName) =>
           Path.Combine(targetDirectory, $"{fileName}.unitypackage");
    }

    public interface IRawExportProcessor
    {
        bool Export(PackageInfo packageInfo, string targetDirectory);
    }

    public class RawExportProcessor : IRawExportProcessor
    {
        public bool Export(PackageInfo packageInfo, string targetDirectory)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                    return false;

                foreach (string file in packageInfo.files)
                {
                    File.Copy(file, Path.Combine(targetDirectory, Path.GetFileName(file)), true);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public interface IUnityPackageExportProcessor
    {
        bool Export(PackageInfo packageInfo, UnityPackageExportInfo exportInfo);
    }

    public class UnityPackageExportProcessor : IUnityPackageExportProcessor
    {
        public bool Export(PackageInfo packageInfo, UnityPackageExportInfo exportInfo)
        {
            try
            {
                AssetDatabase.ExportPackage(
                    packageInfo.files.ToArray(),
                    exportInfo.GetTargetPath(packageInfo.fileName),
                    exportInfo.exportPackageOptions);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class ExportProcessor
    {
        private IRawExportProcessor _rawExportProcessor;
        private IUnityPackageExportProcessor _unityExportProcessor;

        public void SetRawExportProcessor(IRawExportProcessor exportProcessor)
        {
            _rawExportProcessor = exportProcessor;
        }

        public void SetUnityPackageExportProcessor(IUnityPackageExportProcessor exportProcessor)
        {
            _unityExportProcessor = exportProcessor;
        }

        public bool ExportRaw(PackageInfo packageInfo, string targetDirectory)
        {
            if (_rawExportProcessor == null)
                return false;

            return _rawExportProcessor.Export(packageInfo, targetDirectory);
        }

        public bool ExportUnityPackage(PackageInfo packageInfo, UnityPackageExportInfo exportInfo)
        {
            if (_unityExportProcessor == null)
                return false;

            return _unityExportProcessor.Export(packageInfo, exportInfo);
        }
    }
}
