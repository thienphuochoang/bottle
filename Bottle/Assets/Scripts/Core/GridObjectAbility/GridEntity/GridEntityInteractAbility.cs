using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityInteractAbility : GridObjectAbility<GridEntity>
    {
        private static Dictionary<KeyCode, InputButton> interactButtonStates => InputManager.Instance.buttonStates;
        [SerializeField]
        private GridEntity _interactiveGridObject;
        protected override void Start()
        {
            interactButtonStates[KeyCode.E].ButtonDownHandler += Interact;
        }

        protected override void Update()
        {
            
        }

        private GridEntity GetOppositeGridObject()
        {
            Vector3Int targetGridPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight - 1, _currentGridObject.gridPosition.y) + Vector3Int.forward;
            Vector3Int blockableGridObjectPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight, _currentGridObject.gridPosition.y) + Vector3Int.forward;
            var targetEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(targetGridPosition.x, targetGridPosition.z), targetGridPosition.y);
            if (targetEntities.Count > 0)
            {
                return targetEntities[0];
            }
            return null;
        }

        private void Interact(InputButton.States state, KeyCode keyCode)
        {
            _interactiveGridObject = GetOppositeGridObject();
        }
    }

}