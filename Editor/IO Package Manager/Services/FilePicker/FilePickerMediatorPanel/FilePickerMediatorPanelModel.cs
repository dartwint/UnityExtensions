using System.Collections.Generic;
using System.Linq;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class FilePickerMediatorPanelModel
    {
        private HashSet<string> _selectedFiles = new();
        private HashSet<string> _extensions = new();

        public FilePickerMediatorPanelModel() { }

        public FilePickerMediatorPanelModel(IEnumerable<string> extensions)
        {
            _extensions = extensions.ToHashSet();
        }

        //public string[] SelectedFiles
        //{
        //    get => _selectedFiles.ToArray();
        //    set
        //    {
        //        _selectedFiles.Clear();
        //        _selectedFiles = value.ToHashSet();
        //    }
        //}

        public string[] GetSelectedFiles() => _selectedFiles.ToArray();
        public string[] GetExtensions() => _extensions.ToArray();

        public void AddFile(string file) => _selectedFiles.Add(file);

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                _selectedFiles.Add(file);
            }
        }

        public void RemoveFile(string file) => _selectedFiles.Remove(file);

        public void RemoveFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                _selectedFiles.Remove(file);
            }
        }
    }
}
