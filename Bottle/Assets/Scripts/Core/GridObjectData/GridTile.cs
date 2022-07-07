using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
using Bottle.Core.PathSystem;
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
        [ShowInInspector]
        public List<GridEntity> currentStandingGridEntity;

        [DisableIf("isStandable", true)]
        [BoxGroup("Grid Tile Settings", true, true)]
        [Tooltip("Is this Grid Tile a ramp?")]
        public bool isARamp = false;
        [DisableIf("isStandable", true)]
        [BoxGroup("Grid Tile Settings", true, true)]
        [Tooltip("Is this Grid Tile a ramp?")]
        public GameplayManager.GlobalDirection stepOnDirection = GameplayManager.GlobalDirection.NONE;


        // These are properties for path finding
        [HideInInspector]
        public int gCost;
        [HideInInspector]
        public int hCost;
        [HideInInspector]
        public int fCost => gCost + hCost;
        [HideInInspector]
        public GridTile cameFromGridTile;

        protected override void Awake()
        {
            base.Awake();
            currentStandingGridEntity = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(this.gridPosition, this.gridHeight + 1);
            PathFinding.ResetDistanceCost(this);
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}