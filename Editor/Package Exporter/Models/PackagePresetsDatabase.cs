using System.Collections.Generic;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class PackagePresetsDatabase : ScriptableObject
    {
        public List<PackagePresetNEW> presets = new();
    }
}
