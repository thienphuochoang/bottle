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
        private float threshold = 0.2f;

        [ReadOnly]
        [BoxGroup("Grid Object Settings")]
        [Tooltip("The position of this grid object in the Grid.")]
        public Vector2Int gridPosition = new Vector2Int(0, 0);

        [ReadOnly]
        [BoxGroup("Grid Object Settings")]
        [Tooltip("The height of this grid object in the Grid.")]
        public float gridHeight = 0f;

        void Update()
        {
            Vector3Int cellPosition = GridManager.Instance.grid.WorldToCell(this.transform.position);
            Vector3 newPos = GridManager.Instance.grid.GetCellCenterWorld(cellPosition);
            this.transform.position = newPos;
            //gridPosition = new Vector2Int(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.z));
        }
    }
}

