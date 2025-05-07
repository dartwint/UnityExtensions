using UnityEditor;
using UnityEngine;

//[CreateAssetMenu(fileName = "BuildStatusSettings", menuName = "Editor/Build Status Settings")]
public class BuildStatusSettings : ScriptableObject
{
    public const string settingsAssetName = "BuildStatusSettings";
    public const string settingsAssetDir = "Data/";

    public bool autoReload = true;

    public bool AutoCompile
    {
        get
        {
            int status = EditorPrefs.GetInt("kAutoRefresh");
            if (status == 1)
                return true;

            return false;
        }
        set
        {
            int status = EditorPrefs.GetInt("kAutoRefresh");
            if ((status == 1 && value == true) || (status == 0 && value == false))
                return;

            if (value == true)
            {
                EditorPrefs.SetInt("kAutoRefresh", 1);
                Debug.Log("Auto script recompile is enabled");
            }
            else
            {
                EditorPrefs.SetInt("kAutoRefresh", 0);
                Debug.Log("Auto script recompile is disabled");
            }
        }
    }

    public Texture2D upToDateIcon;
    public Texture2D outdatedAssetsWithoutScriptsIcon;
    public Texture2D outdatedIcon;
}