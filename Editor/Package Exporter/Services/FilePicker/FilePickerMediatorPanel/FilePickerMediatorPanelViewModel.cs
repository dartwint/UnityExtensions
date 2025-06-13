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

    public class SelectorOptions
    {
        public SelectionMode selectionMode = SelectionMode.Single;
        public SelectionItemType selectionItemType = SelectionItemType.File;
        public SearchOption searchOption = SearchOption.TopDirectoryOnly;
    }

    public enum SelectionMode
    {
        Single,
        Multiple
    }

    public enum SelectorDialogResult
    {
        Canceled,
        OK,
        OutOfProjectFolder
    }

    public class FilePickerMediatorPanelViewModel
    {
        public FilePickerMediatorPanelModel model;

        private PackagePreset _preset;

        public PackageFilesEditMode PackageFilesEditMode { get; private set; }
        public SelectorOptions SelectorOptions { get; private set; } = new();
        public SelectorDialogResult selectorResult;

        public event Action ViewClosed;

        public FilePickerMediatorPanelViewModel(FilePickerMediatorPanelModel model, PackagePreset preset)
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

        // TO DO: refactor
        public void HandleSelectorDialog()
        {
            string folderPath = "";

            if (SelectorOptions.selectionMode == SelectionMode.Single)
            {
                if (SelectorOptions.selectionItemType == SelectionItemType.File)
                {
                    folderPath = EditorUtility.OpenFilePanelWithFilters(
                        GetSelectorPanelTitle(), Application.dataPath, new string[] { "All files", "*" });
                }
                else if (SelectorOptions.selectionItemType == SelectionItemType.Directory)
                {
                    folderPath = EditorUtility.OpenFolderPanel(
                        GetSelectorPanelTitle(), Application.dataPath, "");
                }
            }
            else if (SelectorOptions.selectionMode == SelectionMode.Multiple)
            {
                Debug.LogAssertion("Selector with multiple selection mode is not implemented");
            }

            OnSelectorClosed(folderPath);
        }

        public void RemoveFileButtonClick(string file)
        {
            model.RemoveFile(file);
        }

        public void OpenViewForAdditionButtonClick()
        {
            PackageFilesEditMode = PackageFilesEditMode.ADD;
            ShowView();
        }

        public void OpenViewForRemovalButtonClick()
        {
            PackageFilesEditMode = PackageFilesEditMode.REMOVE;
            ShowView();
        }

        private void ShowView()
        {
            FilePickerMediatorPanelView window = ScriptableObject.CreateInstance<FilePickerMediatorPanelView>();
            window.titleContent.text = GetViewTitle(PackageFilesEditMode);
            window.viewModel = this;
            window.ShowUtility();
        }

        public void CloseView(FilePickerMediatorPanelView window)
        {
            ViewClosed?.Invoke();
        }

        // TO DO: refactor
        private void UpdatePickedItemsList(string itemPath)
        {
            if (SelectorOptions.selectionMode == SelectionMode.Multiple)
            {
                foreach (string ext in model.GetExtensions())
                {
                    string[] files = Directory.GetFiles(itemPath, "*" + ext, SelectorOptions.searchOption);
                    Helper.FormatPaths(ref files);

                    model.AddFiles(files);
                }
            }
            else if (SelectorOptions.selectionMode == SelectionMode.Single)
            {
                itemPath = AssetFinder.GetRelativeToAssetsPath(itemPath);
                model.AddFile(itemPath);
            }
        }

        private void OnSelectorClosed(string selectedFile)
        {
            selectorResult = ValidateSelectionResult(selectedFile);
            if (selectorResult == SelectorDialogResult.OK)
            {
                UpdatePickedItemsList(selectedFile);
            }
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

            return string.Empty;
        }

        private static SelectorDialogResult ValidateSelectionResult(string selectedPath)
        {
            if (string.IsNullOrEmpty(selectedPath))
                return SelectorDialogResult.Canceled;

            if (Helper.IsInProjectFolder(selectedPath))
                return SelectorDialogResult.OK;
            else
                return SelectorDialogResult.OutOfProjectFolder;
        }

        private string GetSelectorPanelTitle()
        {
            string title = "";

            if (SelectorOptions.selectionMode == SelectionMode.Multiple)
                title += "Multiple ";
            else if (SelectorOptions.selectionMode == SelectionMode.Single)
                title += "Single ";

            title += "selector";

            return title;
        }
    }
}
