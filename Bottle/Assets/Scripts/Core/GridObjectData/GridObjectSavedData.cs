using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bottle.Core.GridObjectData
{
    [System.Serializable]
    public class GridObjectSavedData
    {
        public GridTile savedGridTile;
        public GridEntity savedGridEntity;
        public Vector2Int savedGridPosition;
        public float savedGridHeight;

        public GridObjectSavedData() { }

        public GridObjectSavedData(GridObjectSavedData gridObjectSavedData)
        {
            this.savedGridTile = gridObjectSavedData.savedGridTile;
            this.savedGridEntity = gridObjectSavedData.savedGridEntity;
            this.savedGridPosition = gridObjectSavedData.savedGridPosition;
            this.savedGridHeight = gridObjectSavedData.savedGridHeight;
        }
    }
}

