using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectData
{
    public class GridTileData : MonoBehaviour
    {
        [BoxGroup("Tile Settings")]
        [Tooltip("Whether or not can the Character move to this tile.")]
        public bool isTileWalkable = true;

        [ReadOnly]
        [BoxGroup("Tile Settings")]
        [Tooltip("The position of this tile in the Grid.")]
        public Vector2Int gridPosition = new Vector2Int(0, 0);

        [ReadOnly]
        [BoxGroup("Tile Settings")]
        [Tooltip("The height of this tile in the Grid.")]
        public float gridHeight = 0f;

    }
}

