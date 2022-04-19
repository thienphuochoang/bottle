using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectData
{
    public class GridObject : SerializedMonoBehaviour
    {

        [ReadOnly]
        [BoxGroup("Grid Object Settings")]
        [Tooltip("The position of this grid object in the Grid.")]
        public Vector2Int gridPosition = new Vector2Int(0, 0);

        [ReadOnly]
        [BoxGroup("Grid Object Settings")]
        [Tooltip("The height of this grid object in the Grid.")]
        public float gridHeight = 0f;
    }
}

