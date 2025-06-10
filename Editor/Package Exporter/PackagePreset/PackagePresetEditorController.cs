using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    /// <summary>
    /// The view and controller operating with <see cref="PackagePresetNEW"></see>
    /// </summary>
    [CustomEditor(typeof(PackagePresetNEW))]
    public class PackagePresetEditorController : UnityEditor.Editor
    {
        private PackagePresetNEW _target;
        //private FilePickerMediatorPanelViewModel _filePickerViewModel;

        private FileHandlerDraggable _fileHandler;

        public Color backgroundColor = Color.green;

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            _target = (PackagePresetNEW) target;
            if (_target == null)
                return;

            //if (_filePickerViewModel == null)
            //{
            //    _filePickerViewModel = new FilePickerMediatorPanelViewModel(
            //        new FilePickerMediatorPanelModel(), _target);
            //    _filePickerViewModel.ViewClosed += OnViewClosed;
            //}

            if (_fileHandler == null)
            {
                _fileHandler = new FileHandlerDraggable(this);
            }

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(nameof(_target.exportInfo)), true);

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            DrawFilePickerControls();

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            DrawPackageFiles();
        }

        //private void OnViewClosed()
        //{
        //    _filePickerViewModel.ViewClosed -= OnViewClosed;
        //    _filePickerViewModel = null;
        //    Repaint();
        //}

        private void DrawFilePickerControls()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Open items selector modes", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Add items"))
            //{
            //    _filePickerViewModel.OpenViewForAdditionButtonClick();
            //}

            //if (GUILayout.Button("Remove items"))
            //{
            //    _filePickerViewModel.OpenViewForRemovalButtonClick();
            //}

            _fileHandler.PickFiles(DrawGUIAndGetDroppedObjects().ToList());

            GUILayout.EndHorizontal();
        }

        private IEnumerable<UnityEngine.Object> DrawGUIAndGetDroppedObjects()
        {
            Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));

            Event currentEvent = Event.current;

            Color originalColor = GUI.backgroundColor;

            GUI.backgroundColor = backgroundColor;
            GUI.Box(dropArea, "Drag assets here");
            GUI.backgroundColor = originalColor;

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    {
                        if (dropArea.Contains(currentEvent.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (currentEvent.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();

                                foreach (var draggedObject in DragAndDrop.objectReferences)
                                {
                                    yield return draggedObject;
                                }
                            }

                            currentEvent.Use();
                        }

                        break;
                    }
            }

            yield break;
        }

        private Vector2 _packageFilesScrollPosition;

        private void DrawPackageFiles()
        {
            string title = $"Package files";

            List<string> files = _target.packageInfo.GetFilesSorted().ToList();
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

                    _target.packageInfo.RemoveFile(file);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
