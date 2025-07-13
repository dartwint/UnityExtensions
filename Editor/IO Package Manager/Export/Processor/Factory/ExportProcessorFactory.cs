using System;
using System.Collections.Generic;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class ExportProcessorFactory : IExportProcessorFactory
    {
        private Dictionary<Type, IExportProcessor> _processors = new();

        public IExportProcessor GetProcessor(PackageExportOptions exportOptions)
        {
            var type = exportOptions.GetType();

            if (!_processors.ContainsKey(type))
            {
                _processors.Add(type, CreateProcessor(exportOptions));
            }

            return _processors[type];
        }

        private IExportProcessor CreateProcessor(PackageExportOptions exportOptions)
        {
            var type = exportOptions.GetRequiredProcessorType();
            return Activator.CreateInstance(type) as IExportProcessor;
        }
    }
}
