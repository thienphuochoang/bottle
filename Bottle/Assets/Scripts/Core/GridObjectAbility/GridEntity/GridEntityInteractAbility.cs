using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using System;
using Sirenix.Serialization;
namespace Bottle.Core.GridObjectAbility
{
    [CreateAssetMenu(fileName = "GridEntityInteractAbility", menuName = "Bottle/Ability/[Grid Entity] Interact Ability", order = 1)]
    [InlineEditor]
    public class GridEntityInteractAbility : GridObjectAbilitySettings<GridEntity>
    {

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

        public override void AbilityStart()
        {
            InputManager.Instance.buttonStates[KeyCode.E].ButtonDownHandler += Interact;
        }
        public override void AbilityUpdate()
        {
            
        }

        private GridEntity GetOppositeGridObject()
        {
            Vector3Int convertedForwardVector = new Vector3Int((int)currentGridObject.transform.forward.x, (int)currentGridObject.transform.forward.y, (int)currentGridObject.transform.forward.z);
            Vector3Int oppositeGridObjectPosition = new Vector3Int(currentGridObject.gridPosition.x, (int)currentGridObject.gridHeight, currentGridObject.gridPosition.y) + convertedForwardVector;
            var targetEntities = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(new Vector2Int(oppositeGridObjectPosition.x, oppositeGridObjectPosition.z), oppositeGridObjectPosition.y);
            if (targetEntities.Count > 0)
            {
                return targetEntities[0];
            }
            return null;
        }

        private void Interact(InputButton.States state, KeyCode keyCode)
        {
            if (GameplayManager.Instance.isTurnInProgress == true) return;
            if (currentInteractingGridObject == null)
                currentInteractingGridObject = GetOppositeGridObject();
            else
                currentInteractingGridObject = null;
        }
    }

}