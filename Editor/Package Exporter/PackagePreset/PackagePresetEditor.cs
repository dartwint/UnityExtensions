using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    /// <summary>
    /// The view and controller operating with <see cref="PackagePreset"></see>
    /// </summary>
    [CustomEditor(typeof(PackagePreset))]
    public class PackagePresetEditor : UnityEditor.Editor
    {
        private IPackageFilesDrawer _packageFilesDrawer;
        private IFilePickerDrawer _filePickerDrawer;
        private IPackageExportInfoDrawer _packageExportInfoDrawer;
        private PackagePreset _target;

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

            _target = (PackagePreset) target;
            if (_target == null)
                return;

            _exportInfoFoldout = EditorGUILayout.Foldout(_exportInfoFoldout, $"{nameof(PackageExportInfo)}");
            if (_exportInfoFoldout)
            {
                _packageExportInfoDrawer.Draw(serializedObject);
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            _filePickerDrawer.Draw(serializedObject);

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            _packageFilesDrawer.Draw(_target.packageInfo);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
