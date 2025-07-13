using System.Collections.Generic;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    /// <summary>
    /// For internal file sources
    /// </summary>
    public interface IFilePicker
    {
        public IEnumerable<string> PickFiles();
    }
}
