using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    /// <summary>
    /// The view and controller operating with <see cref="Package"></see>
    /// </summary>
    [CustomEditor(typeof(Package))]
    public class PackageEditor : UnityEditor.Editor
    {
        private IPackageFilesDrawer _packageFilesDrawer;
        private IFilePickerDrawer _filePickerDrawer;
        private IPackageExportInfoDrawer _packageExportInfoDrawer;
        private Package _target;

        private static bool _exportInfoFoldout = true;

        private void OnEnable()
        {
            if (_filePickerDrawer == null)
            {
                _filePickerDrawer = new DragNDropDrawer(
                    new UnityObjectPickerFromSourceDraggable(this), this);
                //_filePickerDrawer = new FilePickerMediatorPanelDrawer();
            }

            if (_packageFilesDrawer == null)
            {
                _packageFilesDrawer = new PackageFilesDrawerDummy();
            }

            if (_packageExportInfoDrawer == null)
            {
                _packageExportInfoDrawer = new PackageExportInfoDrawerStd();
            }
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            _target = (Package) target;
            if (_target == null)
                return;

            _exportInfoFoldout = EditorGUILayout.Foldout(_exportInfoFoldout, $"{nameof(PackageExportOptions)}");
            if (_exportInfoFoldout)
            {
                _packageExportInfoDrawer.Draw(serializedObject);
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            _filePickerDrawer.Draw(serializedObject);

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            _packageFilesDrawer.Draw(_target.packageFiles);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
