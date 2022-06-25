using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
namespace Bottle.Core.PathSystem
{
    public class PathFindingGridTile
    {
        public GridTile currentGridTile;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public PathFindingGridTile previousGridTile;

        public PathFindingGridTile()
        {
            this.gCost = int.MaxValue;
            this.previousGridTile = null;
        }
    }
}
