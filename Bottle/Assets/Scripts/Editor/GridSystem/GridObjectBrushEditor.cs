using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using Bottle.Core.GridObjectData;
namespace Bottle.Editor.GridSystem
{
    [System.Serializable]
    public class GridTileBrushData
    {
        public GridTileData gridTile;
        public float scale = 1.0f;
        public Vector3 rotation = Vector3.zero;

        public GridTileBrushData(GridTileBrushData gridTileBrushData)
        {
            this.gridTile = gridTileBrushData.gridTile;
            this.scale = gridTileBrushData.scale;
            this.rotation = gridTileBrushData.rotation;
        }

        public void ResetParameters()
        {
            scale = 1.0f;
            rotation = Vector3.zero;
        }
    }
    [CustomGridBrush(true, false, false, "[Bottle] Grid Object Brush")]
    public class GridObjectBrushEditor : GridBrushBase
    {
        // ahihi
    }
}

