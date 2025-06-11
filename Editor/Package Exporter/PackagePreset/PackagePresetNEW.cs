using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class PackagePresetNEW : ScriptableObject
    {
        public PackageFiles packageInfo = new();

        [SerializeReference]
        public PackageExportInfo exportInfo;

        public void Save()
        {
            packageInfo.SaveFiles();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        [MenuItem("Tools/Package Exporter/Create package preset/Unity package preset")]
        public static void CreateAssetAsUnityPackagePreset()
        {
            CreateAsset<UnityPackageExportInfo>();
        }

        public static void CreateAsset<TPackageExportInfo>() where TPackageExportInfo : PackageExportInfo, new()
        {
            var presetAsset = CreateInstance<PackagePresetNEW>();
            presetAsset.exportInfo = new TPackageExportInfo();
            string baseName = $"New {typeof(TPackageExportInfo).Name.Replace("ExportInfo", " Preset")}";

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
