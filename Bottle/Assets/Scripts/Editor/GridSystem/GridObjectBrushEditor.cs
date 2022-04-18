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
        public GridTile gridTile;
        public float scale = 1.0f;
        public Vector3 rotation = Vector3.zero;

        public GridTileBrushData() { }

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
    public class BrushCell
    {
        public GameObject ahihi;
    }
    [CustomGridBrush(true, false, false, "[Bottle] Grid Object Brush")]
    public class GridObjectBrushEditor : GridBrushBase
    {
        public BrushCell[] BrushCells => _brushCell;
        private BrushCell[] _brushCell;
        private Vector3Int _pivot;
        public Vector3Int pivot
        {
            get
            {
                return _pivot;
            }
            set
            {
                _pivot = value; 
            }
        }
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null || gridLayout == null) return;
            foreach (var ahihi in _brushCell)
            {
                Debug.Log(ahihi);
            }
        }
    }
}

