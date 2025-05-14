using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PackagePreset", menuName = "Export/Unity package export config", order = 1)]
public class PackagePreset : ScriptableObject
{
    [HideInInspector] public bool isTrackable = true;

    public string packageName = "Package";

    public ExportPackageOptions ExportPackageOptions { get; private set; } = ExportPackageOptions.Interactive;

    [SerializeField] private string[] _extensions = { ".png", ".jpg", ".jpeg", ".tga", ".bmp", ".gif", ".psd", ".tiff", ".tif", ".webp" };

    public List<string> assetPaths;

    private void OnEnable()
    {
        if (isTrackable)
            AssetTracker.RegisterPackage(this);
    }

    [CustomEditor(typeof(PackagePreset))]
    public class PackagePresetEditor : Editor
    {
        private PackagePreset _package;

        private AssetsSelectorWindow _assetsSelectorWindow;

        public enum SelectionOperandType { ADD, MASK };

        private void OnAssetsSelectorClosed()
        {
            if (_assetsSelectorWindow == null) return;

            _assetsSelectorWindow.AssetsSelected -= OnAssetsSelected;
            _assetsSelectorWindow.Closed -= OnAssetsSelectorClosed;
        }

        private void DrawOpenSelectorControls()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Open items selector modes", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add items"))
            {
                _assetsSelectorWindow.Show("Addition items selector", _package._extensions, SelectionOperandType.ADD);
            }

            if (GUILayout.Button("Remove items"))
            {
                _assetsSelectorWindow.Show("Remove items selector", _package._extensions, SelectionOperandType.MASK);
            }
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            if (_assetsSelectorWindow == null)
            {
                _assetsSelectorWindow = CreateInstance<AssetsSelectorWindow>();
                _assetsSelectorWindow.AssetsSelected += OnAssetsSelected;
                _assetsSelectorWindow.Closed += OnAssetsSelectorClosed;
            }

            base.OnInspectorGUI();

            _package = (PackagePreset) target;

            DrawOpenSelectorControls();

            GUILayout.Space(15);

            EditorGUI.BeginChangeCheck();
            _package.isTrackable = EditorGUILayout.Toggle("Enable tracking", _package.isTrackable);
            if (EditorGUI.EndChangeCheck())
            {
                if (_package.isTrackable) AssetTracker.RegisterPackage(_package);
                else AssetTracker.UnregisterPackage(_package);
            }
            if (_package.isTrackable)
            {
                GUILayout.Space(15);

                if (GUILayout.Button("Show tracker logger window"))
                {
                    AssetTrackerLoggerWindow.ShowWindow();
                }

                if (GUILayout.Button("Refresh and print collision items"))
                {
                    AssetTracker.RefreshAndLogCollisions(_package);
                }
            }
        }

        private void OnAssetsSelected(List<string> files, List<string> directories, SelectionOperandType fileSelectionType)
        {
            if (_package.assetPaths == null)
                _package.assetPaths = new List<string>();

            switch (fileSelectionType)
            {
                case SelectionOperandType.ADD:
                    {
                        foreach (string file in files)
                        {
                            if (!_package.assetPaths.Contains(file))
                                _package.assetPaths.Add(file);
                        }

                        break;
                    }
                case SelectionOperandType.MASK:
                    {
                        foreach (string file in files)
                        {
                            if (_package.assetPaths.Contains(file))
                                _package.assetPaths.Remove(file);
                        }

                        break;
                    }
            }

            EditorUtility.SetDirty(_package);
            AssetDatabase.SaveAssetIfDirty(_package);

            Debug.Log($"Files in {_package.name} to export count: " + _package.assetPaths.Count);
        }

        private void PrintDiffTest()
        {
            string s = Application.dataPath + "\\Services\\Editor\\Package exporter\\Asset tracker\\Data\\MicroSplat_TerrainCollection_URP2022";
            string content = File.ReadAllText(s + ".json");
            TrackedAssetsWrapper wrapper = JsonUtility.FromJson<TrackedAssetsWrapper>(content);
            List<string> l = wrapper.trackedAssetsPath;

            List<string> diff = GetDifference(_package.assetPaths, l);
            foreach (string str in diff)
            {
                Debug.Log(str);
            }

            Debug.Log(diff.Count);
        }

        public static List<string> GetDifference(List<string> list1, List<string> list2)
        {
            List<string> diff = new();

            int n = list1.Count, m = list2.Count;

            for (int i = 0; i < n; i++)
            {
                if (!list1.Contains(list2[i]))
                    diff.Add(list1[i]);
            }

            if (n >= m)
                return diff;

            for (int i = n; i < m - 1; i++)
            {
                diff.Add(list2[i]);
            }

            return diff;
        }

        [System.Serializable]
        public class TrackedAssetsWrapper
        {
            public List<string> trackedAssetsPath;
        }
    }
}
