using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static PackagePreset.PackagePresetEditor;

[Obsolete("")]
public class AssetsSelectorWindow : EditorWindow
{
    public event Action Closed;
    public event Action<List<string>, List<string>, SelectionOperandType> AssetsSelected;

    private SelectionOperandType _operandType;

    private List<string> _selectedFiles = new();
    private List<string> _selectedDirectories = new();

    private List<string> _pickedFolders = new();

    private Vector2 _pickedFoldersScrollPosition;

    private string[] _extensions;

    private enum ItemType { File, Directory };
    private ItemType _fileType = ItemType.File;

    private Dictionary<ItemType, List<string>> _selectedItemsDictionary = new();
    private Dictionary<ItemType, Vector2> _selectedItemsScrollsPosition = new();

    private enum SelectionMode { Single, Multiple };
    private SelectionMode _selectionMode;

    private void Awake()
    {
        _selectedItemsDictionary[ItemType.File] = _selectedFiles;
        _selectedItemsDictionary[ItemType.Directory] = _selectedDirectories;

        _selectedItemsScrollsPosition.Add(ItemType.File, Vector2.zero);
        _selectedItemsScrollsPosition.Add(ItemType.Directory, Vector2.zero);
    }

    public void Show(string title, string[] extensions, SelectionOperandType fileSelectionType)
    {
        this.titleContent.text = title;
        _extensions = extensions;
        _operandType = fileSelectionType;

        ShowModal();
    }

    private SearchOption _searchOption = SearchOption.AllDirectories;

    private enum SelectionResult { Cancel, Valid, OutOfProject }
    private SelectionResult _lastSelectionResult = SelectionResult.Valid;

