using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class FilePickerWindow : EditorWindow
    {
        public FilePickerViewModel viewModel;

        private Vector2 _controlsScrollPosition;
        private Vector2 _filesListingScrollPosition;

        private void OnGUI()
        {
            _controlsScrollPosition = GUILayout.BeginScrollView(_controlsScrollPosition);
            DrawSelectionControls();

            DrawSelectedFiles();

            GUILayout.Space(15);

            if (GUILayout.Button("Save and close"))
            {
                viewModel.SaveChanges();
                Close();
            }

            GUILayout.EndScrollView();
        }

        private void OnDestroy()
        {
            viewModel.CloseView(this);
        }

        private void DrawSelectedFiles()
        {
            string title = $"Selected files";

            string[] files = viewModel.model.GetSelectedFiles();
            int filesCount = files.Length;

            if (filesCount > 0)
            {
                title += $" (count: {filesCount})";
                GUILayout.Label(title, EditorStyles.boldLabel);
                _filesListingScrollPosition = EditorGUILayout.BeginScrollView(_filesListingScrollPosition, GUILayout.Height(200));

                for (int i = 0; i < filesCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    files[i] = EditorGUILayout.TextField(files[i]);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        viewModel.RemoveFile(files[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label(title, EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.Label("No items selected at the moment", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
            }
        }

        private void DrawSelectionControls()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            viewModel.selectionMode = (SelectionMode) EditorGUILayout.EnumPopup(nameof(SelectionMode), viewModel.selectionMode);
            if (viewModel.selectionMode != SelectionMode.Single)
            {
                viewModel.searchOption = (SearchOption) EditorGUILayout.EnumPopup(nameof(SearchOption), viewModel.searchOption);
            }

            if (GUILayout.Button("Open selector"))
            {
                viewModel.HandleSelector();
                if (viewModel.selectionResult == SelectiorDialogResult.OutOfProjectFolder)
                {
                    DrawOutOfProjectWarning();
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawOutOfProjectWarning()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon.sml"), GUILayout.Width(20));
            GUILayout.Label("Selected item was out of Unity project directory. Selection was cancelled.");
            GUILayout.EndHorizontal();
        }
    }
}
