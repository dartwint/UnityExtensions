using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class FilePickerMediatorPanelView : EditorWindow
    {
        public FilePickerMediatorPanelViewModel viewModel;

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
                        viewModel.RemoveFileButtonClick(files[i]);
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

            //GUI.enabled = false;
            viewModel.SelectorOptions.selectionItemType = (SelectionItemType)
                EditorGUILayout.EnumPopup("File type", viewModel.SelectorOptions.selectionItemType);
            //if (viewModel.SelectorOptions.selectionItemType == SelectionItemType.Directory)
            //{
            //    GUI.enabled = false;
            //    viewModel.SelectorOptions.selectionMode = SelectionMode.Single;
            //    viewModel.SelectorOptions.searchOption = SearchOption.TopDirectoryOnly;
            //}
            //GUI.enabled = true;

            viewModel.SelectorOptions.selectionMode = (SelectionMode)
                EditorGUILayout.EnumPopup(nameof(SelectionMode), viewModel.SelectorOptions.selectionMode);
            if (viewModel.SelectorOptions.selectionItemType == SelectionItemType.Directory)
            {
                viewModel.SelectorOptions.searchOption = (SearchOption)
                    EditorGUILayout.EnumPopup(nameof(SearchOption), viewModel.SelectorOptions.searchOption);
            }

            if (GUILayout.Button("Open selector"))
            {
                viewModel.HandleSelectorDialog();
            }
            if (viewModel.selectorResult == SelectorDialogResult.OutOfProjectFolder)
            {
                DrawOutOfProjectWarning();
            }

            GUILayout.EndVertical();
        }

        private void DrawOutOfProjectWarning()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon.sml"), GUILayout.Width(20));
            GUILayout.Label("Last selection is out of Unity project directory. Selection cancelled.");
            GUILayout.EndHorizontal();
        }
    }
}
