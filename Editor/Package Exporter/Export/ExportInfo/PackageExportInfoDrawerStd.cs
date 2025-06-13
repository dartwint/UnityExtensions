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
            var packagePreset = (PackagePreset) serializedObject.targetObject;
            if (packagePreset == null)
            {
                throw new ArgumentNullException($"Provided {nameof(serializedObject.targetObject)} " +
                    $"is not a {typeof(PackagePreset)} type");
            }

            var exportInfo = packagePreset.exportInfo;

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
                GUILayout.BeginHorizontal();
                GUILayout.Label("Directory name", GUILayout.ExpandWidth(false));
                rawExportInfo.directoryName = GUILayout.TextField(rawExportInfo.directoryName);
                GUILayout.EndHorizontal();
            }
        }
    }
}
