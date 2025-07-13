using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    [System.Serializable]
    public class PackageFiles
    {
        public string[] GetFiles() => Files.ToArray();
        public string[] GetFilesSorted() => Files.OrderBy(f => f).ToArray();

        public void Save()
        {
            _serializedFiles.Clear();

            if (_files != null)
            {
                _serializedFiles = _files.ToList();
            }
        }

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Files.Add(file);
            }
        }

        public void AddFile(string file)
        {
            Files.Add(file);
        }

        public void RemoveFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Files.Remove(file);
            }
        }

        public void RemoveFile(string file)
        {
            Files.Remove(file);
        }

        [SerializeField, HideInInspector]
        private List<string> _serializedFiles = new();

        private HashSet<string> _files;

        private HashSet<string> Files
        {
            get
            {
                if (_files == null)
                    _files = _serializedFiles.ToHashSet();

                return _files;
            }
            set
            {
                _files = value;
            }
        }
    }
}