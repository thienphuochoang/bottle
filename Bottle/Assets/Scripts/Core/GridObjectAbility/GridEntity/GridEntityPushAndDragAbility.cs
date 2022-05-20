using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragAbility : GridEntityInteractAbility
    {
        private List<Vector3Int> _availableMovementDirection = new List<Vector3Int>();

        private GridEntityMovementAbility gridEntityMovementAbility => GetComponent<GridEntityMovementAbility>();
        protected override void Update()
        {
            base.Update();
            if (currentInteractingGridObject != null && _availableMovementDirection.Count == 0)
            {
                Vector3Int interactingGridObjectDirection = new Vector3Int(currentInteractingGridObject.gridPosition.x, (int)currentInteractingGridObject.gridHeight, currentInteractingGridObject.gridPosition.y);
                Vector3Int currentGridObjectDirection = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight, _currentGridObject.gridPosition.y);
                Vector3Int direction = interactingGridObjectDirection - currentGridObjectDirection;
                Vector3Int oppositeDirection = direction * -1;
                List<Vector3Int> tempList = new List<Vector3Int> { direction, oppositeDirection };
                _availableMovementDirection = tempList;
                LimitMovementDirection();
                //InputManager.Instance.buttonStates[KeyCode.W].ButtonDownHandler -= GetComponent<GridEntityMovementAbility>().DetectMovementDirection;
                //InputManager.Instance.buttonStates[KeyCode.S].ButtonDownHandler -= GetComponent<GridEntityMovementAbility>().DetectMovementDirection;
            }
            else if (currentInteractingGridObject == null && _availableMovementDirection.Count > 0)
            {
                foreach (Vector3Int movementDirection in _availableMovementDirection)
                {
                    var direction = gridEntityMovementAbility.GetDirectionFromValue(movementDirection);
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
                _availableMovementDirection.Clear();
            }
        }
        protected override void Start()
        {
            base.Start();
        }

        private void LimitMovementDirection()
        {
            foreach (Vector3Int movementDirection in _availableMovementDirection)
            {
                var direction = gridEntityMovementAbility.GetDirectionFromValue(movementDirection);
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
    }
}
