using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectData
{
    public class GridEntity : GridObject
    {
        public enum FacingDirections { NONE, PositiveX, NegativeX, PositiveZ, NegativeZ };
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

        [BoxGroup("Grid Entity Settings", true, true)]
        [Tooltip("The current direction that this Grid Entity is facing")]
        [ShowInInspector]
        public FacingDirections currentFacingDirection = FacingDirections.NONE;

        public Vector3 currentRotation
        {
            get => this.transform.localEulerAngles;
            set
            {
                if (this.transform.localEulerAngles == value) return;
                this.transform.localEulerAngles = value;
                if (OnRotationChanged != null)
                    OnRotationChanged(currentFacingDirection, this.transform.localEulerAngles);
            }
        }

        public delegate Vector3 OnRotationChangedDelegate(FacingDirections currentFacingDirection, Vector3 newRotationEulerAngles);
        public event OnRotationChangedDelegate OnRotationChanged;

        protected override void Update()
        {
            base.Update();
        }

        protected override void Start()
        {
            base.Start();
            OnRotationChanged += ConvertFacingDirectionToValue;
            //_currentStandingGridTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(this.gridPosition, this.gridHeight - 1)[0];
            //_currentStandingGridTile.OnStandingGridEntityChanged += _currentStandingGridTile.SetStandingGridEntity;
            //_currentStandingGridTile.currentStandingGridEntity.Add(this);
            //OnPositionChanged += OnCurrentStandingGridTileChangedHandler;
            //OnControllableObjectChanged += ChangeControllableSetting;
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
            _currentStandingGridTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(newGridPosition, newGridHeight - 1)[0];
            _currentStandingGridTile.OnStandingGridEntityChanged += _currentStandingGridTile.SetStandingGridEntity;
            _currentStandingGridTile.currentStandingGridEntity.Add(this);
        }
        public Vector3 ConvertFacingDirectionToValue(FacingDirections facingDirection, Vector3 newRotationEulerAngles)
        {
            switch (facingDirection)
            {
                case FacingDirections.PositiveX:
                    return this.transform.localEulerAngles + Vector3Int.forward;
                case FacingDirections.NegativeX:
                    return this.transform.localEulerAngles + Vector3Int.back;
                case FacingDirections.PositiveZ:
                    return this.transform.localEulerAngles + Vector3Int.right;
                case FacingDirections.NegativeZ:
                    return this.transform.localEulerAngles + Vector3Int.left;
            }
            return Vector3Int.zero;
        }
    }
}