using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragAbility : GridEntityAbility
    {
        private GridEntityPushAndDragAbilitySettings _settings;
        private GridEntityInteractAbility _gridEntityInteractAbilityRef;
        private GridEntityMovementAbility _gridEntityMovementAbilityRef;
        private GridEntityPushAndDragPassiveAbility _gridEntityPushAndDragPassiveAbilityRef;
        [ShowInInspector]
        [ReadOnly]
        private List<Vector3Int> _availableMovementDirection = new List<Vector3Int>();
        public List<Vector3Int> availableMovementDirection
        {
            get => _availableMovementDirection;
            set
            {
                if (_availableMovementDirection == null) return;
                _availableMovementDirection = value;
                EventManager.Instance.TriggerEvent("PushAndDragEvent", new Dictionary<string, object> { { "availableMovementDirectionList", _availableMovementDirection } });
            }
        }
        public override void AbilityOnAwake()
        {
            _settings = gridEntityAbilitySettings[0] as GridEntityPushAndDragAbilitySettings;
        }
        public override void AbilityOnEnable()
        {
            _availableMovementDirection = new List<Vector3Int>();
            _gridEntityInteractAbilityRef = (GridEntityInteractAbility)gridEntityAbilityController.availableAbilities.Find(entityAbility =>
                entityAbility.gridEntityAbilitySettings.Find(entityAbilitySetting => entityAbilitySetting.GetType() == typeof(GridEntityInteractAbilitySettings)));
            _gridEntityMovementAbilityRef = (GridEntityMovementAbility)gridEntityAbilityController.availableAbilities.Find(entityAbility =>
                entityAbility.gridEntityAbilitySettings.Find(entityAbilitySetting => entityAbilitySetting.GetType() == typeof(GridEntityMovementAbilitySettings)));
        }
        public override void AbilityStart()
        {
            EventManager.Instance.StartListening("PushAndDragEvent", LimitMovementDirection);
        }
        public override void AbilityUpdate()
        {
            if (_gridEntityInteractAbilityRef.currentInteractingGridObject != null && availableMovementDirection.Count == 0)
            {
                if (GridEntityAbility.CheckIfAbilityExist<GridEntityPushAndDragPassiveAbility>(
                        _gridEntityInteractAbilityRef.currentInteractingGridObject))
                {
                    Vector3Int interactingGridObjectDirection = new Vector3Int(_gridEntityInteractAbilityRef.currentInteractingGridObject.gridPosition.x, (int)_gridEntityInteractAbilityRef.currentInteractingGridObject.gridHeight, _gridEntityInteractAbilityRef.currentInteractingGridObject.gridPosition.y);
                    Vector3Int currentGridObjectDirection = new Vector3Int(currentGridEntity.gridPosition.x, (int)currentGridEntity.gridHeight, currentGridEntity.gridPosition.y);
                    Vector3Int direction = interactingGridObjectDirection - currentGridObjectDirection;
                    Vector3Int oppositeDirection = direction * -1;
                    List<Vector3Int> directionList = new List<Vector3Int> { direction, oppositeDirection };
                    availableMovementDirection = directionList;
                }
                return;
            }

            if (availableMovementDirection.Count > 0)
            {
                if (_gridEntityInteractAbilityRef.currentInteractingGridObject != null)
                {
                    
                }
                else
                {
                    ResetToDefault(availableMovementDirection);
                }
            }
        }

        private void ActivatePushAndDragPassiveAbility()
        {
            _gridEntityPushAndDragPassiveAbilityRef = (GridEntityPushAndDragPassiveAbility)GridEntityAbility.GetGridEntityAbility<GridEntityPushAndDragPassiveAbility>(
                _gridEntityInteractAbilityRef.currentInteractingGridObject);
            _gridEntityPushAndDragPassiveAbilityRef.isTriggered = true;
            _gridEntityPushAndDragPassiveAbilityRef.triggerGridEntity = currentGridEntity;
        }
       private void LimitMovementDirection(Dictionary<string,object> message)
       {
           List<Vector3Int> availableMovementDirectionList = (List<Vector3Int>)message["availableMovementDirectionList"];
           foreach (Vector3Int movementDirection in availableMovementDirectionList)
           {
               var direction = GridEntityMovementAbility.GetDirectionFromValue(movementDirection, GameplayManager.Instance.globalFrontDirection);
               switch (direction)
               {
                   case GridEntityMovementAbility.MovementDirections.FORWARD:
                       InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
                   case GridEntityMovementAbility.MovementDirections.BACK:
                       InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
                   case GridEntityMovementAbility.MovementDirections.LEFT:
                       InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
                   case GridEntityMovementAbility.MovementDirections.RIGHT:
                       InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler -= _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
               }
           }
           ActivatePushAndDragPassiveAbility();
       }
       private void ResetToDefault(List<Vector3Int> availableMovementDirectionList)
       {
           foreach (Vector3Int movementDirection in availableMovementDirectionList)
           {
               var direction = GridEntityMovementAbility.GetDirectionFromValue(movementDirection, GameplayManager.Instance.globalFrontDirection);
               switch (direction)
               {
                   case GridEntityMovementAbility.MovementDirections.FORWARD:
                       InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
                   case GridEntityMovementAbility.MovementDirections.BACK:
                       InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
                   case GridEntityMovementAbility.MovementDirections.LEFT:
                       InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
                   case GridEntityMovementAbility.MovementDirections.RIGHT:
                       InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += _gridEntityMovementAbilityRef.DetectMovementDirection;
                       break;
               }
           }
           availableMovementDirectionList.Clear();
           _gridEntityPushAndDragPassiveAbilityRef.isTriggered = false;
           _gridEntityPushAndDragPassiveAbilityRef.triggerGridEntity = null;
       }
    }
}
