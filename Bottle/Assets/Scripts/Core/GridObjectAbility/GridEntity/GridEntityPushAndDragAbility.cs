using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragAbility : GridEntityInteractAbility
    {
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
                EventManager.Instance.TriggerEvent("PushAndDragPassiveEvent", new Dictionary<string, object> { { "interactingGridObject", currentInteractingGridObject }, {"isTriggered", true }, { "controllableMainGridEntity", this } });
            }
        }

        private GridEntityMovementAbility gridEntityMovementAbility => GetComponent<GridEntityMovementAbility>();
        protected override void Update()
        {
            base.Update();
            if (currentInteractingGridObject != null && availableMovementDirection.Count == 0)
            {
                Vector3Int interactingGridObjectDirection = new Vector3Int(currentInteractingGridObject.gridPosition.x, (int)currentInteractingGridObject.gridHeight, currentInteractingGridObject.gridPosition.y);
                Vector3Int currentGridObjectDirection = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight, _currentGridObject.gridPosition.y);
                Vector3Int direction = interactingGridObjectDirection - currentGridObjectDirection;
                Vector3Int oppositeDirection = direction * -1;
                List<Vector3Int> directionList = new List<Vector3Int> { direction, oppositeDirection };
                availableMovementDirection = directionList;
            }
            else if (currentInteractingGridObject == null && availableMovementDirection.Count > 0)
            {
                ResetToDefault(availableMovementDirection);
            }
        }
        protected override void Start()
        {
            base.Start();
            EventManager.Instance.StartListening("PushAndDragEvent", LimitMovementDirection);
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
                        InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.BACK:
                        InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.LEFT:
                        InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.RIGHT:
                        InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler -= gridEntityMovementAbility.DetectMovementDirection;
                        break;
                }
            }
        }
        private void ResetToDefault(List<Vector3Int> availableMovementDirectionList)
        {
            foreach (Vector3Int movementDirection in availableMovementDirectionList)
            {
                var direction = GridEntityMovementAbility.GetDirectionFromValue(movementDirection, GameplayManager.Instance.globalFrontDirection);
                switch (direction)
                {
                    case GridEntityMovementAbility.MovementDirections.FORWARD:
                        InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.BACK:
                        InputManager.Instance.buttonStates[KeyCode.A].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.D].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.LEFT:
                        InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        break;
                    case GridEntityMovementAbility.MovementDirections.RIGHT:
                        InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler += gridEntityMovementAbility.DetectMovementDirection;
                        break;
                }
            }
            availableMovementDirectionList.Clear();
        }
    }
}
