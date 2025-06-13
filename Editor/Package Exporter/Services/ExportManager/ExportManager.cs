namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportManager
    {
        private readonly IExportProcessorFactory _exportProcessorFactory;

        public ExportManager(IExportProcessorFactory exportProcessorFactory)
        {
            _exportProcessorFactory = exportProcessorFactory;
        }

        public bool Export(PackagePreset packagePreset)
        {
            var processor = _exportProcessorFactory.GetProcessor(packagePreset.exportInfo);
            if (!processor.CanProccess(packagePreset))
                return false;

            return processor.Export(packagePreset);
        }
    }
}
