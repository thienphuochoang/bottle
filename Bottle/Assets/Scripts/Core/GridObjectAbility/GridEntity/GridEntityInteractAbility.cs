using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using System;
using Sirenix.Serialization;
using Bottle.Core.PathSystem;
namespace Bottle.Core.GridObjectAbility
{
    [CreateAssetMenu(fileName = "GridEntityInteractAbility", menuName = "Bottle/Ability/[Grid Entity] Interact Ability", order = 1)]
    [InlineEditor]
    public class GridEntityInteractAbility : GridObjectAbility<GridEntity>
    {
        public GridObjectAbilitySettings _gridEntityInteractAbilitySettings;
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
            _gridEntityInteractAbilitySettings.OnEnable();
        }

        //public override void AbilityEnable()
        //{

        //}

        private GridEntity GetOppositeGridObject()
        {
            Vector3Int convertedForwardVector = new Vector3Int((int)_currentGridObject.transform.forward.x, (int)_currentGridObject.transform.forward.y, (int)_currentGridObject.transform.forward.z);
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
            if (GameplayManager.Instance.isTurnInProgress == true) return;
            if (currentInteractingGridObject == null)
                currentInteractingGridObject = GetOppositeGridObject();
            else
                currentInteractingGridObject = null;
        }
    }
}