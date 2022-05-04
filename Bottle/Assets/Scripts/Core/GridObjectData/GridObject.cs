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
    public abstract class GridObject : MonoBehaviour
    {
        [JsonProperty]
        [ReadOnly]
        [SerializeField]
        private string _uID;
        public string uID => _uID;

        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The position of this grid object in the Grid.")]
        private Vector2Int _gridPosition;
        public Vector2Int gridPosition
        {
            get => _gridPosition;
            set
            {
                if (_gridPosition == value) return;
                _gridPosition = value;
                if (OnPositionChanged != null)
                    OnPositionChanged(_gridPosition);
            }
        }

        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The height of this grid object in the Grid.")]
        public float gridHeight = 0f;

        [JsonProperty]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The height of this grid object in the Grid.")]
        public Vector3 pivotOffset;

        public delegate void OnPositionChangedDelegate(Vector2Int newPosition);
        public event OnPositionChangedDelegate OnPositionChanged;

        protected virtual void Update()
        {
            if (this.transform.hasChanged)
            {
                Vector3Int cellPosition = GridManager.Instance.grid.WorldToCell(this.transform.position);
                Vector3 newPos = GridManager.Instance.grid.GetCellCenterWorld(cellPosition);
                newPos.y = Mathf.Ceil(this.transform.position.y);
                this.transform.position = newPos;
                // Mathf.RoundToInt did not work because they always round up to even result
                _gridPosition = new Vector2Int((int)(newPos.x - 0.5f), (int)(newPos.z - 0.5f));
                gridHeight = (int)newPos.y + Mathf.Ceil(pivotOffset.y);
            }
        }
        protected virtual void Start()
        {
            this.OnPositionChanged += PositionChangedHandler;
            _uID = this.GetInstanceID().ToString();
        }

        private void PositionChangedHandler(Vector2Int newPosition)
        {
            Debug.Log("event changed");
        }
    }
}

