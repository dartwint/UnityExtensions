using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class AssetTracker : AssetPostprocessor
{
    private static HashSet<PackagePreset> _presets = new();

    private static Dictionary<PackagePreset, Dictionary<string, HashSet<PackagePreset>>> _collisionsDictionary;

    private static void UpdateCollisionsDictionary()
    {
        if (_collisionsDictionary == null)
            _collisionsDictionary = new();
        else
            _collisionsDictionary.Clear();

        foreach (PackagePreset preset in _presets)
        {
            Dictionary<string, HashSet<PackagePreset>> collisionsForPreset = new Dictionary<string, HashSet<PackagePreset>>();

            foreach (string path in preset.assetPaths)
            {
                if (!collisionsForPreset.ContainsKey(path))
                {
                    collisionsForPreset.Add(path, new HashSet<PackagePreset>());
                }
            }

            foreach (PackagePreset other in _presets)
            {
                if (preset == other)
                    continue;

                foreach (string path in other.assetPaths)
                {
                    if (collisionsForPreset.ContainsKey(path))
                    {
                        collisionsForPreset[path].Add(other);
                    }
                }
            }

            var kvps = collisionsForPreset.ToArray();
            for (int i = collisionsForPreset.Count - 1; i > -1; i--)
            {
                if (kvps[i].Value.Count == 0)
                    collisionsForPreset.Remove(kvps[i].Key);
            }

            _collisionsDictionary.Add(preset, collisionsForPreset);
        }
    }

    private static List<PackagePreset> GetCommonPresets(string path)
    {
        List<PackagePreset> result = new();

        if (_presets.Count == 0)
            return result;

        foreach (PackagePreset preset in _collisionsDictionary.Keys)
        {
            if (!_collisionsDictionary[preset].ContainsKey(path))
                continue;

            result.Add(preset);
        }

        return result;
    }

    public static void RefreshAndLogCollisions(PackagePreset preset)
    {
        UpdateCollisionsDictionary();

        if (!_collisionsDictionary.ContainsKey(preset))
            return;

        bool hasCollisions = false;

        foreach (string path in _collisionsDictionary[preset].Keys)
        {
            hasCollisions = true;

            string text = path + "\nalso in other presets: ";
            foreach (PackagePreset p in _collisionsDictionary[preset][path])
            {
                text += p.name + ", ";
            }
            text = text.Substring(0, text.Length - 2);
            AssetTrackerLoggerWindow.LogMessage(text, preset, AssetTrackerLoggerWindow.MessageType.Warning);
        }

        if (!hasCollisions)
            AssetTrackerLoggerWindow.LogMessage("No collisions detected", preset);
    }

    public static void RegisterPackage(PackagePreset preset)
    {
        if (!_presets.Contains(preset))
        {
            _presets.Add(preset);
            //UpdateCollisionsDictionary();
        }
    }

    public static void UnregisterPackage(PackagePreset package)
    {
        if (_presets.Contains(package))
        {
            _presets.Remove(package);
            //UpdateCollisionsDictionary();
        }
    }

    /*private static List<string> GetNonIgnoredDirectories(PackagePreset preset, out List<string> ignoredSubDirs)
    {
        List<string> result = new(preset.assetDirs);
        ignoredSubDirs = new();

        foreach (string dir in preset.assetDirs)
        {
            foreach (string ignoreDir in preset.ignoreDirs)
            {
                if (dir == ignoreDir)
                {
                    result.Remove(dir);
                    //break;
                }
                else if (dir.StartsWith(ignoreDir))
                {
                    result.Remove(dir);
                }
                else if (ignoreDir.StartsWith(dir))
                {
                    ignoredSubDirs.Add(ignoreDir);
                }
            }
        }

        return result;
    }*/

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (_presets.Count == 0) return;

        foreach (PackagePreset preset in _presets)
        {
            bool hasChanges = false;

            /*List<string> ignoredSubDirs;
            List<string> nonIgnoredDirs = GetNonIgnoredDirectories(preset, out ignoredSubDirs);

            List<string> dirFiles = new();
            foreach (string dir in preset.assetDirs)
            {
                string[] items = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                AssetsSelectorWindow.FormatPaths(ref items);

                List<string> itemsList = new List<string>(items);

                foreach (string ignoredDir in preset.ignoreDirs)
                {
                    if (dir == ignoredDir)
                    {
                        itemsList.Clear();
                        break;
                    }
                    else if (dir.StartsWith(ignoredDir))
                    {
                        itemsList.Clear();
                        break;
                    }
                    else if (ignoredDir.StartsWith(dir))
                    {
                        string[] ignoreItems = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                        AssetsSelectorWindow.FormatPaths(ref ignoreItems);

                        foreach (string ignoredItem in ignoreItems)
                        {
                            if (itemsList.Contains(ignoredItem))
                                itemsList.Remove(ignoredItem);
                        }
                    }
                }
                dirFiles.AddRange(itemsList);
            }*/

            foreach (string assetPath in importedAssets)
            {
                if (preset.assetPaths.Contains(assetPath))
                {
                    AssetTrackerLoggerWindow.LogMessage($"Asset imported: {assetPath}.", preset);
                }

                /*string assetDir = AssetPathRetriever.GetAssetPath(Directory.GetDirectoryRoot(assetPath));
                if (nonIgnoredDirs.Contains(assetDir))
                {


                    AssetTrackerLoggerWindow.LogMessage($"(from dir) Asset imported: {assetPath}. Sender: " + preset.packageName);
                }*/
            }

            foreach (string assetPath in deletedAssets)
            {
                if (preset.assetPaths.Contains(assetPath))
                {
                    hasChanges = true;

                    AssetTrackerLoggerWindow.LogMessage($"Asset deleted: {assetPath}.", preset);
                }

                /*if (preset.assetDirs.Contains(AssetPathRetriever.GetAssetPath(Directory.GetDirectoryRoot(assetPath))))
                {
                    AssetTrackerLoggerWindow.LogMessage($"(from dir) Asset deleted: {assetPath}. Sender: " + preset.packageName);
                }*/
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                string oldPath = movedFromAssetPaths[i];
                string newPath = movedAssets[i];

                if (preset.assetPaths.Contains(oldPath))
                {
                    preset.assetPaths.Remove(oldPath);
                    preset.assetPaths.Add(newPath);

                    hasChanges = true;

                    AssetTrackerLoggerWindow.LogMessage($"Asset moved from {oldPath} to {newPath}.", preset);
                }

                /*if (preset.assetDirs.Contains(AssetPathRetriever.GetAssetPath(Directory.GetDirectoryRoot(newPath))))
                {
                    AssetTrackerLoggerWindow.LogMessage($"(from dir) Asset moved to: {newPath}. Sender: " + preset.packageName);
                }*/
            }

            if (hasChanges)
            {
                EditorUtility.SetDirty(preset);
                AssetDatabase.SaveAssetIfDirty(preset);
            }
        }
    }
}