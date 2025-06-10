using System.Collections.Generic;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    /// <summary>
    /// For internal file sources
    /// </summary>
    public interface IFilePicker
    {
        public IEnumerable<string> PickFiles();
    }
}
