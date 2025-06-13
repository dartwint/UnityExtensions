namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportProcessor
    {
        private IExportProcessor _exportProcessor;

        public ExportProcessor(IExportProcessor exportProcessor)
        {
            _exportProcessor = exportProcessor;
        }

        public bool Export(PackagePreset packagePreset)
        {
            if (packagePreset == null)
                throw new System.ArgumentNullException(nameof(packagePreset));

            return _exportProcessor.Export(packagePreset);
        }
    }
}
