using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectData
{
    public class GridTile : GridObject
    {
        [BoxGroup("Grid Tile Settings", true, true)]
        [Tooltip("The specific grid tile setup.")]
        public bool isStandable;
    }
}