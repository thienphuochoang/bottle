using System.Collections;
using System.Collections.Generic;
using Bottle.Core.GridObjectAbility;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectData
{
    public class GridEntity : GridObject
    {
        [BoxGroup("Grid Entity Settings", true, true)]
        [Tooltip("The specific grid entity setup.")]
        [SerializeField]
        private bool _isControllable = true;
        public bool isControllable
        {
            get { return _isControllable; }
            set
            {
                if (_isControllable == value) return;
                _isControllable = value;
                //if (OnControllableObjectChanged != null)
                //{
                EventManager.Instance.TriggerEvent("ChangeMainControllableGridEntity", null);
                EventManager.Instance.TriggerEvent("MovementDirectionDetectionEventsChanged", new Dictionary<string, object> { { "GridEntity", this }, { "IsControllable", this._isControllable } });
                //    OnControllableObjectChanged(_isControllable);
                //}
            }
        }

        [BoxGroup("Grid Entity Settings", true, true)]
        [Tooltip("The current Grid Tile used to be stand on")]
        [ShowInInspector]
        public GridTile currentStandingGridTile;


        protected override void Update()
        {
            base.Update();
        }

        protected override void Start()
        {
            base.Start();
            //currentStandingGridTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(this.gridPosition, this.gridHeight - 1)[0];
            OnPositionChanged += OnCurrentStandingGridTileChangedHandler;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            currentStandingGridTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(this.gridPosition, this.gridHeight - 1)[0];
        }
        private void OnCurrentStandingGridTileChangedHandler(Vector2Int newGridPosition, int newGridHeight)
        {
            // Remove current standing Grid Entity on the old grid tile
            if (currentStandingGridTile != null)
            {
                currentStandingGridTile.currentStandingGridEntity = null;
            }
            var currentStandingGridTileList = GridManager.Instance.GetGridObjectAtPosition<GridTile>(newGridPosition, newGridHeight - 1);
            if (currentStandingGridTileList.Count > 0)
            {
                currentStandingGridTile = currentStandingGridTileList[0];
                currentStandingGridTile.currentStandingGridEntity = new List<GridEntity> { this };
            }
                
        }
    }
}