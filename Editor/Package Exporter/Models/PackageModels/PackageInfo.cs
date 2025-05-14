using System.Collections.Generic;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class PackageInfo
    {
        public string displayName;
        public string fileName;
        public HashSet<string> files;
    }
}