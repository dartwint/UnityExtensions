using System;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class ExportPresetsViewModel
    {
        [SerializeField]
        private PackagePresetsDatabase _presetsDatabase;

        public PackagePresetsDatabase Database
        {
            get => _presetsDatabase;
        }

        public ExportPresetsViewModel(PackagePresetsDatabase presetsDatabase)
        {
            _presetsDatabase = presetsDatabase ?? 
                throw new ArgumentNullException(nameof(presetsDatabase));
        }

        private static PackagePresetsDatabase CreateDatabase(string folderPath = "Assets/PackageExporter")
        {
            var database = ScriptableObject.CreateInstance<PackagePresetsDatabase>();
            if (database == null)
                throw new ArgumentNullException();

            string path = string.Concat(folderPath, "/", "PackagePresetsDb.asset");
            AssetDatabase.CreateAsset(database, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return database;
        }
    }
}
