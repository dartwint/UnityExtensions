using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public enum PackageFilesEditMode
    {
        ADD,
        REMOVE
    }

    public enum SelectionItemType
    {
        File,
        Directory
    }

    public class SelectiorOptions
    {
        public SelectionMode selectionMode;
        public SelectionItemType selectionItemType;
    }

    public enum SelectionMode
    {
        Single,
        Multiple
    }

    public enum SelectiorDialogResult
    {
        Canceled,
        OK,
        OutOfProjectFolder
    }

    public class FilePickerViewModel
    {
        public FilePickerModel model;

        private PackagePresetNEW _preset;

        public PackageFilesEditMode PackageFilesEditMode { get; private set; }
        public SelectionMode selectionMode;
        public SelectiorDialogResult selectionResult;
        public SearchOption searchOption;

        public event Action ViewClosed;

        public FilePickerViewModel(FilePickerModel model, PackagePresetNEW preset)
        {
            this.model = model;
            _preset = preset;
        }

        public void SaveChanges()
        {
            var files = model.GetSelectedFiles();
            if (files == null || files.Length == 0)
                return;

            if (PackageFilesEditMode == PackageFilesEditMode.ADD)
                _preset.packageInfo.AddFiles(files);
            else if (PackageFilesEditMode == PackageFilesEditMode.REMOVE)
                _preset.packageInfo.RemoveFiles(files);

            _preset.Save();
        }

        public void HandleSelector()
        {
            string folderPath = "";

            if (selectionMode == SelectionMode.Multiple)
            {
                folderPath = EditorUtility.OpenFolderPanel(GetSelectorPanelTitle(), Application.dataPath, "");
            }
            else if (selectionMode == SelectionMode.Single)
            {
                folderPath = EditorUtility.OpenFilePanelWithFilters(
                    GetSelectorPanelTitle(), Application.dataPath, new string[] { "All files", "*" });
            }

            OnSelectorClosed(folderPath);
        }

        public void AddFile(string file)
        {
            model.AddFile(file);
        }

        public void RemoveFile(string file)
        {
            model.RemoveFile(file);
        }

        public void RemoveFiles(string[] files)
        {
            model.RemoveFiles(files);
        }

        public void AddFiles(string[] files)
        {
            model.AddFiles(files);
        }

        public void AddItemsButtonClick()
        {
            PackageFilesEditMode = PackageFilesEditMode.ADD;
            ShowView();
        }

        public void RemoveItemsButtonClick()
        {
            PackageFilesEditMode = PackageFilesEditMode.REMOVE;
            ShowView();
        }

        private void ShowView()
        {
            FilePickerWindow window = ScriptableObject.CreateInstance<FilePickerWindow>();
            window.titleContent.text = GetViewTitle(PackageFilesEditMode);
            window.viewModel = this;
            window.ShowModal();
        }

        public void CloseView(FilePickerWindow window)
        {
            ViewClosed?.Invoke();
        }

        public void AddItems(string folderPath)
        {
            if (selectionMode == SelectionMode.Multiple)
            {
                foreach (string ext in model.GetExtensions())
                {
                    string[] files = Directory.GetFiles(folderPath, "*" + ext, searchOption);
                    FormatPaths(ref files);
                    model.AddFiles(files);
                }
            }
            else if (selectionMode == SelectionMode.Single)
            {
                folderPath = AssetFinder.GetRelativeToAssetsPath(folderPath);

                AddFile(folderPath);
            }
        }

        public void RemoveItems(string folderPath)
        {
            if (selectionMode == SelectionMode.Multiple)
            {
                foreach (string ext in model.GetExtensions())
                {
                    string[] files = Directory.GetFiles(folderPath, "*" + ext, searchOption);
                    FormatPaths(ref files);
                    RemoveFiles(files);
                }
            }
            else if (selectionMode == SelectionMode.Single)
            {
                folderPath = AssetFinder.GetRelativeToAssetsPath(folderPath);

                RemoveFile(folderPath);
            }
        }

        private void OnSelectorClosed(string selectedFile)
        {
            selectionResult = ValidateSelection(selectedFile);
            if (selectionResult != SelectiorDialogResult.OK)
                return;

            if (PackageFilesEditMode == PackageFilesEditMode.ADD)
                AddItems(selectedFile);
            else if (PackageFilesEditMode == PackageFilesEditMode.REMOVE)
                RemoveItems(selectedFile);
        }

        private string GetViewTitle(PackageFilesEditMode pickMode)
        {
            switch (pickMode)
            {
                case PackageFilesEditMode.ADD:
                    {
                        return "File picker (addition)";
                    }
                case PackageFilesEditMode.REMOVE:
                    {
                        return "File picker (removal)";
                    }
            }

            return "File picker (none)";
        }

        private string GetSelectorPanelTitle()
        {
            string title = "";

            if (selectionMode == SelectionMode.Multiple)
                title += "Multiple ";
            else if (selectionMode == SelectionMode.Single)
                title += "Single ";

            title += "selector";

            return title;
        }

        private static SelectiorDialogResult ValidateSelection(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return SelectiorDialogResult.Canceled;

            if (IsInProjectFolder(folderPath))
                return SelectiorDialogResult.OK;
            else
                return SelectiorDialogResult.OutOfProjectFolder;
        }

        // TO DO: move method to another class
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
