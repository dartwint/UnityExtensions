using System;
using System.ComponentModel;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportPresetsViewModel : INotifyPropertyChanged
    {
        [SerializeField]
        private PackagePresetsDatabase _presetsDatabase;

        private PackagePresetNEW _selectedPreset;

        public event PropertyChangedEventHandler PropertyChanged;

        public PackagePresetsDatabase Database
        {
            get => _presetsDatabase;
        }

        public PackagePresetNEW SelectedPreset
        {
            get => _selectedPreset;
            private set
            {
                _selectedPreset = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPreset)));
            }
        }

        public void SelectPreset(PackagePresetNEW packagePreset)
        {
            SelectedPreset = packagePreset;
        }

        public ExportPresetsViewModel(PackagePresetsDatabase presetsDatabase)
        {
            _presetsDatabase = presetsDatabase ?? 
                throw new ArgumentNullException(nameof(presetsDatabase));
        }
    }
}
