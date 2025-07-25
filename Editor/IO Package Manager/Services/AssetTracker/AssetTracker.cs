using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class AssetTracker : AssetPostprocessor
    {
        private static HashSet<Package> _packages = new();

        private static Dictionary<Package, Dictionary<string, HashSet<Package>>> _collisionsDictionary;

        private static void UpdateCollisionsDictionary()
        {
            if (_collisionsDictionary == null)
                _collisionsDictionary = new();
            else
                _collisionsDictionary.Clear();

            foreach (Package package in _packages)
            {
                Dictionary<string, HashSet<Package>> collisionsForPreset = new();

                foreach (string path in package.packageFiles.GetFiles())
                {
                    if (!collisionsForPreset.ContainsKey(path))
                    {
                        collisionsForPreset.Add(path, new HashSet<Package>());
                    }
                }

                foreach (Package other in _packages)
                {
                    if (package == other)
                        continue;

                    foreach (string path in other.packageFiles.GetFiles())
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

                _collisionsDictionary.Add(package, collisionsForPreset);
            }
        }

        private static List<Package> GetCommonPresets(string path)
        {
            List<Package> result = new();

            if (_packages.Count == 0)
                return result;

            foreach (Package preset in _collisionsDictionary.Keys)
            {
                if (!_collisionsDictionary[preset].ContainsKey(path))
                    continue;

                result.Add(preset);
            }

            return result;
        }

        public static void RefreshAndLogCollisions(Package preset)
        {
            UpdateCollisionsDictionary();

            if (!_collisionsDictionary.ContainsKey(preset))
                return;

            bool hasCollisions = false;

            foreach (string path in _collisionsDictionary[preset].Keys)
            {
                hasCollisions = true;

                string text = path + "\nalso in other presets: ";
                foreach (Package p in _collisionsDictionary[preset][path])
                {
                    text += p.name + ", ";
                }
                text = text.Substring(0, text.Length - 2);
                AssetTrackerLoggerWindow.LogMessage(text, preset, AssetTrackerLoggerWindow.MessageType.Warning);
            }

            if (!hasCollisions)
                AssetTrackerLoggerWindow.LogMessage("No collisions detected", preset);
        }

        public static void RegisterPackage(Package package)
        {
            if (!_packages.Contains(package))
            {
                _packages.Add(package);
                //UpdateCollisionsDictionary();
            }
        }

        public static void UnregisterPackage(Package package)
        {
            if (_packages.Contains(package))
            {
                _packages.Remove(package);
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
            if (_packages.Count == 0) return;

            foreach (Package package in _packages)
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
                    if (package.packageFiles.GetFiles().Contains(assetPath))
                    {
                        AssetTrackerLoggerWindow.LogMessage($"Asset imported: {assetPath}.", package);
                    }

                    /*string assetDir = AssetPathRetriever.GetAssetPath(Directory.GetDirectoryRoot(assetPath));
                    if (nonIgnoredDirs.Contains(assetDir))
                    {


                        AssetTrackerLoggerWindow.LogMessage($"(from dir) Asset imported: {assetPath}. Sender: " + preset.packageName);
                    }*/
                }

                foreach (string assetPath in deletedAssets)
                {
                    if (package.packageFiles.GetFiles().Contains(assetPath))
                    {
                        hasChanges = true;

                        AssetTrackerLoggerWindow.LogMessage($"Asset deleted: {assetPath}.", package);
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

                    if (package.packageFiles.GetFiles().Contains(oldPath))
                    {
                        package.packageFiles.RemoveFile(oldPath);
                        package.packageFiles.AddFile(newPath);

                        hasChanges = true;

                        AssetTrackerLoggerWindow.LogMessage($"Asset moved from {oldPath} to {newPath}.", package);
                    }

                    /*if (preset.assetDirs.Contains(AssetPathRetriever.GetAssetPath(Directory.GetDirectoryRoot(newPath))))
                    {
                        AssetTrackerLoggerWindow.LogMessage($"(from dir) Asset moved to: {newPath}. Sender: " + preset.packageName);
                    }*/
                }

                if (hasChanges)
                {
                    EditorUtility.SetDirty(package);
                    AssetDatabase.SaveAssetIfDirty(package);
                }
            }
        }
    }
}