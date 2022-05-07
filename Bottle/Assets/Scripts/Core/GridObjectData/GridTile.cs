using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectData
{
    public class GridTile : GridObject
    {
        [BoxGroup("Grid Tile Settings", true, true)]
        [Tooltip("Permission for Grid Entity standing on")]
        public bool isStandable;

        [ShowIf("isStandable", true)]
        [BoxGroup("Grid Tile Settings", true, true)]
        [Tooltip("The current Grid Entity standing on it")]
        public GridEntity _currentStandingGridEntity;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}