using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using Bottle.Core.PathSystem;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragPassiveAbility : GridEntityPassiveAbility
    {
        //private GridEntityPushAndDragAbilitySettings _pushAndDragSettings;
        private GridEntityMovementAbilitySettings _movementSettings;
        private PathCreator pathCreatorGameObject;

        //private bool _isBeingPushedOrDragged => isInteracted;
        [SerializeField]
        public bool isBeingPushedOrDragged
        {
            get => isTriggered;
            set
            {
                if (isTriggered == value) return;
                isTriggered = value;
                EventManager.Instance.TriggerEvent("PushAndDragPassiveEvent", null);
            }
        }
        private GridEntityMovementAbility gridEntityMovementAbilityRef;
        

        // protected override void Start()
        // {
        //     base.Start();
        //     EventManager.Instance.StartListening("PushAndDragPassiveEvent", BeginBeingPushedOrDragged);
        //     //InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirectionFromPath;
        //     //InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirectionFromPath;
        //     //InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirectionFromPath;
        //     //InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirectionFromPath;
        // }
        //
        private void BeginBeingPushedOrDragged(Dictionary<string, object> message)
        {
            Debug.Log(currentGridEntity);
            Debug.Log("Test event");
            // bool isTriggeredValue = (bool)message["isTriggered"];
            // GridEntity currentInteractingObject = (GridEntity)message["interactingGridObject"];
            // _mainGridEntity = (GridEntityPushAndDragAbility)message["controllableMainGridEntity"];
            // if (currentInteractingObject == _currentGridObject)
            // {
            //     isPassiveAbilityTriggered = isTriggeredValue;
            // }
        }
        //
        // protected override void Update()
        // {
        //     base.Update();
        //     ApplyAcceleration();
        //     if (isPassiveAbilityTriggered)
        //     {
        //         //GameplayManager.Instance.isTurnInProgress = true;
        //         isPassiveAbilityTriggered = false;
        //         _availableMovementDirection = _mainGridEntity.availableMovementDirection;
        //         _isBeingPushedOrDragged = true;
        //         //_currentGridObject.isBlockable = false;
        //         LimitMovementDirection();
        //         //_nextTargetTile = _mainGridEntity.currentStandingGridTile;
        //     }
        //     if ( isPassiveAbilityTriggered == false && _mainGridEntity != null && _isBeingPushedOrDragged == true)
        //     {
        //         //if (_mainGridEntity.currentStandingGridTile.gridPosition != _nextTargetTile.gridPosition || _mainGridEntity.currentStandingGridTile.gridHeight != _nextTargetTile.gridHeight)
        //         //{
        //         if (_mainGridEntity.GetComponent<GridEntityPushAndDragAbility>().currentInteractingGridObject != null)
        //             if (_targetTile != null)
        //                 BeingPushOrDrag();
        //         //}
        //         
        //     }
        // }
        //
        // private void BeingPushOrDrag()
        // {
        //     Vector3 destination = new Vector3(_targetTile.transform.position.x, _targetTile.transform.position.y + 1, _targetTile.transform.position.z);
        //     Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * currentSpeed);
        //     if (this.transform.position != newPosition)
        //     {
        //         this.transform.position = newPosition;
        //         _isBeingPushedOrDragged = true;
        //         GameplayManager.Instance.isTurnInProgress = true;
        //     }
        //     else
        //     {
        //         var gridPos = GridManager.Instance.ConvertWorldPositionToGridPosition(this._currentGridObject);
        //         this._currentGridObject.gridPosition = new Vector2Int(gridPos.x, gridPos.z);
        //         this._currentGridObject.gridHeight = gridPos.y;
        //         //_isBeingPushedOrDragged = false;
        //         GameplayManager.Instance.isTurnInProgress = false;
        //         //_nextTargetTile = _mainGridEntity.currentStandingGridTile;
        //         currentSpeed = 0;
        //     }
        // }
        // private void ApplyAcceleration()
        // {
        //     if (currentSpeed < maximumSpeed * maximumSpeedMultiplier)
        //     {
        //         currentSpeed = currentSpeed + acceleration * accelerationMultiplier * Time.deltaTime;
        //         currentSpeed = Mathf.Clamp(currentSpeed, 0f, maximumSpeed * maximumSpeedMultiplier);
        //     }
        // }
        // public Vector3Int GetValueFromDirection(GridEntityMovementAbility.MovementDirections direction)
        // {
        //     switch (direction)
        //     {
        //         case GridEntityMovementAbility.MovementDirections.FORWARD: return Vector3Int.forward;
        //         case GridEntityMovementAbility.MovementDirections.BACK: return Vector3Int.back;
        //         case GridEntityMovementAbility.MovementDirections.LEFT: return Vector3Int.left;
        //         case GridEntityMovementAbility.MovementDirections.RIGHT: return Vector3Int.right;
        //     }
        //     return Vector3Int.zero;
        // }
        // private GridTile GetTargetTile(GridEntityMovementAbility.MovementDirections theDirection)
        // {
        //     Vector3Int targetGridPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight - 1, _currentGridObject.gridPosition.y) + GetValueFromDirection(theDirection);
        //     Vector3Int blockableGridObjectPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight, _currentGridObject.gridPosition.y) + GetValueFromDirection(theDirection);
        //     var targetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(targetGridPosition.x, targetGridPosition.z), targetGridPosition.y);
        //     var blockableGridTiles = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
        //     var blockableGridEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(blockableGridObjectPosition.x, blockableGridObjectPosition.z), blockableGridObjectPosition.y);
        //     if (targetTile.Count > 0)
        //     {
        //         if (blockableGridTiles.Count == 0 && blockableGridEntities.Count == 0)
        //             return targetTile[0];
        //         else if (blockableGridTiles.Count > 0 && blockableGridTiles[0].isBlockable == false)
        //             return targetTile[0];
        //         else if (blockableGridEntities.Count > 0 && blockableGridEntities[0].isBlockable == false)
        //             return targetTile[0];
        //     }
        //     return null;
        // }
        // private void LimitMovementDirection()
        // {
        //     foreach (Vector3Int movementDirection in _availableMovementDirection)
        //     {
        //         var direction = GridEntityMovementAbility.GetDirectionFromValue(movementDirection, GameplayManager.Instance.globalFrontDirection);
        //         switch (direction)
        //         {
        //             case GridEntityMovementAbility.MovementDirections.FORWARD:
        //                 InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
        //                 InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirection;
        //                 break;
        //             case GridEntityMovementAbility.MovementDirections.BACK:
        //                 InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
        //                 InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += DetectMovementDirection;
        //                 break;
        //             case GridEntityMovementAbility.MovementDirections.LEFT:
        //                 InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirection;
        //                 InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirection;
        //                 break;
        //             case GridEntityMovementAbility.MovementDirections.RIGHT:
        //                 InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += DetectMovementDirection;
        //                 InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += DetectMovementDirection;
        //                 break;
        //         }
        //     }
        // }
        // public void DetectMovementDirection(InputButton.States state, KeyCode keyCode)
        // {
        //     switch (keyCode)
        //     {
        //         case KeyCode.W:
        //             if (_isBeingPushedOrDragged == true)
        //             {
        //                 _currentMovementDirection = GridEntityMovementAbility.MovementDirections.FORWARD;
        //                 _targetTile = GetTargetTile(_currentMovementDirection);
        //                 //Turn(_currentMovementDirection);
        //             }
        //             break;
        //         case KeyCode.S:
        //             if (_isBeingPushedOrDragged == true)
        //             {
        //                 _currentMovementDirection = GridEntityMovementAbility.MovementDirections.BACK;
        //                 _targetTile = GetTargetTile(_currentMovementDirection);
        //                 //Turn(_currentMovementDirection);
        //             }
        //             break;
        //         case KeyCode.A:
        //             if (_isBeingPushedOrDragged == true)
        //             {
        //                 _currentMovementDirection = GridEntityMovementAbility.MovementDirections.LEFT;
        //                 _targetTile = GetTargetTile(_currentMovementDirection);
        //                 //Turn(_currentMovementDirection);
        //             }
        //             break;
        //         case KeyCode.D:
        //             if (_isBeingPushedOrDragged == true)
        //             {
        //                 _currentMovementDirection = GridEntityMovementAbility.MovementDirections.RIGHT;
        //                 _targetTile = GetTargetTile(_currentMovementDirection);
        //                 //Turn(_currentMovementDirection);
        //             }
        //             break;
        //     }
        // }

        public override void AbilityOnAwake()
        {
            // _pushAndDragSettings = (GridEntityPushAndDragAbilitySettings)gridEntityAbilitySettings.Find(entityAbilitySettings =>
            //     entityAbilitySettings.GetType() == typeof(GridEntityPushAndDragAbilitySettings));
            gridEntityMovementAbilityRef = (GridEntityMovementAbility)GridEntityAbility.GetGridEntityAbility<GridEntityMovementAbility>(currentGridEntity);
        }

        public override void AbilityOnEnable()
        {
        }

        public override void AbilityStart()
        {
            EventManager.Instance.StartListening("PushAndDragPassiveEvent", BeginBeingPushedOrDragged);
        }

        public override void AbilityUpdate()
        {
            if (isBeingPushedOrDragged)
            {
                if (gridEntityMovementAbilityRef.currentAssignedPathCreator != null) 
                {
                    gridEntityMovementAbilityRef.currentAssignedPathCreator.nodes.Clear();
                    gridEntityMovementAbilityRef.currentAssignedPathCreator.nodes = new List<Vector3>();
                    Vector3 localCurrentStandingGridTileNodePos =
                        gridEntityMovementAbilityRef.currentAssignedPathCreator.ConvertGridPosToNodeLocalPos(
                            currentGridEntity.currentStandingGridTile);
                    localCurrentStandingGridTileNodePos.y += 1;
                    Vector3 localTriggerCurrentStandingGridTileNodePos = gridEntityMovementAbilityRef.currentAssignedPathCreator.ConvertGridPosToNodeLocalPos(
                        triggerGridEntity.currentStandingGridTile);
                    localTriggerCurrentStandingGridTileNodePos.y += 1;
                    gridEntityMovementAbilityRef.currentAssignedPathCreator.nodes.Insert(0, localCurrentStandingGridTileNodePos);
                    gridEntityMovementAbilityRef.currentAssignedPathCreator.nodes.Insert(1, localTriggerCurrentStandingGridTileNodePos);
                }
                else
                {
                    GameObject pathGameObject = new GameObject(currentGridEntity.name + "_Path"); 
                    pathGameObject.transform.position = Vector3.zero;
                    pathGameObject.AddComponent<PathCreator>();
                    pathCreatorGameObject = pathGameObject.GetComponent<PathCreator>();
                    gridEntityMovementAbilityRef.currentAssignedPathCreator = pathCreatorGameObject;
                }
            }
            //Debug.Log(isInteracted);
            //Debug.Log(isBeingPushedOrDragged);
        }
    }
}

