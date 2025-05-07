using UnityEditor;

public class AutoCompileSwitch
{
    public static void EnableDomainReload() =>
        EditorApplication.UnlockReloadAssemblies();

    public static void DisableDomainReload() =>
        EditorApplication.LockReloadAssemblies();
}
