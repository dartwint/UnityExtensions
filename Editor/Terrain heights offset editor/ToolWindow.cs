using UnityEditor;

namespace CustomServices.UnityEditor.Terrain.HeightmapOffsetTool
{
    /// <summary>
    /// This class just a window-wrapper for the View component of the tool. View is the actual tool window.
    /// </summary>
    public class ToolWindow : EditorWindow
    {
        [MenuItem("Tools/Terrain/Set heightmap offset")]
        public static void ShowWindow()
        {
            View view = GetWindow<View>("Terrain heightmap offset tool");
            Model model = new Model();
            Controller controller = new Controller(model, view);
        }
    }
}