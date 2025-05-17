using System;
using System.IO;
using UnityEngine;

namespace Dartwint.UnityExtensions.Editor.PackageExporter
{
    public enum PickMode
    {
        ADD,
        REMOVE
    }

    // TO DO: perhaps rename the type
    public enum SelectionSize
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

        public PickMode pickMode;
        public SelectionSize selectionSize;
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

            if (pickMode == PickMode.ADD)
                _preset.packageInfo.AddFiles(files);
            else if (pickMode == PickMode.REMOVE)
                _preset.packageInfo.RemoveFiles(files);

            _preset.Save();
        }

        public void AddFile(string file)
        {
            model.AddFile(file);
        }

        public void RemoveFile(string file)
        {
            model.RemoveFile(file);
        }

        public void AddFiles(string[] files)
        {
            model.AddFiles(files);
        }

        public void AddItemsButtonClick()
        {
            ShowView(PickMode.ADD);
        }

        public void RemoveItemsButtonClick()
        {
            ShowView(PickMode.REMOVE);
        }

        private void ShowView(PickMode pickMode)
        {
            this.pickMode = pickMode;
            FilePickerWindow window = ScriptableObject.CreateInstance<FilePickerWindow>();
            window.titleContent.text = GetViewTitle(pickMode);
            window.viewModel = this;
            window.ShowModal();
        }

        public void CloseView(FilePickerWindow window)
        {
            ViewClosed?.Invoke();
        }

        public void AddItems(string folderPath)
        {
            if (selectionSize == SelectionSize.Multiple)
            {
                foreach (string ext in model.GetExtensions())
                {
                    string[] files = Directory.GetFiles(folderPath, "*" + ext, searchOption);
                    FormatPaths(ref files);
                    model.AddFiles(files);
                }
            }
            else if (selectionSize == SelectionSize.Single)
            {
                folderPath = AssetFinder.GetRelativeToAssetsPath(folderPath);

                AddFile(folderPath);
            }
        }

        public void RemoveItems(string folderPath)
        {
            if (selectionSize == SelectionSize.Multiple)
            {
                foreach (string ext in model.GetExtensions())
                {
                    string[] files = Directory.GetFiles(folderPath, "*" + ext, searchOption);
                    FormatPaths(ref files);
                    model.RemoveFiles(files);
                }
            }
            else if (selectionSize == SelectionSize.Single)
            {
                folderPath = AssetFinder.GetRelativeToAssetsPath(folderPath);

                RemoveFile(folderPath);
            }
        }

        public void OnSelectorClosed(string selectedFile)
        {
            selectionResult = ValidateSelection(selectedFile);
            if (selectionResult != SelectiorDialogResult.OK)
                return;

            if (pickMode == PickMode.ADD)
                AddItems(selectedFile);
            else if (pickMode == PickMode.REMOVE)
                RemoveItems(selectedFile);
        }

        private string GetViewTitle(PickMode pickMode)
        {
            switch (pickMode)
            {
                case PickMode.ADD:
                    {
                        return "File picker (addition)";
                    }
                case PickMode.REMOVE:
                    {
                        return "File picker (removal)";
                    }
            }

            return "File picker (none)";
        }

        public static SelectiorDialogResult ValidateSelection(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return SelectiorDialogResult.Canceled;

            if (IsInProjectFolder(folderPath))
                return SelectiorDialogResult.OK;
            else
                return SelectiorDialogResult.OutOfProjectFolder;
        }

        // TO DO: move method to another class
        private static bool IsInProjectFolder(string path)
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
