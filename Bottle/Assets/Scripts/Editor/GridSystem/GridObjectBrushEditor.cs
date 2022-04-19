using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using Bottle.Core.GridObjectData;
using System;
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
    [System.Serializable]
    public class BrushCell
    {
        public GridTile Tile
        {
            get
            {
                return _tile;
            }
            set
            {
                _tile = value;
            }
        }
        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        
        [SerializeField] private GridTile _tile;
        [SerializeField] private float _scale = 1;
        [SerializeField] Quaternion _rotation = Quaternion.identity;

        public override int GetHashCode()
        {
            int hash;
            unchecked
            {
                hash = Tile != null ? Tile.GetInstanceID() : 0;
                hash = hash * 33 + Scale.GetHashCode();
                hash = hash * 33 + Rotation.GetHashCode();
            }
            return hash;
        }
    }
    [CustomGridBrush(true, false, false, "[Bottle] Grid Object Brush")]
    public class GridObjectBrushEditor : GridBrushBase
    {
        public int sizeCount
        {
            get { return _size.x * _size.y * _size.z; }
        }
        private bool _isCellSelected = false;
        public bool IsCellSelected
        {
            get
            {
                if (_isCellSelected == true && IsCellNull == false)
                    return true;
                return false;
            }
            set
            {
                _isCellSelected = value;
            }
        }
        public bool IsCellNull
        {
            get
            {
                foreach (var eachCell in _cells)
                {
                    if (eachCell != null)
                        return false;
                }
                return true;
            }
        }
        private BrushCell _currentSelectedBrushCell;
        [SerializeField]
        private BrushCell[] _cells;
        public BrushCell[] cells { get { return _cells; } }
        private Vector3Int _size;
        public Vector3Int size => _size;
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

        public GridObjectBrushEditor()
        {
            Init();
        }

        private void Init()
        {
            _pivot = Vector3Int.zero;
            _size = Vector3Int.one;
            SizeUpdated();
        }

        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return (x % size.x) + size.x * (y % size.y) + size.x * size.y * (z % size.z);
        }

        private void PaintCell(GridLayout grid, Vector3Int position, BrushCell tile)
        {
            if (tile.Tile != null)
                Debug.Log(tile.Tile);
                //GridManager.Instance.InstantiateGridTile(tile.Tile, position.ToVector2IntXY(), tile.Height, tile.Orientation);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null) return;
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell tile = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (tile != null)
                    PaintCell(gridLayout, location, tile);
            }
        }

        public void DeselectCell()
        {
            //Re-init
            Init();
            IsCellSelected = false;

            if (_currentSelectedBrushCell != null)
                _cells[0] = _currentSelectedBrushCell;
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null || gridLayout == null) return;
            if (IsCellSelected == false)
                DeselectCell();

            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, _size);
            BoxFill(gridLayout, brushTarget, bounds);
        }

        public void SetBrushCellData(GridTile gridTile, float scale, Quaternion orientation)
        {
            _currentSelectedBrushCell = new BrushCell();
            _currentSelectedBrushCell.Tile = gridTile;
            _currentSelectedBrushCell.Scale = scale;
            _currentSelectedBrushCell.Rotation = orientation;
        }
        public int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        public int GetCellIndex(int x, int y, int z)
        {
            return x + _size.x * y + _size.x * _size.y * z;
        }

        private void SizeUpdated()
        {
            Array.Resize(ref _cells, sizeCount);
            _cells = new BrushCell[_size.x * _size.y * _size.z];
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, _size);
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (_cells[GetCellIndex(pos)] == null)
                    _cells[GetCellIndex(pos)] = new BrushCell();
            }
        }
    }
}

