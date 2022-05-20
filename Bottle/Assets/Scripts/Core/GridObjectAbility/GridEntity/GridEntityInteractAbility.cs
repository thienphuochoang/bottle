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
            InputManager.Instance.buttonStates[KeyCode.E].ButtonDownHandler += Interact;
        }

        protected override void Update()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private GridEntity GetOppositeGridObject()
        {
            Vector3Int convertedForwardVector = new Vector3Int((int)this.transform.forward.x, (int)this.transform.forward.y, (int)this.transform.forward.z);
            Vector3Int oppositeGridObjectPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight, _currentGridObject.gridPosition.y) + convertedForwardVector;
            var targetEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(oppositeGridObjectPosition.x, oppositeGridObjectPosition.z), oppositeGridObjectPosition.y);
            if (targetEntities.Count > 0)
            {
                return targetEntities[0];
            }
            return null;
        }

        private void Interact(InputButton.States state, KeyCode keyCode)
        {
            if (currentInteractingGridObject == null)
                currentInteractingGridObject = GetOppositeGridObject();
            else
                currentInteractingGridObject = null;
        }
    }

}