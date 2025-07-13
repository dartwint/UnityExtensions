using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class PackageManager
    {
        public static void Save(Package package)
        {
            package.packageFiles.Save();

            EditorUtility.SetDirty(package);
            AssetDatabase.SaveAssetIfDirty(package);
        }

        [MenuItem("Tools/Package Exporter/Create package preset/Unity package")]
        public static void CreateAssetAsUnityPackagePreset()
        {
            CreateAsset<UnityPackageExportOptions>();
        }

        [MenuItem("Tools/Package Exporter/Create package preset/Raw files package")]
        public static void CreateAssetAsRawFilesPreset()
        {
            CreateAsset<RawPackageExportOptions>();
        }

        private static void CreateAsset<TPackageExportOptions>() where TPackageExportOptions : PackageExportOptions, new()
        {
            var presetAsset = ScriptableObject.CreateInstance<Package>();
            presetAsset.exportOptions = new TPackageExportOptions();
            string baseName = $"New {typeof(TPackageExportOptions).Name.Replace("ExportOptions", " Package")}";

            string path = $"Assets/{baseName}.asset";
            int counter = 0;
            while (AssetDatabase.LoadAssetAtPath<Package>(path) != null)
            {
                counter++;
                presetAsset.name = $"{baseName}{counter}";
                path = $"Assets/{presetAsset.name}.asset";
            }

            AssetDatabase.CreateAsset(presetAsset, path);
            AssetDatabase.SaveAssetIfDirty(presetAsset);
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = presetAsset;
        }
    }
}
