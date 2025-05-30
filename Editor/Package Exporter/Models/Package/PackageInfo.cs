using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class PackageInfo
    {
        [SerializeField, HideInInspector]
        private List<string> _storedFiles = new();

        private HashSet<string> _files;
        private HashSet<string> Files
        {
            get
            {
                if (_files == null)
                    _files = _storedFiles.ToHashSet();

                return _files;
            }
            set
            {
                _files = value;
            }
        }

        public string[] GetFiles() => Files.ToArray();

        public void SaveFiles()
        {
            _storedFiles.Clear();
            _storedFiles = _files.ToList();
        }

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Files.Add(file);
            }
        }

        public void RemoveFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Files.Remove(file);
            }
        }
    }
}