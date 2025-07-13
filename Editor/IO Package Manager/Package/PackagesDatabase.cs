using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public enum PackMode
    {
        Batch,
        Standalone
    }

    [System.Serializable]
    public class PackagesDatabase : ScriptableObject
    {
        [field: SerializeField]
        public List<Package> Packages { get; private set; } = new();

        public int GetPresetsCount() => Packages.Count;

        [SerializeField]
        public PackMode packMode = PackMode.Standalone;

        [SerializeField]
        public string totalPackageName = "NewTotalPackage from " + Helper.GetProjectName();

        public const string folderPath = "Assets/IOPackageManager";
        public static readonly string defaultAssetPath = string.Concat(folderPath, "/", "PackagesDb.asset");

        public static PackagesDatabase GetOrCreate()
        {
            var database = AssetDatabase.LoadAssetAtPath<PackagesDatabase>(defaultAssetPath);
            if (database != null)
                return database;

            database = CreateInstance<PackagesDatabase>();
            if (database == null)
                throw new ArgumentNullException();

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            AssetDatabase.CreateAsset(database, defaultAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return database;
        }

        public void AddPackage(Package package)
        {
            if (!Packages.Contains(package))
            {
                Packages.Add(package);
            }
        }

        public void RemovePreset(Package package)
        {
            Packages.Remove(package);
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
}
