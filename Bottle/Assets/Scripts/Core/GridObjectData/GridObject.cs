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
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The position of this grid object in the Grid.")]
        public Vector2Int gridPosition = new Vector2Int(0, 0);

        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The height of this grid object in the Grid.")]
        public float gridHeight = 0f;

        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The height of this grid object in the Grid.")]
        public Vector3 pivotOffset = new Vector3(0, 0, 0);

        public void Update()
        {
            if (this.transform.hasChanged)
            {
                Vector3Int cellPosition = GridManager.Instance.grid.WorldToCell(this.transform.position);
                Vector3 newPos = GridManager.Instance.grid.GetCellCenterWorld(cellPosition);
                newPos.y = Mathf.Ceil(this.transform.position.y);
                this.transform.position = newPos;
                // Mathf.RoundToInt did not work because they always round up to even result
                gridPosition = new Vector2Int((int)(newPos.x - 0.5f), (int)(newPos.z - 0.5f));
                gridHeight = (int)newPos.y + Mathf.Ceil(pivotOffset.y);
            }
        }
    }
}

