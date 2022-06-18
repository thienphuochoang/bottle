using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectAbility;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityInteractAbility : GridEntityAbility
    {
        [BoxGroup("Interact Ability Settings")]
        [Tooltip("The current Grid Entity which this grid object is interacting")]
        [ShowInInspector]
        private GridEntity _currentInteractingGridObject;
        private GridEntityInteractAbilitySettings _settings;
        public GridEntity currentInteractingGridObject
        {
            get => _currentInteractingGridObject;
            set
            {
                if (_currentInteractingGridObject == value) return;
                _currentInteractingGridObject = value;
            }
        }
        public override void AbilityOnAwake()
        {
            _settings = gridEntityAbilitySettings as GridEntityInteractAbilitySettings;
        }
        public override void AbilityOnEnable()
        {

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
            Vector3Int convertedForwardVector = new Vector3Int((int)currenGridEntity.transform.forward.x, (int)currenGridEntity.transform.forward.y, (int)currenGridEntity.transform.forward.z);
            Vector3Int oppositeGridObjectPosition = new Vector3Int(currenGridEntity.gridPosition.x, (int)currenGridEntity.gridHeight, currenGridEntity.gridPosition.y) + convertedForwardVector;
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