public static class StringExtension
{
    private const char _standardDirSeparator = '/';

    public static string UnifyDirSeparator(this string path) =>
        path.Replace('\\', _standardDirSeparator);

    public static string UnifyDirPath(this string dirPath) =>
        dirPath.UnifyDirSeparator() + _standardDirSeparator;
}
