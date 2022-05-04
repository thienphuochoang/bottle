using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Bottle.Core.Manager;
using Newtonsoft.Json;
namespace Bottle.Core.GridObjectData
{
    [JsonObject(MemberSerialization.OptIn)]
    [ExecuteInEditMode]
    public abstract class GridObject : MonoBehaviour
    {
        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The unique ID of this grid object in the Grid.")]
        [SerializeField]
        private string _uID;
        public string uID => _uID;

        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The position of this grid object in the Grid.")]
        [SerializeField]
        private Vector2Int _gridPosition = new Vector2Int(int.MaxValue, int.MaxValue);
        public Vector2Int gridPosition
        {
            get => _gridPosition;
            set
            {
                if (_gridPosition == value) return;
                _gridPosition = value;
                if (OnPositionChanged != null)
                    OnPositionChanged(_gridPosition, (int)_gridHeight);
            }
        }

        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The height of this grid object in the Grid.")]
        [SerializeField]
        private float _gridHeight = float.MaxValue;

        public float gridHeight
        {
            get => _gridHeight;
            set
            {
                if (_gridHeight == value) return;
                _gridHeight = value;
                if (OnPositionChanged != null)
                    OnPositionChanged(_gridPosition, (int)_gridHeight);
            }
        }

        [JsonProperty]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The height of this grid object in the Grid.")]
        public Vector3 pivotOffset;

        public delegate void OnPositionChangedDelegate(Vector2Int newGridPosition, int newGridHeight);
        public event OnPositionChangedDelegate OnPositionChanged;

        protected virtual void Update()
        {
            if (this.transform.hasChanged)
            {
                Vector3Int gridPosAndGridHeight = GridManager.Instance.ConvertWorldPositionToGridPosition(this);
                gridPosition = new Vector2Int(gridPosAndGridHeight.x, gridPosAndGridHeight.z);
                gridHeight = gridPosAndGridHeight.y;
            }
        }

        private void Awake()
        {
            Vector3Int gridPosAndGridHeight = GridManager.Instance.ConvertWorldPositionToGridPosition(this);
            gridPosition = new Vector2Int(gridPosAndGridHeight.x, gridPosAndGridHeight.z);
            gridHeight = gridPosAndGridHeight.y;
        }

        protected virtual void Start()
        {
            this.OnPositionChanged += PositionChangedHandler;
            _uID = this.GetInstanceID().ToString();
        }

        private void PositionChangedHandler(Vector2Int newGridPosition, int newGridHeight)
        {
            Vector3Int newCellPos = new Vector3Int(newGridPosition.x, newGridPosition.y, newGridHeight);
            Vector3 newWorldPos = GridManager.Instance.grid.GetCellCenterWorld(newCellPos);
            newWorldPos.y = newGridHeight;
            this.transform.position = newWorldPos;
        }
    }
}

