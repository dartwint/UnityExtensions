using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [System.Serializable]
    public class PackagePresetsDatabase : ScriptableObject
    {
        public List<PackagePresetNEW> presets = new();

        public static PackagePresetsDatabase CreateDatabase(string folderPath = "Assets/PackageExporter")
        {
            var database = CreateInstance<PackagePresetsDatabase>();
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
