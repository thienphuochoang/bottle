using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using Bottle.Core.PathSystem;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityMovementAbility : GridEntityAbility
    {
        private GridEntityMovementAbilitySettings _settings;
        [BoxGroup("Uncontrollable Movement Settings", true, true)]
        [SerializeField]
        private PathCreator currentAssignedPathCreator;
        [BoxGroup("Uncontrollable Movement Settings", true, true)]
        //[DisableIf("@currentGridEntity.isControllable", true)]
        [ReadOnly]
        [ShowInInspector]
        private int _currentNode;
        [BoxGroup("Uncontrollable Movement Settings", true, true)]
        //[DisableIf("@currentGridEntity.isControllable", true)]
        [ReadOnly]
        [ShowInInspector]
        private Vector3 _stepPos;
        [BoxGroup("Uncontrollable Movement Settings", true, true)]
        //[DisableIf("@currentGridEntity.isControllable", true)]
        [ReadOnly]
        [SerializeField]
        private int _step = 1;

        [HideInInspector]
        protected static Dictionary<KeyCode, InputButton> _movementButtonStates => InputManager.Instance.buttonStates;
        public enum MovementDirections { NONE, FORWARD, BACK, LEFT, RIGHT };
        [BoxGroup("Movement Settings", true, true)]
        [ReadOnly]
        [ShowInInspector]
        private MovementDirections _currentMovementDirection = MovementDirections.NONE;
        [BoxGroup("Movement Settings", true, true)]
        [ShowInInspector]
        [ReadOnly]
        private GridTile _targetTile;
        [BoxGroup("Movement Settings", true, true)]
        [ReadOnly]
        [SerializeField]
        public bool _isMoving = false;


        [BoxGroup("Acceleration Settings", true, true)]
        [SerializeField]
        [ReadOnly]
        private float currentSpeed;


        public override void AbilityOnAwake()
        {
            _settings = gridEntityAbilitySettings as GridEntityMovementAbilitySettings;
            _step = 1;
        }

        public void DetectMovementDirection(InputButton.States state, KeyCode keyCode)
        {

            switch (keyCode)
            {
                case KeyCode.W:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.FORWARD;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        Turn(_currentMovementDirection, GameplayManager.Instance.globalFrontDirection);
                    }
                    break;
                case KeyCode.S:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.BACK;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        Turn(_currentMovementDirection, GameplayManager.Instance.globalFrontDirection);
                    }
                    break;
                case KeyCode.A:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.LEFT;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        Turn(_currentMovementDirection, GameplayManager.Instance.globalFrontDirection);
                    }
                    break;
                case KeyCode.D:
                    if (_isMoving == false)
                    {
                        _currentMovementDirection = MovementDirections.RIGHT;
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        Turn(_currentMovementDirection, GameplayManager.Instance.globalFrontDirection);
                    }
                    break;
            }
        }

        private void DetectMovementDirectionFromPath(InputButton.States state, KeyCode keyCode)
        {
            if (currentAssignedPathCreator != null)
            {
                if (_currentNode != currentAssignedPathCreator.nodes.Count - 1)
                {
                    if (_isMoving == false)
                    {
                        int nextNode = _currentNode + 1;
                        Vector3 currentNodeWorldSpacePos = currentAssignedPathCreator.transform.TransformPoint(currentAssignedPathCreator.nodes[_currentNode]);
                        Vector3 nextNodeWorldSpacePos = currentAssignedPathCreator.transform.TransformPoint(currentAssignedPathCreator.nodes[nextNode]);
                        Vector3 direction = nextNodeWorldSpacePos - currentNodeWorldSpacePos;
                        _stepPos = CalculateStepPosition(currentNodeWorldSpacePos, nextNodeWorldSpacePos, _step);
                        Vector3Int stepPosGridPosition = GridManager.Instance.ConvertWorldPositionToGridPosition(_stepPos, currentGridEntity.pivotOffset.y);
                        Vector3Int rampGridPosition = new Vector3Int(stepPosGridPosition.x, stepPosGridPosition.y - 2, stepPosGridPosition.z);
                        List<GridTile> checkedRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(rampGridPosition.x, rampGridPosition.z), rampGridPosition.y);
                        if (checkedRampTile.Count > 0)
                        {
                            if (direction.x > 1)
                                direction.x = 1;
                            else if (direction.x == 1)
                                direction.x = 0;
                            if (direction.y > 1)
                                direction.y = 1;
                            else if (direction.y == 1)
                                direction.y = 0;
                            if (direction.z > 1)
                                direction.z = 1;
                            else if (direction.z == 1)
                                direction.z = 0;
                        }
                        _currentMovementDirection = GetDirectionFromValue(direction, GameplayManager.Instance.globalFrontDirection);
                        _targetTile = GetTargetTile(_currentMovementDirection);
                        if (_targetTile != null && _targetTile.isStandable == true)
                        {
                            Turn(_currentMovementDirection, GameplayManager.Instance.globalFrontDirection);
                            Vector3Int stepPosGridData = GridManager.Instance.ConvertWorldPositionToGridPosition(_stepPos, currentGridEntity.pivotOffset.y);
                            Vector3Int nextNodeGridData = GridManager.Instance.ConvertWorldPositionToGridPosition(nextNodeWorldSpacePos, currentGridEntity.pivotOffset.y);
                            if (stepPosGridData == nextNodeGridData || checkedRampTile.Count > 0)
                            {
                                _step = 1;
                                _currentNode = _currentNode + 1;
                            }
                            else
                            {
                                _step = _step + 1;
                            }
                        }
                    }
                }
            }
        }

        public static Vector3Int GetValueFromDirection(MovementDirections direction, GameplayManager.GlobalDirection globalDirection)
        {
            switch (globalDirection)
            {
                case GameplayManager.GlobalDirection.POSITIVE_Z:
                    if (direction == MovementDirections.FORWARD)
                        return Vector3Int.forward;
                    else if (direction == MovementDirections.BACK)
                        return Vector3Int.back;
                    else if (direction == MovementDirections.LEFT)
                        return Vector3Int.left;
                    else if (direction == MovementDirections.RIGHT)
                        return Vector3Int.right;
                    break;
                case GameplayManager.GlobalDirection.NEGATIVE_Z:
                    if (direction == MovementDirections.FORWARD)
                        return Vector3Int.back;
                    else if (direction == MovementDirections.BACK)
                        return Vector3Int.forward;
                    else if (direction == MovementDirections.LEFT)
                        return Vector3Int.right;
                    else if (direction == MovementDirections.RIGHT)
                        return Vector3Int.left;
                    break;
                case GameplayManager.GlobalDirection.POSITIVE_X:
                    if (direction == MovementDirections.FORWARD)
                        return Vector3Int.right;
                    else if (direction == MovementDirections.BACK)
                        return Vector3Int.left;
                    else if (direction == MovementDirections.LEFT)
                        return Vector3Int.forward;
                    else if (direction == MovementDirections.RIGHT)
                        return Vector3Int.back;
                    break;
                case GameplayManager.GlobalDirection.NEGATIVE_X:
                    if (direction == MovementDirections.FORWARD)
                        return Vector3Int.left;
                    else if (direction == MovementDirections.BACK)
                        return Vector3Int.right;
                    else if (direction == MovementDirections.LEFT)
                        return Vector3Int.back;
                    else if (direction == MovementDirections.RIGHT)
                        return Vector3Int.forward;
                    break;
            }
            return Vector3Int.zero;
        }

        public static MovementDirections GetDirectionFromValue(Vector3 direction, GameplayManager.GlobalDirection globalDirection)
        {
            switch (globalDirection)
            {
                case GameplayManager.GlobalDirection.POSITIVE_Z:
                    if (direction.normalized == Vector3Int.forward)
                        return MovementDirections.FORWARD;
                    else if (direction.normalized == Vector3Int.back)
                        return MovementDirections.BACK;
                    else if (direction.normalized == Vector3Int.left)
                        return MovementDirections.LEFT;
                    else if (direction.normalized == Vector3Int.right)
                        return MovementDirections.RIGHT;
                    break;
                case GameplayManager.GlobalDirection.NEGATIVE_Z:
                    if (direction.normalized == Vector3Int.forward)
                        return MovementDirections.BACK;
                    else if (direction.normalized == Vector3Int.back)
                        return MovementDirections.FORWARD;
                    else if (direction.normalized == Vector3Int.left)
                        return MovementDirections.RIGHT;
                    else if (direction.normalized == Vector3Int.right)
                        return MovementDirections.LEFT;
                    break;
                case GameplayManager.GlobalDirection.POSITIVE_X:
                    if (direction.normalized == Vector3Int.forward)
                        return MovementDirections.LEFT;
                    else if (direction.normalized == Vector3Int.back)
                        return MovementDirections.RIGHT;
                    else if (direction.normalized == Vector3Int.left)
                        return MovementDirections.BACK;
                    else if (direction.normalized == Vector3Int.right)
                        return MovementDirections.FORWARD;
                    break;
                case GameplayManager.GlobalDirection.NEGATIVE_X:
                    if (direction.normalized == Vector3Int.forward)
                        return MovementDirections.RIGHT;
                    else if (direction.normalized == Vector3Int.back)
                        return MovementDirections.LEFT;
                    else if (direction.normalized == Vector3Int.left)
                        return MovementDirections.FORWARD;
                    else if (direction.normalized == Vector3Int.right)
                        return MovementDirections.BACK;
                    break;
            }
            return MovementDirections.NONE;
        }
        public override void AbilityStart()
        {
            EventManager.Instance.StartListening("MovementDirectionDetectionEventsChanged", AssignMovementDirectionDetectionEvents);
            if (currentGridEntity.isControllable)
            {
                _movementButtonStates[KeyCode.W].ButtonDownHandler -= DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.S].ButtonDownHandler -= DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.A].ButtonDownHandler -= DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.D].ButtonDownHandler -= DetectMovementDirectionFromPath;

                _movementButtonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirection;
            }
            else
            {
                _movementButtonStates[KeyCode.W].ButtonDownHandler -= DetectMovementDirection;
                _movementButtonStates[KeyCode.S].ButtonDownHandler -= DetectMovementDirection;
                _movementButtonStates[KeyCode.A].ButtonDownHandler -= DetectMovementDirection;
                _movementButtonStates[KeyCode.D].ButtonDownHandler -= DetectMovementDirection;

                _movementButtonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirectionFromPath;
            }
        }

        public void AssignMovementDirectionDetectionEvents(Dictionary<string, object> message)
        {
            if ((bool)message["IsControllable"] && (GridEntity)message["GridEntity"] == currentGridEntity)
            {
                currentAssignedPathCreator = null;
                _movementButtonStates[KeyCode.W].ButtonDownHandler -= DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.S].ButtonDownHandler -= DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.A].ButtonDownHandler -= DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.D].ButtonDownHandler -= DetectMovementDirectionFromPath;

                _movementButtonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirection;
                _movementButtonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirection;
            }
            else if ((bool)message["IsControllable"] == false && (GridEntity)message["GridEntity"] == currentGridEntity)
            {
                _movementButtonStates[KeyCode.W].ButtonDownHandler -= DetectMovementDirection;
                _movementButtonStates[KeyCode.S].ButtonDownHandler -= DetectMovementDirection;
                _movementButtonStates[KeyCode.A].ButtonDownHandler -= DetectMovementDirection;
                _movementButtonStates[KeyCode.D].ButtonDownHandler -= DetectMovementDirection;

                _movementButtonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirectionFromPath;
                _movementButtonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirectionFromPath;
            }
        }

        public override void AbilityUpdate()
        {
            ApplyAcceleration();
            if (_targetTile != null && _targetTile.isStandable == true)
            {
                Move();
            }
            //if (Input.GetKeyUp(KeyCode.T))
            //{
            //    if (_currentGridEntity.isControllable)
            //    {
            //        _currentGridEntity.isControllable = false;
            //    }
            //    else
            //        _currentGridEntity.isControllable = true;
            //}
        }

        public override void AbilityOnEnable()
        {

        }

        private GridTile GetTargetTile(MovementDirections theDirection)
        {
            Vector3Int targetGridPosition = new Vector3Int(currentGridEntity.gridPosition.x, (int)currentGridEntity.gridHeight - 1, currentGridEntity.gridPosition.y) + GetValueFromDirection(theDirection, GameplayManager.Instance.globalFrontDirection);
            //Vector3Int blockableGridObjectPosition = new Vector3Int(_currentGridEntity.gridPosition.x, (int)_currentGridEntity.gridHeight, _currentGridEntity.gridPosition.y) + GetValueFromDirection(theDirection, GameplayManager.Instance.globalFrontDirection);
            List<GridTile> targetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(targetGridPosition.x, targetGridPosition.z), targetGridPosition.y);
            //var blockableGridTiles = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
            //var blockableGridEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
            var (blockableGridTiles, blockableGridEntities) = GetBlockableGridObjects(currentGridEntity, theDirection);
            if (targetTile.Count > 0)
            {
                if (blockableGridTiles.Count == 0 && blockableGridEntities.Count == 0)
                {
                    if (targetTile[0].isARamp)
                    {
                        Vector3Int nextTargetGridPosition = new Vector3Int(currentGridEntity.gridPosition.x, (int)currentGridEntity.gridHeight - 1, currentGridEntity.gridPosition.y) + GetValueFromDirection(theDirection, GameplayManager.Instance.globalFrontDirection) * 2;
                        var nextTargetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(nextTargetGridPosition.x, nextTargetGridPosition.z), nextTargetGridPosition.y + 1);
                        if (nextTargetTile.Count > 0)
                            return nextTargetTile[0];
                    }
                    return targetTile[0];
                }
            }
            else
            {
                Vector3Int targetRampPosition = new Vector3Int(currentGridEntity.gridPosition.x, (int)currentGridEntity.gridHeight - 2, currentGridEntity.gridPosition.y) + GetValueFromDirection(theDirection, GameplayManager.Instance.globalFrontDirection);
                var targetRampTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(targetRampPosition.x, targetRampPosition.z), targetRampPosition.y);
                if (targetRampTile.Count > 0)
                {
                    if (targetRampTile[0].isARamp)
                    {
                        Vector3Int nextTargetGridPosition = new Vector3Int(currentGridEntity.gridPosition.x, (int)currentGridEntity.gridHeight - 2, currentGridEntity.gridPosition.y) + GetValueFromDirection(theDirection, GameplayManager.Instance.globalFrontDirection) * 2;
                        var nextTargetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(nextTargetGridPosition.x, nextTargetGridPosition.z), nextTargetGridPosition.y);
                        if (nextTargetTile.Count > 0)
                            return nextTargetTile[0];
                    }
                }
            }
            return null;
        }

        public static (List<GridTile> gridTiles, List<GridEntity> gridEntities) GetBlockableGridObjects(GridEntity theMovingGridEntity, MovementDirections movementDirection)
        {
            Vector3Int blockableGridObjectPosition = new Vector3Int(theMovingGridEntity.gridPosition.x, (int)theMovingGridEntity.gridHeight, theMovingGridEntity.gridPosition.y) + GetValueFromDirection(movementDirection, GameplayManager.Instance.globalFrontDirection);
            List<GridTile> blockableGridTiles = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
            List<GridEntity> blockableGridEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
            List<GridTile> blockedGridTiles = new List<GridTile>();
            List<GridEntity> blockedGridEntities = new List<GridEntity>();
            if (blockableGridTiles.Count > 0)
            {
                foreach (GridTile blockableGridTile in blockableGridTiles)
                {
                    if (blockableGridTile.isBlockable)
                    {
                        blockedGridTiles.Add(blockableGridTile);
                    }
                }
            }
            if (blockableGridEntities.Count > 0)
            {
                foreach (GridEntity blockableGridEntity in blockableGridEntities)
                {
                    if (blockableGridEntity.isBlockable)
                    {
                        blockedGridEntities.Add(blockableGridEntity);
                    }
                }
            }
            return (blockedGridTiles, blockedGridEntities);
        }
        private void ApplyAcceleration()
        {
            if (currentSpeed < _settings.maximumSpeed * _settings.maximumSpeedMultiplier)
            {
                currentSpeed = currentSpeed + _settings.acceleration * _settings.accelerationMultiplier * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, _settings.maximumSpeed * _settings.maximumSpeedMultiplier);
            }
        }
        private void Move()
        {
            Vector3 destination = new Vector3(_targetTile.transform.position.x, _targetTile.transform.position.y + 1, _targetTile.transform.position.z);
            Vector3 newPosition = Vector3.MoveTowards(currentGridEntity.transform.position, destination, Time.deltaTime * currentSpeed);
            if (currentGridEntity.transform.position != newPosition)
            {
                currentGridEntity.transform.position = newPosition;
                _isMoving = true;
                GameplayManager.Instance.isTurnInProgress = true;
            }
            else
            {
                var gridPos = GridManager.Instance.ConvertWorldPositionToGridPosition(currentGridEntity);
                currentGridEntity.gridPosition = new Vector2Int(gridPos.x, gridPos.z);
                currentGridEntity.gridHeight = gridPos.y;
                _currentMovementDirection = MovementDirections.NONE;
                _isMoving = false;
                GameplayManager.Instance.isTurnInProgress = false;
                _targetTile = null;
                currentSpeed = 0;
            }
        }
        private Vector3 CalculateStepPosition(Vector3 currentNodeWorldSpacePos, Vector3 nextNodeWorldSpacePos, int distance)
        {
            Vector3 direction = (nextNodeWorldSpacePos - currentNodeWorldSpacePos).normalized;
            Vector3 stepPos = currentNodeWorldSpacePos + direction * distance;
            return stepPos;
        }
        private void Turn(MovementDirections targetDirection, GameplayManager.GlobalDirection globalDirection)
        {
            Vector3 currentChildRotationEulerAngle = currentGridEntity.transform.rotation.eulerAngles;
            switch (globalDirection)
            {
                case GameplayManager.GlobalDirection.POSITIVE_Z:
                    if (targetDirection == MovementDirections.FORWARD)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 0 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.BACK)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 180 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.LEFT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, -90 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.RIGHT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 90 - currentChildRotationEulerAngle.y);
                    }
                    break;
                case GameplayManager.GlobalDirection.NEGATIVE_Z:
                    if (targetDirection == MovementDirections.FORWARD)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 180 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.BACK)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 0 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.LEFT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 90 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.RIGHT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, -90 - currentChildRotationEulerAngle.y);
                    }
                    break;
                case GameplayManager.GlobalDirection.POSITIVE_X:
                    if (targetDirection == MovementDirections.FORWARD)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 90 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.BACK)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, -90 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.LEFT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 0 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.RIGHT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 180 - currentChildRotationEulerAngle.y);
                    }
                    break;
                case GameplayManager.GlobalDirection.NEGATIVE_X:
                    if (targetDirection == MovementDirections.FORWARD)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, -90 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.BACK)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 90 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.LEFT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 180 - currentChildRotationEulerAngle.y);
                    }
                    else if (targetDirection == MovementDirections.RIGHT)
                    {
                        currentGridEntity.transform.RotateAround(currentGridEntity.transform.position, currentGridEntity.transform.parent.up, 0 - currentChildRotationEulerAngle.y);
                    }
                    break;
            }
        }
    }
}
