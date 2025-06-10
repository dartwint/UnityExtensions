using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public interface IFilePickerDrawer
    {
        void Draw(SerializedObject serializedObject);
    }
}
