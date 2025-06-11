using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class PackageFilesDrawerDummy : IPackageFilesDrawer
    {
        private Vector2 _packageFilesScrollPosition;

        public void Draw(PackageFiles packageInfo)
        {
            DrawPackageFiles(packageInfo);
        }

        private void DrawPackageFiles(PackageFiles packageInfo)
        {
            string title = $"Package files";

            List<string> files = packageInfo.GetFilesSorted().ToList();
            int filesCount = files.Count;
            if (filesCount == 0)
            {
                EditorGUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(title, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("No files in package", EditorStyles.boldLabel);

                EditorGUILayout.EndVertical();

                return;
            }

            title += $" (count: {filesCount})";
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            _packageFilesScrollPosition = EditorGUILayout.BeginScrollView(
                _packageFilesScrollPosition, GUILayout.ExpandHeight(true));

            for (int i = 0; i < filesCount; i++)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.SelectableLabel(files[i], EditorStyles.boldLabel,
                    GUILayout.ExpandHeight(false),
                    GUILayout.MinHeight(14f), GUILayout.MaxHeight(20f));

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                {
                    var file = files[i];

                    files.RemoveAt(i);
                    i--;
                    filesCount--;

                    packageInfo.RemoveFile(file);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
