using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        // FIX ME: unnable to remove selected files by clicking on "X" 
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

            viewModel.selectionSize = (SelectionSize) EditorGUILayout.EnumPopup(nameof(SelectionSize), viewModel.selectionSize);
            if (viewModel.selectionSize != SelectionSize.Single)
            {
                viewModel.searchOption = (SearchOption) EditorGUILayout.EnumPopup(nameof(SearchOption), viewModel.searchOption);
            }

            if (GUILayout.Button("Open selector"))
            {
                HandleSelector();
                if (viewModel.selectionResult == SelectiorDialogResult.OutOfProjectFolder)
                {
                    DrawOutOfProjectWarning();
                }
            }

            GUILayout.EndVertical();
        }

        private void HandleSelector()
        {
            string folderPath = "";

            if (viewModel.selectionSize == SelectionSize.Multiple)
            {
                folderPath = EditorUtility.OpenFolderPanel(GetSelectorPanelTitle(), Application.dataPath, "");
            }
            else if (viewModel.selectionSize == SelectionSize.Single)
            {
                folderPath = EditorUtility.OpenFilePanelWithFilters(
                    GetSelectorPanelTitle(), Application.dataPath, new string[] { "All files", "*" });
            }

            viewModel.OnSelectorClosed(folderPath);
        }

        private string GetSelectorPanelTitle()
        {
            string title = "";

            if (viewModel.selectionSize == SelectionSize.Multiple)
                title += "Multiple ";
            else if (viewModel.selectionSize == SelectionSize.Single)
                title += "Single ";

            title += "selector";

            return title;
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
