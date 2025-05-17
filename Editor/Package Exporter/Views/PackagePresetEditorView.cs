using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [CustomEditor(typeof(PackagePresetNEW))]
    public class PackagePresetEditorView : UnityEditor.Editor
    {
        private PackagePresetNEW _target;
        private FilePickerViewModel _filePickerViewModel;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _target = (PackagePresetNEW) target;
            if (_target == null)
                return;

            if (_filePickerViewModel == null)
            {
                _filePickerViewModel = new FilePickerViewModel(new FilePickerModel(), _target);
                _filePickerViewModel.ViewClosed += OnViewClosed;
            }

            DrawPackageFiles();

            DrawFilePickerControls();
        }

        private void OnViewClosed()
        {
            _filePickerViewModel.ViewClosed -= OnViewClosed;
            Repaint();
        }

        private void DrawFilePickerControls()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Open items selector modes", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add items"))
            {
                _filePickerViewModel.AddItemsButtonClick();
            }

            if (GUILayout.Button("Remove items"))
            {
                _filePickerViewModel.RemoveItemsButtonClick();
            }
            GUILayout.EndHorizontal();
        }

        private Vector2 _packageFilesScrollPosition;

        private void DrawPackageFiles()
        {
            string title = $"Package files";

            string[] files = _target.packageInfo.GetFiles();
            int filesCount = files.Length;

            if (filesCount > 0)
            {
                title += $" (count: {filesCount})";
                GUILayout.Label(title, EditorStyles.boldLabel);
                _packageFilesScrollPosition = EditorGUILayout.BeginScrollView(_packageFilesScrollPosition, GUILayout.Height(200));

                for (int i = 0; i < filesCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label(files[i], EditorStyles.textField);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label(title, EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.Label("No files in package", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
            }
        }
    }
}
