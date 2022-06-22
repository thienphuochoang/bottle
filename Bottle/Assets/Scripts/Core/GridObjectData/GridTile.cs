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
        private List<GridEntity> _currentStandingGridEntity;
        public List<GridEntity> currentStandingGridEntity
        {
            get => _currentStandingGridEntity;
            set
            {
                if (_currentStandingGridEntity == value) return;
                _currentStandingGridEntity = value;
                if (OnStandingGridEntityChanged != null)
                {
                    OnStandingGridEntityChanged(_currentStandingGridEntity);
                }
            }
        }
        [DisableIf("isStandable", true)]
        [BoxGroup("Grid Tile Settings", true, true)]
        [Tooltip("Is this Grid Tile a ramp?")]
        public bool isARamp = false;
        [DisableIf("isStandable", true)]
        [BoxGroup("Grid Tile Settings", true, true)]
        [Tooltip("Is this Grid Tile a ramp?")]
        public GameplayManager.GlobalDirection stepOnDirection = GameplayManager.GlobalDirection.NONE;

        public int cost;
        public int distance;
        public int costDistance => cost + distance;
        public GridTile parent;

        public void SetDistance(int targetX, int targetY)
        {
            this.distance = Mathf.Abs(targetX - gridPosition.x) + Mathf.Abs(targetY - gridPosition.y);
        }

        public delegate void OnStandingGridEntityDelegate(List<GridEntity> newStandingGridEntity);
        public event OnStandingGridEntityDelegate OnStandingGridEntityChanged;

        protected override void Awake()
        {
            base.Awake();
            _currentStandingGridEntity = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(this.gridPosition, this.gridHeight + 1);
        }

        protected override void Start()
        {
            base.Start();
            OnStandingGridEntityChanged += SetStandingGridEntity;
        }

        public void SetStandingGridEntity(List<GridEntity> newStandingGridEntity)
        {
            _currentStandingGridEntity = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(this.gridPosition, this.gridHeight + 1);
        }
    }
}