using System;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public interface IExportProcessorFactory
    {
        //IExportProcessor<T> GetProcessor<T>() where T : PackageExportOptions;
        IExportProcessor GetProcessor(PackageExportOptions exportOptions);
    }
}
