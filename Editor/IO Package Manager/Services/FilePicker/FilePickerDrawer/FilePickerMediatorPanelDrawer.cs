using System;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class FilePickerMediatorPanelDrawer : IFilePickerDrawer
    {
        private FilePickerMediatorPanelViewModel _filePickerViewModel;

        public FilePickerMediatorPanelDrawer() { }
        public FilePickerMediatorPanelDrawer(FilePickerMediatorPanelViewModel filePickerViewModel)
        {
            if (filePickerViewModel == null)
            {
                throw new ArgumentNullException($"Provided {nameof(filePickerViewModel)} was null");
            }

            _filePickerViewModel = filePickerViewModel;
        }

        public void Draw(SerializedObject serializedObject)
        {
            if (_filePickerViewModel == null && serializedObject.targetObject is Package target)
            {
                _filePickerViewModel = new FilePickerMediatorPanelViewModel(
                    new FilePickerMediatorPanelModel(), target);
                _filePickerViewModel.ViewClosed += OnViewClosed;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Open items selector modes", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add items"))
            {
                _filePickerViewModel.OpenViewForAdditionButtonClick();
            }

            if (GUILayout.Button("Remove items"))
            {
                _filePickerViewModel.OpenViewForRemovalButtonClick();
            }

            GUILayout.EndHorizontal();
        }

        private void OnViewClosed()
        {
            _filePickerViewModel.ViewClosed -= OnViewClosed;
            _filePickerViewModel = null;
        }
    }
}
