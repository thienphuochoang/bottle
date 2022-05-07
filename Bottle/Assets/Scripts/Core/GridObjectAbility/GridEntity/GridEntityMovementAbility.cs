using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityMovementAbility : GridObjectAbility<GridEntity>
    {
        [ShowInInspector]
        private Dictionary<KeyCode, InputButton> _movementButtonStates => InputManager.Instance.buttonStates;
        public enum MovementDirections { NONE, FORWARD, BACK , LEFT, RIGHT};
        [ShowInInspector]
        private MovementDirections _currentMovementDirection;

        private void DetectMovementDirection(InputButton.States state, KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.W:
                    _currentMovementDirection = MovementDirections.FORWARD;
                    Move();
                    break;
            }
            
        }

        public Vector3Int GetValueFromDirection(MovementDirections direction)
        {
            switch (direction)
            {
                case MovementDirections.FORWARD: return Vector3Int.forward;
                case MovementDirections.BACK: return Vector3Int.back;
                case MovementDirections.LEFT: return Vector3Int.left;
                case MovementDirections.RIGHT: return Vector3Int.right;
            }
            return Vector3Int.zero;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();
            _movementButtonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
        }

        protected override void Update()
        {
            base.Update();
            if (_movementButtonStates[KeyCode.W].currentState == InputButton.States.BUTTON_DOWN)
            {
            }
        }
        private void Move()
        {
            Vector3Int targetGridPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight - 1, _currentGridObject.gridPosition.y) + GetValueFromDirection(_currentMovementDirection);
            var targetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(targetGridPosition.x, targetGridPosition.z), targetGridPosition.y);
            Debug.Log(targetTile);
        }
    }
}
