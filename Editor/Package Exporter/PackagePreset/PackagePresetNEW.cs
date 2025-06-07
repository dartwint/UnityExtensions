using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class PackagePresetNEW : ScriptableObject
    {
        public PackageInfo packageInfo = new();

        [SerializeReference]
        public ExportInfo exportInfo = new UnityPackageExportInfo();

        public void Save()
        {
            packageInfo.SaveFiles();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        [MenuItem("Tools/Package Exporter/Create new package preset")]
        public static void CreateAsset()
        {
            var presetAsset = CreateInstance<PackagePresetNEW>();
            string baseName = "NewPackagePreset";

            string path = $"Assets/{baseName}.asset";
            int counter = 0;
            while (AssetDatabase.LoadAssetAtPath<PackagePresetNEW>(path) != null)
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
