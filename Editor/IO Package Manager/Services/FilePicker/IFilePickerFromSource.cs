using System.Collections.Generic;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    /// <summary>
    /// For external file sources
    /// </summary>
    public interface IFilePickerFromSource<in TSource>
    {
        public IEnumerable<string> PickFiles(TSource source);
    }
}
