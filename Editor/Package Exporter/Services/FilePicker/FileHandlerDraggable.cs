using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public class FileHandlerDraggable : IFileHandler<IEnumerable<UnityEngine.Object>>
    {
        public ScriptableObject Context { get; private set; }

        public static readonly Type[] allowedTypes =
        {
            typeof(UnityEditor.Editor),
            typeof(EditorWindow),
        };

        public FileHandlerDraggable(ScriptableObject context)
        {
            if (context == null || !allowedTypes.Where(t => t.IsAssignableFrom(context.GetType())).Any())
            {
                throw new ArgumentException($"Passed {typeof(ScriptableObject)} {context} " +
                    $"has incorrect type: {context.GetType()}");
            }

            Context = context;
        }

        private void ValidateContextActive()
        {
            if (Context is EditorWindow windowContext)
            {
                if (EditorWindow.focusedWindow != windowContext)
                    throw new InvalidOperationException("EditorWindow context is not currently focused.");
            }
            else if (Context is UnityEditor.Editor editorContext)
            {
                var activeEditors = ActiveEditorTracker.sharedTracker.activeEditors;
                bool isActive = activeEditors.Any(e => e == editorContext);

                if (!isActive)
                    throw new InvalidOperationException("Editor context is not active in current inspector.");
            }
            else
            {
                throw new InvalidOperationException($"Unsupported context type: {Context.GetType()}");
            }
        }

        private IEnumerable<string> ConvertToAssetPaths(IEnumerable<UnityEngine.Object> droppedObjects)
        {
            foreach (var droppedObject in droppedObjects)
            {
                string path = AssetDatabase.GetAssetPath(droppedObject);
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException($"Provided Unity Object " +
                        $"{droppedObject} has no path to itself");
                }

                yield return path;
            }

            yield break;
        }

        public IEnumerable<string> PickFiles(IEnumerable<UnityEngine.Object> objects)
        {
            ValidateContextActive();

            return ConvertToAssetPaths(objects);
        }
    }
}
