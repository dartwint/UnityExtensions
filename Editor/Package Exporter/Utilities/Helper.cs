using System.IO;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public static class Helper
    {
        public static bool IsInProjectFolder(string path)
        {
            path = Path.GetFullPath(path).Replace(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            string projectDir = AssetFinder.ProjectRootDir.Replace(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (path + "/" == projectDir)
                return true;

            return path.StartsWith(projectDir);
        }

        public static void FormatPaths(ref string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = AssetFinder.GetRelativeToAssetsPath(paths[i]);
            }
        }
    }
}
