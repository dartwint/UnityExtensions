using UnityEditor;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public interface IPackageExportInfoDrawer
    {
        void Draw(SerializedObject serializedObject);
    }
}
