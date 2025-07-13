namespace Dartwint.UnityExtensions.IOPackageManager
{
    public interface IExportProcessor<in T> where T : PackageExportOptions
    {
        bool CanProccess(Package package);
        bool Export(Package package);
    }

    public interface IExportProcessor
    {
        bool CanProccess(Package package);
        bool Export(Package package);
    }
}
