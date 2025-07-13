using UnityEditor;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public interface IFilePickerDrawer
    {
        void Draw(SerializedObject serializedObject);
    }
}
