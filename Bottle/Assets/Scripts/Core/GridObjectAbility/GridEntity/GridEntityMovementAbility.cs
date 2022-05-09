using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityMovementAbility : GridObjectAbility<GridEntity>
    {
        [ShowInInspector]
        private Dictionary<KeyCode, InputButton> _movementButtonStates => InputManager.Instance.buttonStates;
        public enum MovementDirections { NONE, FORWARD, BACK , LEFT, RIGHT};
        [BoxGroup("Movement Settings", true, true)]
        [ShowInInspector]
        private MovementDirections _currentMovementDirection = MovementDirections.NONE;
        [BoxGroup("Movement Settings", true, true)]
        [ShowInInspector]
        [ReadOnly]
        private GridTile _targetTile;
        [BoxGroup("Movement Settings", true, true)]
        [ReadOnly]
        [SerializeField]
        private bool _isMoving = false;
        [BoxGroup("Turning Settings", true, true)]
        [ReadOnly]
        [SerializeField]
        private bool _alreadyTurned = false;


        [BoxGroup("Acceleration Settings", true, true)]
        public float maximumSpeed = 4;
        [BoxGroup("Acceleration Settings", true, true)]
        public float acceleration = 5;
        [BoxGroup("Acceleration Settings", true, true)]
        public float currentSpeed;
        [BoxGroup("Acceleration Settings", true, true)]
        public float maximumSpeedMultiplier = 1f;
        [BoxGroup("Acceleration Settings", true, true)]
        public float accelerationMultiplier = 1f;

        private void DetectMovementDirection(InputButton.States state, KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.W:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.FORWARD;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                    }
                    break;
                case KeyCode.S:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.BACK;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                    }
                    break;
                case KeyCode.A:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.LEFT;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                    }
                    break;
                case KeyCode.D:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.RIGHT;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                    }
                    break;
            }
        }

        public Vector3Int GetValueFromDirection(MovementDirections direction)
        {
            switch (direction)
            {
                case MovementDirections.FORWARD: return Vector3Int.forward;
                case MovementDirections.BACK: return Vector3Int.back;
                case MovementDirections.LEFT: return Vector3Int.left;
                case MovementDirections.RIGHT: return Vector3Int.right;
            }
            return Vector3Int.zero;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();
            if (_currentGridObject.isControllable)
            {
                _movementButtonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirection;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (_currentGridObject.isControllable)
            {
                ApplyAcceleration();
                if (_targetTile != null && _targetTile.isStandable == true && _targetTile.currentStandingGridEntity == null)
                {
                    if (_alreadyTurned == false)
                        Turn(_currentMovementDirection);
                    Move();
                }
                    
            }
        }
        private GridTile GetTargetTile(MovementDirections theDirection)
        {
            Vector3Int targetGridPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight - 1, _currentGridObject.gridPosition.y) + GetValueFromDirection(theDirection);
            var targetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(targetGridPosition.x, targetGridPosition.z), targetGridPosition.y);
            return targetTile;
        }
        private void ApplyAcceleration()
        {
            if (currentSpeed < maximumSpeed * maximumSpeedMultiplier)
            {
                currentSpeed = currentSpeed + acceleration * accelerationMultiplier * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, maximumSpeed * maximumSpeedMultiplier);
            }
        }
        private void Move()
        {
            Vector3 destination = new Vector3(_targetTile.transform.position.x, _targetTile.transform.position.y + 1, _targetTile.transform.position.z);
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * currentSpeed);
            if (this.transform.position != newPosition)
            {
                this.transform.position = newPosition;
                _isMoving = true;
            }
            else
            {
                var gridPos = GridManager.Instance.ConvertWorldPositionToGridPosition(this._currentGridObject);
                this._currentGridObject.gridPosition = new Vector2Int(gridPos.x, gridPos.z);
                this._currentGridObject.gridHeight = gridPos.y;
                _currentMovementDirection = MovementDirections.NONE;
                _isMoving = false;
                _alreadyTurned = false;
                _targetTile = null;
                currentSpeed = 0;
            }
        }
        private void Turn(MovementDirections targetDirection)
        {
            Vector3 currentChildRotationEulerAngle = this._currentGridObject.transform.rotation.eulerAngles;
            switch (targetDirection)
            {
                case MovementDirections.FORWARD:
                    this._currentGridObject.transform.RotateAround(this._currentGridObject.transform.position, this._currentGridObject.transform.parent.up, 0 - currentChildRotationEulerAngle.y);
                    break;
                case MovementDirections.BACK:
                    this._currentGridObject.transform.RotateAround(this._currentGridObject.transform.position, this._currentGridObject.transform.parent.up, 180 - currentChildRotationEulerAngle.y);
                    break;
                case MovementDirections.LEFT:
                    this._currentGridObject.transform.RotateAround(this._currentGridObject.transform.position, this._currentGridObject.transform.parent.up, -90 - currentChildRotationEulerAngle.y);
                    break;
                case MovementDirections.RIGHT:
                    this._currentGridObject.transform.RotateAround(this._currentGridObject.transform.position, this._currentGridObject.transform.parent.up, 90 - currentChildRotationEulerAngle.y);
                    break;
            }
            _alreadyTurned = true;
        }
    }
}
