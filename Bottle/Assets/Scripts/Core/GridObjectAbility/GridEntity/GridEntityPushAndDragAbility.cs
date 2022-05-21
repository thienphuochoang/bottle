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
                LimitMovementDirection(_availableMovementDirection);
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
                List<Vector3Int> tempList = new List<Vector3Int> { direction, oppositeDirection };
                availableMovementDirection = tempList;
            }
            else if (currentInteractingGridObject == null && availableMovementDirection.Count > 0)
            {
                ResetToDefault(availableMovementDirection);
            }
        }
        protected override void Start()
        {
            base.Start();
            availableMovementDirection = new List<Vector3Int>();
        }

        private void LimitMovementDirection(List<Vector3Int> availableMovementDirectionList)
        {
            foreach (Vector3Int movementDirection in availableMovementDirectionList)
            {
                var direction = GridEntityMovementAbility.GetDirectionFromValue(movementDirection);
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
                var direction = GridEntityMovementAbility.GetDirectionFromValue(movementDirection);
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
