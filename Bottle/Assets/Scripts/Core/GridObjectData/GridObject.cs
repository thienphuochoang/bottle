using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectData
{
    [ExecuteInEditMode]
    public class GridObject : MonoBehaviour
    {
        [SerializeField]
        private Vector3 lastPos;

        [ReadOnly]
        [BoxGroup("Grid Object Settings")]
        [Tooltip("The position of this grid object in the Grid.")]
        public Vector2Int gridPosition = new Vector2Int(0, 0);

        [ReadOnly]
        [BoxGroup("Grid Object Settings")]
        [Tooltip("The height of this grid object in the Grid.")]
        public float gridHeight = 0f;

        private void Awake()
        {
            
        }

        void Update()
        {
            if (this.transform.hasChanged)
            {
                Vector3Int cellPosition = GridManager.Instance.grid.WorldToCell(this.transform.position);
                Vector3 newPos = GridManager.Instance.grid.GetCellCenterWorld(cellPosition);
                Debug.Log(newPos);
                newPos.y = Mathf.Ceil(this.transform.position.y);
                this.transform.position = newPos;
                gridPosition = new Vector2Int(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.z));
                gridHeight = Mathf.RoundToInt(newPos.y);
            }
        }
    }
}

