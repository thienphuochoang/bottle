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
        private GridEntity _currentStandingGridEntity;
        public GridEntity currentStandingGridEntity
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

        public delegate void OnStandingGridEntityDelegate(GridEntity newStandingGridEntity);
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

        private void SetStandingGridEntity(GridEntity newStandingGridEntity)
        {
            if (_currentStandingGridEntity != null)
                _currentStandingGridEntity = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(this.gridPosition, this.gridHeight + 1);
        }
    }
}