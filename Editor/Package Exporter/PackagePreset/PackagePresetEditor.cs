using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    /// <summary>
    /// The view and controller operating with <see cref="PackagePresetNEW"></see>
    /// </summary>
    [CustomEditor(typeof(PackagePresetNEW))]
    public class PackagePresetEditor : UnityEditor.Editor
    {
        private IFilePickerDrawer _filePickerDrawer;
        private IPackageInfoDrawer _packageInfoDrawer;
        private PackagePresetNEW _target;

        private void OnEnable()
        {
            if (_filePickerDrawer == null)
            {
                _filePickerDrawer = new DragNDropDrawer(new UnityObjectPickerFromSourceDraggable(this), this);
                //_filePickerDrawer = new FilePickerMediatorPanelDrawer();
            }

            if (_packageInfoDrawer == null)
            {
                _packageInfoDrawer = new PackageInfoDrawerDummy();
            }
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            _target = (PackagePresetNEW) target;
            if (_target == null)
                return;

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(nameof(_target.exportInfo)), true);

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            _filePickerDrawer.Draw(serializedObject);

            GUILayout.Space(EditorGUIUtility.singleLineHeight * 1.0f);
            _packageInfoDrawer.Draw(_target.packageInfo);
        }
    }
}
