namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public interface IRawExportProcessor
    {
        bool Export(PackageInfo packageInfo, string targetDirectory);
    }
}
