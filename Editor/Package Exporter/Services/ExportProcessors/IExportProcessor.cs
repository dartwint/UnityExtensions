namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public interface IExportProcessor
    {
        bool CanProccess(PackagePresetNEW packagePreset);
        bool Export(PackagePresetNEW packagePreset);
    }
}