    private void DrawItemsWithType(ItemType itemType)
    {
        string title = "Selected items";
        if (itemType == ItemType.File)
            title = "Selected files";
        else if (itemType == ItemType.Directory)
            title = "Selected directories";

        if (_selectedItemsDictionary[itemType].Count > 0)
        {
            title += $" (count: {_selectedItemsDictionary[itemType].Count})";
            GUILayout.Label(title, EditorStyles.boldLabel);

            _selectedItemsScrollsPosition[itemType] = EditorGUILayout.BeginScrollView(_selectedItemsScrollsPosition[itemType], GUILayout.Height(200));

            for (int i = 0; i < _selectedItemsDictionary[itemType].Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _selectedItemsDictionary[itemType][i] = EditorGUILayout.TextField(_selectedItemsDictionary[itemType][i]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _selectedItemsDictionary[itemType].RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("No items selected at the moment", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
        }
    }

    private void DrawPickedFolders()
    {
        if (_pickedFolders.Count == 0) return;

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Picked folders list", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical(GUI.skin.box);
        _pickedFoldersScrollPosition = EditorGUILayout.BeginScrollView(_pickedFoldersScrollPosition, GUILayout.Height(65));

        foreach (string folder in _pickedFolders)
        {
            GUILayout.Label(folder);
        }

        GUILayout.EndVertical();

        EditorGUILayout.EndScrollView();

        GUILayout.Space(8);
    }

    private void AddItems(string selectedItem, ItemType fileType, SelectionMode selectionMode)
    {
        if (selectionMode == SelectionMode.Multiple)
        {
            if (fileType == ItemType.File)
            {
                foreach (string ext in _extensions)
                {
                    string[] files = Directory.GetFiles(selectedItem, "*" + ext, _searchOption);
                    FormatPaths(ref files);
                    foreach (string file in files)
                    {
                        if (!_selectedFiles.Contains(file))
                            _selectedFiles.Add(file);
                    }
                }
            }
            else if (fileType == ItemType.Directory)
            {
                string[] dirs = Directory.GetDirectories(selectedItem, "*", _searchOption);
                FormatPaths(ref dirs);
                foreach (string dir in dirs)
                {
                    if (!_selectedDirectories.Contains(dir))
                        _selectedDirectories.Add(dir);
                }
            }
        }
        else if (selectionMode == SelectionMode.Single)
        {
            selectedItem = AssetFinder.GetRelativeToAssetsPath(selectedItem);

            if (fileType == ItemType.File)
            {
                if (!_selectedFiles.Contains(selectedItem))
                    _selectedFiles.Add(selectedItem);
            }
            else if (fileType == ItemType.Directory)
            {
                if (!_selectedDirectories.Contains(selectedItem))
                    _selectedDirectories.Add(selectedItem);
            }
        }
    }

    private string GetSelectorPanelTitle()
    {
        string title = "";

        if (_selectionMode == SelectionMode.Multiple)
            title += "Multiple ";
        else if (_selectionMode == SelectionMode.Single)
            title += "Single ";

        if (_fileType == ItemType.File)
            title += "file ";
        else if (_fileType == ItemType.Directory)
            title += "directory ";

        title += "selector";

        return title;
    }

    private void DrawOutOfProjectWarning()
    {
        GUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon.sml"), GUILayout.Width(20));
        GUILayout.Label("Selected item was out of Unity project directory. Selection was cancelled.");
        GUILayout.EndHorizontal();
    }

    private bool IsInProjectFolder(string path)
    {
        path = Path.GetFullPath(path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        string projectDir = AssetFinder.ProjectRootDir.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (path + "/" == projectDir)
            return true;

        return path.StartsWith(projectDir);
    }

    private SelectionResult ValidateSelection(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
            return SelectionResult.Cancel;

        if (IsInProjectFolder(folderPath))
            return SelectionResult.Valid;
        else
            return SelectionResult.OutOfProject;
    }
    private void HandleSelector()
    {
        string folderPath = "";

        if (_selectionMode == SelectionMode.Multiple)
        {
            folderPath = EditorUtility.OpenFolderPanel(GetSelectorPanelTitle(), "", "");
        }
        else if (_selectionMode == SelectionMode.Single)
        {
            if (_fileType == ItemType.File)
            {
                folderPath = EditorUtility.OpenFilePanelWithFilters(GetSelectorPanelTitle(), "", new string[] { "All files", "*" });
            }
            else if (_fileType == ItemType.Directory)
            {
                folderPath = EditorUtility.OpenFolderPanel(GetSelectorPanelTitle(), "", "");
            }
        }

        _lastSelectionResult = ValidateSelection(folderPath);

        if (_lastSelectionResult != SelectionResult.Cancel)
            _pickedFolders.Add(folderPath);

        if (_lastSelectionResult != SelectionResult.Valid)
            return;

        AddItems(folderPath, _fileType, _selectionMode);
    }

    private void DrawSelectionControls()
    {
        GUILayout.BeginVertical(GUI.skin.box);

        GUI.enabled = false;

        _fileType = (ItemType) EditorGUILayout.EnumPopup("File type", _fileType);

        /*if (_fileType == ItemType.Directory)
        {
            GUI.enabled = false;
            _selectionMode = SelectionMode.Single;
            _searchOption = SearchOption.TopDirectoryOnly;
        }
        else
        {
            GUI.enabled = true;
        }*/

        GUI.enabled = true;

        _selectionMode = (SelectionMode) EditorGUILayout.EnumPopup("Selection mode", _selectionMode);

        if (!(_selectionMode == SelectionMode.Single && _fileType == ItemType.File))
        {
            _searchOption = (SearchOption) EditorGUILayout.EnumPopup("Search option", _searchOption);
        }

        if (GUILayout.Button("Open selector"))
        {
            HandleSelector();
        }

        GUILayout.EndVertical();
    }

    private Vector2 _controlsScrollPosition;
    private void OnGUI()
    {
        _controlsScrollPosition = GUILayout.BeginScrollView(_controlsScrollPosition);

        DrawSelectionControls();

        //DrawPickedFolders();

        if (_lastSelectionResult == SelectionResult.OutOfProject)
        {
            DrawOutOfProjectWarning();
        }

        DrawItemsWithType(ItemType.File);

        //GUILayout.Space(15);
        //DrawItemsWithType(ItemType.Directory);

        GUILayout.Space(15);

        if (GUILayout.Button("Save and close"))
        {
            AssetsSelected?.Invoke(_selectedFiles, _selectedDirectories, _operandType);

            Close();
        }

        GUILayout.EndScrollView();
    }

    public static void FormatPaths(ref string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetFinder.GetRelativeToAssetsPath(paths[i]);
        }
    }

    private void OnDestroy() => Closed?.Invoke();
}