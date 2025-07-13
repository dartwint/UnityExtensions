using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class Package : ScriptableObject
    {
        public PackageFiles packageFiles = new();

        [SerializeReference]
        public PackageExportOptions exportOptions;
    }
}
