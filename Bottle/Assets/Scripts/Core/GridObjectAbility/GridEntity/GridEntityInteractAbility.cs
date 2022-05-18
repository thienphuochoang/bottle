using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityInteractAbility : GridObjectAbility<GridEntity>
    {
        protected static Dictionary<KeyCode, InputButton> interactButtonStates => InputManager.Instance.buttonStates;
        [BoxGroup("Interact ability Settings")]
        [Tooltip("The current Grid Entity that this grid object is interacting")]
        [ShowInInspector]
        private GridEntity _currentInteractingGridObject;
        public GridEntity currentInteractingGridObject
        {
            get => _currentInteractingGridObject;
            set
            {
                if (_currentInteractingGridObject == value) return;
                _currentInteractingGridObject = value;
            }
        }
        protected override void Start()
        {
            interactButtonStates[KeyCode.E].ButtonDownHandler += Interact;
        }

        protected override void Update()
        {
        }

        private GridEntity GetOppositeGridObject()
        {
            Vector3Int oppositeGridObjectPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight, _currentGridObject.gridPosition.y) + Vector3Int.FloorToInt(_currentGridObject.ConvertFacingDirectionToValue(_currentGridObject.currentFacingDirection));
            var targetEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(oppositeGridObjectPosition.x, oppositeGridObjectPosition.z), oppositeGridObjectPosition.y);
            Debug.Log(oppositeGridObjectPosition);
            if (targetEntities.Count > 0)
            {
                return targetEntities[0];
            }
            return null;
        }

        private void Interact(InputButton.States state, KeyCode keyCode)
        {
            currentInteractingGridObject = GetOppositeGridObject();
        }
    }

}