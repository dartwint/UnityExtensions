using System;
using UnityEditor;

using UnityEngine;

/// <summary>
/// This class represents the window with GUI of the terrain's heights offset tool
/// </summary>
public class View : EditorWindow
{
    /// <summary>
    /// Selected Terrain object which heights should be adjusted
    /// </summary>
    /// 
    /// <remarks>
    /// This field is just a visual representation of the <see cref="Model"/> actual tool's data field
    /// </remarks>
    public Terrain Terrain { get; private set; }

    /// <summary>
    /// Offset that should be applied for each height of the <see cref="Terrain"/>
    /// </summary>
    /// 
    /// <remarks>
    /// This field is just a visual representation of the <see cref="Model"/> actual tool's data field
    /// </remarks>
    public float Offset { get; private set; }

    /// <summary>
    /// Called after apply button pressed
    /// </summary>
    public event Action ApplyButtonPressed;

    /// <summary>
    /// Called after this instance destroys
    /// </summary>
    public event Action WindowDestroyed;

    /// <summary>
    /// Draws the window's interface
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Terrain's heights offset adjustment panel", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        Terrain = (Terrain) EditorGUILayout.ObjectField(Terrain, typeof(Terrain), true);
        Offset = EditorGUILayout.FloatField("Offset", Offset);

        if (Terrain == null || Terrain.terrainData == null)
        {
            EditorGUILayout.HelpBox("Terrain or it's data is null", MessageType.Info);
            return;
        }

        if (GUILayout.Button("Apply offset"))
        {
            ApplyButtonPressed?.Invoke();
        }
    }

    private void OnDestroy()
    {
        WindowDestroyed?.Invoke();
    }
}