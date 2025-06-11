using System;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class PackageExportInfoDrawerStd : IPackageExportInfoDrawer
    {
        //private GUIStyle _centeredStyle = new GUIStyle(EditorStyles.label)
        //{
        //    alignment = TextAnchor.MiddleCenter
        //};

        public void Draw(SerializedObject serializedObject)
        {
            var packagePreset = (PackagePresetNEW) serializedObject.targetObject;
            if (packagePreset == null)
            {
                throw new ArgumentNullException($"Provided {nameof(serializedObject.targetObject)} " +
                    $"is not a {typeof(PackagePresetNEW)} type");
            }

            var exportInfo = packagePreset.exportInfo;

            //if (exportInfo.alwaysExportStandalone = GUILayout.Toggle(
            //    exportInfo.alwaysExportStandalone, "Always export as standalone package"))
            //{
            //    GUILayout.BeginHorizontal();
            //    GUILayout.FlexibleSpace();
            //    GUILayout.Label("Output Directory", _centeredStyle, GUILayout.ExpandWidth(true));
            //    GUILayout.FlexibleSpace();
            //    GUILayout.EndHorizontal();

            //    GUILayout.BeginHorizontal();
            //    if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
            //    {
            //        string directory = EditorUtility.OpenFolderPanel($"Select directory for " +
            //            $"{nameof(PackageExportInfo)}", "", "");
            //        if (!string.IsNullOrEmpty(directory))
            //        {
            //            exportInfo.directoryPath = directory;
            //        }
            //    }
            //    //exportInfo.directoryPath = GUILayout.TextField(exportInfo.directoryPath);
            //    EditorGUILayout.LabelField(exportInfo.directoryPath);
            //    GUILayout.EndHorizontal();
            //}

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);

            if (exportInfo is UnityPackageExportInfo unityPackageExportInfo)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("File name", GUILayout.ExpandWidth(false));
                unityPackageExportInfo.fileName = GUILayout.TextField(unityPackageExportInfo.fileName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Export options", GUILayout.ExpandWidth(false));
                unityPackageExportInfo.exportPackageOptions = (ExportPackageOptions) 
                    EditorGUILayout.EnumFlagsField(unityPackageExportInfo.exportPackageOptions);
                GUILayout.EndHorizontal();
            }

            if (exportInfo is RawExportInfo rawExportInfo)
            {
                //
            }
        }
    }
}
