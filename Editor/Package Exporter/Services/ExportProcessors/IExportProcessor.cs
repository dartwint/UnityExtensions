namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public interface IExportProcessor
    {
        bool CanProccess(PackagePreset packagePreset);
        bool Export(PackagePreset packagePreset);
    }
}
