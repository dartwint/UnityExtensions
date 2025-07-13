namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class ExportManager
    {
        private readonly IExportProcessorFactory _factory;

        public ExportManager(IExportProcessorFactory factory)
        {
            _factory = factory;
        }

        public bool Export(Package package)
        {
            var processor = _factory.GetProcessor(package.exportOptions);
            if (!processor.CanProccess(package))
                return false;

            return processor.Export(package);
        }
    }
}
