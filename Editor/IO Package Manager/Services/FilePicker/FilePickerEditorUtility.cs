using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class FilePickerEditorUtility : IFilePicker
    {
        public enum SelectorPanelType
        {
            File,
            Directory
        }

        public SelectorPanelType selectorType = SelectorPanelType.File;
        public string panelTitle = "File selector";
        public string[] fileFilters = new string[] { "All files", "*" };

        public IEnumerable<string> PickFiles()
        {
            if (selectorType == SelectorPanelType.File)
            {
                yield return EditorUtility.OpenFilePanelWithFilters(
                                    panelTitle, Application.dataPath, fileFilters);
            }
            else if (selectorType == SelectorPanelType.Directory)
            {
                yield return EditorUtility.OpenFolderPanel(
                                    panelTitle, Application.dataPath, "");
            }

            yield break;
        }
    }
}
