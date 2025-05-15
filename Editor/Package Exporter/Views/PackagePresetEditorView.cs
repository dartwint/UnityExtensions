using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    [CustomEditor(typeof(PackagePresetNEW))]
    public class PackagePresetEditorView : UnityEditor.Editor
    {
        private PackagePresetNEW _target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _target = (PackagePresetNEW) target;
            if (_target == null)
                return;


        }
    }
}
