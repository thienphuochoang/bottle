using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragPassiveAbility : GridObjectPassiveAbility<GridEntity>
    {
        private GridEntityPushAndDragAbility _mainGridEntity;
        [ShowInInspector]
        [ReadOnly]
        private GridTile _targetTile;
        [ShowInInspector]
        private List<Vector3Int> _availableMovementDirection = new List<Vector3Int>();
        private bool _isBeingPushedOrDragged = false;
        [ShowInInspector]
        [ReadOnly]
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
        [BoxGroup("Movement Settings", true, true)]
        [ReadOnly]
        [ShowInInspector]
        private GridEntityMovementAbility.MovementDirections _currentMovementDirection = GridEntityMovementAbility.MovementDirections.NONE;

        protected override void Start()
        {
            base.Start();
            EventManager.Instance.StartListening("PushAndDragPassiveEvent", BeginBeingPushedOrDragged);
            //InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirectionFromPath;
            //InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirectionFromPath;
            //InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirectionFromPath;
            //InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirectionFromPath;
        }

        private void BeginBeingPushedOrDragged(Dictionary<string, object> message)
        {
            bool isTriggeredValue = (bool)message["isTriggered"];
            GridEntity currentInteractingObject = (GridEntity)message["interactingGridObject"];
            _mainGridEntity = (GridEntityPushAndDragAbility)message["controllableMainGridEntity"];
            if (currentInteractingObject == _currentGridObject)
            {
                isPassiveAbilityTriggered = isTriggeredValue;
            }
        }

        protected override void Update()
        {
            base.Update();
            ApplyAcceleration();
            if (isPassiveAbilityTriggered)
            {
                //GameplayManager.Instance.isTurnInProgress = true;
                isPassiveAbilityTriggered = false;
                _availableMovementDirection = _mainGridEntity.availableMovementDirection;
                _isBeingPushedOrDragged = true;
                //_currentGridObject.isBlockable = false;
                LimitMovementDirection();
                //_nextTargetTile = _mainGridEntity.currentStandingGridTile;
            }
            if ( isPassiveAbilityTriggered == false && _mainGridEntity != null && _isBeingPushedOrDragged == true)
            {
                //if (_mainGridEntity.currentStandingGridTile.gridPosition != _nextTargetTile.gridPosition || _mainGridEntity.currentStandingGridTile.gridHeight != _nextTargetTile.gridHeight)
                //{
                if (_mainGridEntity.GetComponent<GridEntityPushAndDragAbility>().currentInteractingGridObject != null)
                    if (_targetTile != null)
                        BeingPushOrDrag();
                //}
                
            }
        }

        private void BeingPushOrDrag()
        {
            Vector3 destination = new Vector3(_targetTile.transform.position.x, _targetTile.transform.position.y + 1, _targetTile.transform.position.z);
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * currentSpeed);
            if (this.transform.position != newPosition)
            {
                this.transform.position = newPosition;
                _isBeingPushedOrDragged = true;
                GameplayManager.Instance.isTurnInProgress = true;
            }
            else
            {
                var gridPos = GridManager.Instance.ConvertWorldPositionToGridPosition(this._currentGridObject);
                this._currentGridObject.gridPosition = new Vector2Int(gridPos.x, gridPos.z);
                this._currentGridObject.gridHeight = gridPos.y;
                //_isBeingPushedOrDragged = false;
                GameplayManager.Instance.isTurnInProgress = false;
                //_nextTargetTile = _mainGridEntity.currentStandingGridTile;
                currentSpeed = 0;
            }
        }
        private void ApplyAcceleration()
        {
            if (currentSpeed < maximumSpeed * maximumSpeedMultiplier)
            {
                currentSpeed = currentSpeed + acceleration * accelerationMultiplier * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, maximumSpeed * maximumSpeedMultiplier);
            }
        }
        public Vector3Int GetValueFromDirection(GridEntityMovementAbility.MovementDirections direction)
        {
            switch (direction)
            {
                case GridEntityMovementAbility.MovementDirections.FORWARD: return Vector3Int.forward;
                case GridEntityMovementAbility.MovementDirections.BACK: return Vector3Int.back;
                case GridEntityMovementAbility.MovementDirections.LEFT: return Vector3Int.left;
                case GridEntityMovementAbility.MovementDirections.RIGHT: return Vector3Int.right;
            }
            return Vector3Int.zero;
        }
        private GridTile GetTargetTile(GridEntityMovementAbility.MovementDirections theDirection)
        {
            Vector3Int targetGridPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight - 1, _currentGridObject.gridPosition.y) + GetValueFromDirection(theDirection);
            Vector3Int blockableGridObjectPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight, _currentGridObject.gridPosition.y) + GetValueFromDirection(theDirection);
            var targetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(targetGridPosition.x, targetGridPosition.z), targetGridPosition.y);
            var blockableGridTiles = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
            var blockableGridEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
            if (targetTile.Count > 0)
            {
                if (blockableGridTiles.Count == 0 && blockableGridEntities.Count == 0)
                    return targetTile[0];
                else if (blockableGridTiles.Count > 0 && blockableGridTiles[0].isBlockable == false)
                    return targetTile[0];
                else if (blockableGridEntities.Count > 0 && blockableGridEntities[0].isBlockable == false)
                    return targetTile[0];
            }
            return null;
        }
        private void LimitMovementDirection()
        {
            foreach (Vector3Int movementDirection in _availableMovementDirection)
            {
                var direction = GridEntityMovementAbility.GetDirectionFromValue(movementDirection, GameplayManager.Instance.globalFrontDirection);
                switch (direction)
                {
                    case GridEntityMovementAbility.MovementDirections.FORWARD:
                        InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.BACK:
                        InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.LEFT:
                        InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.RIGHT:
                        InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirection;
                        break;
                }
            }
        }
        public void DetectMovementDirection(InputButton.States state, KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.W:
                    if (_isBeingPushedOrDragged == true)
                    {
                        _currentMovementDirection = GridEntityMovementAbility.MovementDirections.FORWARD;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        //Turn(_currentMovementDirection);
                    }
                    break;
                case KeyCode.S:
                    if (_isBeingPushedOrDragged == true)
                    {
                        _currentMovementDirection = GridEntityMovementAbility.MovementDirections.BACK;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        //Turn(_currentMovementDirection);
                    }
                    break;
                case KeyCode.A:
                    if (_isBeingPushedOrDragged == true)
                    {
                        _currentMovementDirection = GridEntityMovementAbility.MovementDirections.LEFT;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        //Turn(_currentMovementDirection);
                    }
                    break;
                case KeyCode.D:
                    if (_isBeingPushedOrDragged == true)
                    {
                        _currentMovementDirection = GridEntityMovementAbility.MovementDirections.RIGHT;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        //Turn(_currentMovementDirection);
                    }
                    break;
            }
        }
    }
}

