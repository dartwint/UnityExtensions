using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class represents the warning dialog with user
/// </summary>
public class WarningDialog : EditorWindow
{
    /// <summary>
    /// Text of the warning
    /// </summary>
    private string _message;

    /// <summary>
    /// Called when user presses proceed button
    /// </summary>
    public event Action Proceed;

    public WarningDialog(string message)
    {
        _message = message;
    }

    /// <summary>
    /// Draws the warning dialog
    /// </summary>
    private void OnGUI()
    {
        EditorGUILayout.HelpBox(_message, MessageType.Warning);

        if (GUILayout.Button("Proceed anyway"))
        {
            Close();
            Proceed?.Invoke();
        }
    }
}
