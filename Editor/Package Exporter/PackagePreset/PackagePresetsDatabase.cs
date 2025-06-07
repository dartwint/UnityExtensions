using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public enum PackMode
    {
        Batch,
        Standalone
    }

    [System.Serializable]
    public class PackagePresetsDatabase : ScriptableObject
    {
        [field: SerializeField]
        public List<PackagePresetNEW> Presets { get; private set; } = new();

        public int GetPresetsCount() => Presets.Count;

        [SerializeField]
        public PackMode packMode = PackMode.Standalone;

        [SerializeField]
        public string totalPackageName = "NewTotalPackage from " + Helper.GetProjectName();

        public const string folderPath = "Assets/PackageExporter";
        public static readonly string defaultAssetPath = string.Concat(folderPath, "/", "PackagePresetsDb.asset");

        public static PackagePresetsDatabase GetOrCreate()
        {
            var database = AssetDatabase.LoadAssetAtPath<PackagePresetsDatabase>(defaultAssetPath);
            if (database != null)
                return database;

            database = CreateInstance<PackagePresetsDatabase>();
            if (database == null)
                throw new ArgumentNullException();

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            AssetDatabase.CreateAsset(database, defaultAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return database;
        }

        public void AddPreset(PackagePresetNEW packagePreset)
        {
            if (!Presets.Contains(packagePreset))
            {
                Presets.Add(packagePreset);
            }
        }

        public void RemovePreset(PackagePresetNEW packagePreset)
        {
            Presets.Remove(packagePreset);
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
}
