using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectData
{
    public class GridEntity : GridObject
    {
        [BoxGroup("Grid Entity Settings", true, true)]
        [Tooltip("The specific grid entity setup.")]
        public bool isControllable = true;

        [BoxGroup("Grid Entity Settings", true, true)]
        [Tooltip("The current Grid Tile used to be stand on")]
        [ShowInInspector]
        private GridTile _currentStandingGridTile;
        public GridTile currentStandingGridTile
        {
            get => _currentStandingGridTile;
            set
            {
                if (_currentStandingGridTile == value) return;
                _currentStandingGridTile = value;
            }
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void Start()
        {
            base.Start();
            _currentStandingGridTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(this.gridPosition, this.gridHeight - 1);
            _currentStandingGridTile.OnStandingGridEntityChanged += _currentStandingGridTile.SetStandingGridEntity;
            _currentStandingGridTile.currentStandingGridEntity = this;
            OnPositionChanged += OnCurrentStandingGridTileChangedHandler;
        }

        protected override void Awake()
        {
            base.Awake();
            
        }
        private void OnCurrentStandingGridTileChangedHandler(Vector2Int newGridPosition, int newGridHeight)
        {
            if (_currentStandingGridTile != null)
            {
                _currentStandingGridTile.currentStandingGridEntity = null;
                _currentStandingGridTile.OnStandingGridEntityChanged += _currentStandingGridTile.SetStandingGridEntity;
            }
            _currentStandingGridTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(newGridPosition, newGridHeight - 1);
            _currentStandingGridTile.OnStandingGridEntityChanged += _currentStandingGridTile.SetStandingGridEntity;
            _currentStandingGridTile.currentStandingGridEntity = this;
        }
    }
}