namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public interface IUnityPackageExportProcessor
    {
        bool Export(PackageInfo packageInfo, UnityPackageExportInfo exportInfo);
    }
}
