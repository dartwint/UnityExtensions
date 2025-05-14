namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
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
