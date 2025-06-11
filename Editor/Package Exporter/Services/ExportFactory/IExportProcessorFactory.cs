using System;
using System.Collections.Generic;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public interface IExportProcessorFactory
    {
        IExportProcessor GetProcessor(PackageExportInfo exportInfo);
    }

    public class ExportFactory : IExportProcessorFactory
    {
        private readonly Dictionary<Type, IExportProcessor> _processors = new();

        public IExportProcessor GetProcessor(PackageExportInfo exportInfo)
        {
            var type = exportInfo.GetType();

            if (!_processors.ContainsKey(type))
            {
                _processors.Add(type, CreateProcessor(exportInfo));
            }

            return _processors[type];
        }

        private IExportProcessor CreateProcessor(PackageExportInfo exportInfo)
        {
            var type = exportInfo.GetRequiredProcessorType();
            return Activator.CreateInstance(type) as IExportProcessor;
        }
    }
}
