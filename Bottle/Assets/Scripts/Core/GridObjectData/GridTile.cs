using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
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

        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public GridTile previousTile;

        //public delegate void OnStandingGridEntityDelegate(List<GridEntity> newStandingGridEntity);
        //public event OnStandingGridEntityDelegate OnStandingGridEntityChanged;

        protected override void Awake()
        {
            base.Awake();
            currentStandingGridEntity = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(this.gridPosition, this.gridHeight + 1);
            gCost = int.MaxValue;
            previousTile = null;
        }

        protected override void Start()
        {
            base.Start();
            //OnStandingGridEntityChanged += SetStandingGridEntity;
        }

        //public void SetStandingGridEntity(List<GridEntity> newStandingGridEntity)
        //{
        //    _currentStandingGridEntity = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(this.gridPosition, this.gridHeight + 1);
        //}
    }
}