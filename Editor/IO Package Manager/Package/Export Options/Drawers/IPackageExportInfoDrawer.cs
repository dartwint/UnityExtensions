using UnityEditor;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public interface IPackageExportInfoDrawer
    {
        void Draw(SerializedObject serializedObject);
    }
}
