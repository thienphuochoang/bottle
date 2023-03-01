using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using System;
namespace Bottle.Editor.GridSystem
{
    [System.Serializable]
    public class GridObjectBrushData
    {
        public GridTile gridTile;
        public GridEntity gridEntity;
        public float scale = 1.0f;
        public Vector3 rotation = Vector3.zero;

        public GridObjectBrushData() { }

        public GridObjectBrushData(GridObjectBrushData gridObjectBrushData)
        {
            this.gridEntity = gridObjectBrushData.gridEntity;
            this.gridTile = gridObjectBrushData.gridTile;
            this.scale = gridObjectBrushData.scale;
            this.rotation = gridObjectBrushData.rotation;
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
        public GridEntity Entity
        {
            get
            {
                return _entity;
            }
            set
            {
                _entity = value;
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
        [SerializeField] private GridEntity _entity;
        [SerializeField] private float _scale = 1;
        [SerializeField] Quaternion _rotation = Quaternion.identity;

        public override int GetHashCode()
        {
            int hash;
            unchecked
            {
                hash = Tile != null ? Tile.GetInstanceID() : 0;
                hash = Entity != null ? Entity.GetInstanceID() : 0;
                hash = hash * 33 + Scale.GetHashCode();
                hash = hash * 33 + Rotation.GetHashCode();
            }
            return hash;
        }
    }
    [CustomGridBrush(true, false, false, "[Bottle] Grid Object Brush")]
    public class GridObjectBrushEditor : GridBrushBase
    {
        #region Properties
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
        #endregion

        #region Methods
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

        #region EraseCell
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            UpdateBrushCellSelection();

            if (brushTarget == null || gridLayout == null) return;

            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, size);
            BoxErase(gridLayout, brushTarget, bounds);
        }
        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null) return;

            foreach (Vector3Int location in position.allPositionsWithin)
                EraseCell(gridLayout, location, brushTarget.transform);

            UpdateBrushCellSelection();
        }
        private void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            Vector2Int gridPosition = new Vector2Int(position.x, position.y);
            GridManager.Instance.EraseGridObjectAtPosition<GridTile>(gridPosition, position.z);
        }
        #endregion

        #region PaintCell
        
        private void PaintCell(GridLayout grid, Vector3Int position, BrushCell cell)
        {
            if (cell.Tile != null)
            {
                Vector2Int gridPosition = new Vector2Int(position.x, position.y);
                List<GridTile> checkedAlreadyPlacedGridObject = GridManager.Instance.GetGridObjectAtPosition<GridTile>(gridPosition, position.z);
                if (checkedAlreadyPlacedGridObject.Count > 0)
                {
                    Debug.LogError("There is already a grid tile available at this position");
                }
                else
                {
                    GridManager.Instance.CreateGridObject<GridTile>(cell.Tile, gridPosition, position.z, cell.Scale, cell.Rotation);
                }
            }
            else if (cell.Entity != null)
            {
                Vector2Int gridPosition = new Vector2Int(position.x, position.y);
                List<GridEntity> checkedAlreadyPlacedGridObject = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(gridPosition, position.z);
                if (checkedAlreadyPlacedGridObject.Count > 0)
                {
                    Debug.LogError("There is already a grid entity available at this position");
                }
                else
                {
                    GridManager.Instance.CreateGridObject<GridEntity>(cell.Entity, gridPosition, position.z, cell.Scale, cell.Rotation);
                }
                //if (checkedAlreadyPlacedGridObject == default(GridEntity))
                //    GridManager.Instance.CreateGridObject<GridEntity>(cell.Entity, gridPosition, position.z, cell.Scale, cell.Rotation);
                //else
                //    Debug.LogError("There is already a grid entity available at this position");
            }
        }

        public void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null || gridLayout == null) return;
            if (IsCellSelected == false)
                UpdateBrushCellSelection();

            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, _size);
            BoxFill(gridLayout, brushTarget, bounds);
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
        #endregion

        #region PickCell
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int brushPivot)
        {
            if (brushTarget == null || gridLayout == null) return;

            // Only pick the last grid object
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                PickCell(gridLayout, location, brushTarget.transform);
            }

            UpdateBrushCellSelection();
        }
        private void PickCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            Vector2Int gridPosition = new Vector2Int(position.x, position.y);
            List<GridTile> pickedGridObject = GridManager.Instance.GetGridObjectAtPosition<GridTile>(gridPosition, position.z);
            if (pickedGridObject.Count > 0)
            {
                UnityEngine.Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(pickedGridObject[0]);
                if (prefab)
                {
                    SetBrushCellData(pickedGridObject[0], pickedGridObject[0].transform.localScale.x, pickedGridObject[0].transform.localRotation);
                }
            }
        }
        #endregion

        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return (x % size.x) + size.x * (y % size.y) + size.x * size.y * (z % size.z);
        }

        public void UpdateBrushCellSelection()
        {
            //Re-init
            Init();
            IsCellSelected = false;

            if (_currentSelectedBrushCell != null)
                _cells[0] = _currentSelectedBrushCell;
        }


        public void SetBrushCellData<T>(T gridObject, float scale, Quaternion orientation) where T : Component
        {
            System.Type gridObjectType = typeof(T);
            if (gridObjectType.Equals(typeof(GridTile)))
            {
                _currentSelectedBrushCell = new BrushCell();
                _currentSelectedBrushCell.Tile = (GridTile)(object)gridObject;
                _currentSelectedBrushCell.Scale = scale;
                _currentSelectedBrushCell.Rotation = orientation;
            }
            else if (gridObjectType.Equals(typeof(GridEntity)))
            {
                _currentSelectedBrushCell = new BrushCell();
                _currentSelectedBrushCell.Entity = (GridEntity)(object)gridObject;
                _currentSelectedBrushCell.Scale = scale;
                _currentSelectedBrushCell.Rotation = orientation;
            }
        }

        public void ClearBrushCellData()
        {
            UpdateBrushCellSelection();
            _currentSelectedBrushCell = null;
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
        #endregion
    }
}

