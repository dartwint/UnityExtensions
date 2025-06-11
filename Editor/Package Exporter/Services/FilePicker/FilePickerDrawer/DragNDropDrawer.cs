using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class DragNDropDrawer : IFilePickerDrawer
    {
        public Color backgroundColor = Color.green;

        private UnityObjectPickerFromSourceDraggable _filePicker;
        private ScriptableObject _context;

        public DragNDropDrawer(UnityObjectPickerFromSourceDraggable filePicker, ScriptableObject context)
        {
            if (filePicker == null)
            {
                throw new ArgumentNullException($"Provided {nameof(filePicker)} was null");
            }

            _filePicker = filePicker;
            _context = context;
        }

        public void Draw(SerializedObject serializedObject)
        {
            if (_filePicker == null)
            {
                _filePicker = new UnityObjectPickerFromSourceDraggable(_context);
            }

            GUILayout.BeginHorizontal();
            if (serializedObject.targetObject is PackagePresetNEW packagePreset)
            {
                var droppedObjects = DrawGUIAndGetDroppedObjects().ToList();
                packagePreset.packageInfo.AddFiles(_filePicker.PickFiles(droppedObjects));
                if (droppedObjects != null && droppedObjects.Count > 0)
                {
                    packagePreset.Save();
                }
            }
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
    }
}
