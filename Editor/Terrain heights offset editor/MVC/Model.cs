using UnityEngine;

/// <summary>
/// This class represents tool's data and business-logic
/// </summary>
public class Model
{
    /// <summary>
    /// Offset that should be applied for each height of the <see cref="_terrain"/> in its <see cref="TerrainData"/> component
    /// </summary>
    private float _offset = 0;

    /// <summary>
    /// Terrain object which heights should be adjusted
    /// </summary>
    private Terrain _terrain;

    /// <summary>
    /// Sets data for this instance
    /// </summary>
    /// <param name="view">Data that taken from the given <see cref="View"/> instance</param>
    public void UpdateData(View view)
    {
        _terrain = view.Terrain;
        _offset = view.Offset;
    }

    /// <summary>
    /// Checks for ability to adjust all pixels in the heightmap
    /// </summary>
    /// 
    /// <param name="message">Warning non-empty text for case when the method returns false, otherwise this will be empty</param>
    /// 
    /// <returns>true if all heights can be adjusted without exceeding maximum height of the <see cref="_terrain"/> </returns> in its terrain settings
    public bool AreHeightsOffsetable(out string message)
    {
        TerrainData terrainData = _terrain.terrainData;

        float maxPossibleHeight = terrainData.size.y;

        float relativeHeight = _offset / maxPossibleHeight;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        message = string.Empty;

        for (int y = 0; y < heightmapHeight; y++)
        {
            for (int x = 0; x < heightmapWidth; x++)
            {
                if (heights[y, x] + relativeHeight > 1f || heights[y, x] + relativeHeight < 0f)
                {
                    message = "Given offset will lead to lost some height data";
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Searches for the lowest height in array of heights
    /// </summary>
    /// 
    /// <param name="heights">Heights of the <see cref="_terrain"/></param>
    /// <param name="heightmapWidth">Heightmap width of the <see cref="_terrain"/></param>
    /// <param name="heightmapHeight">Heightmap height of the <see cref="_terrain"/></param>
    /// 
    /// <returns>Lowest height in <see cref="_terrain"/></returns>
    private float GetMinHeightmapHeight(ref float[,] heights, ref int heightmapWidth, ref int heightmapHeight)
    {
        float result = heights[0, 0];

        for (int y = 0; y < heightmapHeight; y++)
        {
            for (int x = 0; x < heightmapWidth; x++)
            {
                if (heights[x, y] < result) result = heights[x, y];
            }
        }

        return result;
    }

    /// <summary>
    /// Searches for the highest height in array of heights
    /// </summary>
    /// 
    /// <param name="heights">Heights of the <see cref="_terrain"/></param>
    /// <param name="heightmapWidth">Heightmap width of the <see cref="_terrain"/></param>
    /// <param name="heightmapHeight">Heightmap height of the <see cref="_terrain"/></param>
    /// 
    /// <returns>Highest height in <see cref="_terrain"/></returns>
    private float GetMaxHeightmapHeight(ref float[,] heights, ref int heightmapWidth, ref int heightmapHeight)
    {
        float result = heights[0, 0];

        for (int y = 0; y < heightmapHeight; y++)
        {
            for (int x = 0; x < heightmapWidth; x++)
            {
                if (heights[x, y] > result) result = heights[x, y];
            }
        }

        return result;
    }

    /// <summary>
    /// Applies offset with the given <see cref="_offset"/> to all heights of the <see cref="_terrain"/>
    /// </summary>
    public void ApplyOffset()
    {
        TerrainData terrainData = _terrain.terrainData;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        float newRelativeHeight = _offset / terrainData.size.y;

        for (int y = 0; y < heightmapHeight; y++)
        {
            for (int x = 0; x < heightmapWidth; x++)
            {
                heights[y, x] = Mathf.Clamp(heights[y, x] + newRelativeHeight, 0f, 1f);
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}