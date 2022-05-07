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
        private MovementDirections _currentMovementDirection = MovementDirections.NONE;
        private GridTile _targetTile;

        private void DetectMovementDirection(InputButton.States state, KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.W:
                    _currentMovementDirection = MovementDirections.FORWARD;
                    _targetTile = GetTargetTile(_currentMovementDirection);
                    if (_targetTile != null)
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
        private GridTile GetTargetTile(MovementDirections theDirection)
        {
            Vector3Int targetGridPosition = new Vector3Int(_currentGridObject.gridPosition.x, (int)_currentGridObject.gridHeight - 1, _currentGridObject.gridPosition.y) + GetValueFromDirection(theDirection);
            var targetTile = GridManager.Instance.GetGridObjectAtPosition<GridTile>(new Vector2Int(targetGridPosition.x, targetGridPosition.z), targetGridPosition.y);
            return targetTile;
        }
        private void Move()
        {
            //this.transform.position = newPosition;
        }
        private IEnumerator StepMoveRoutine(GridTile targetGridTile)
        {
            //var originalGridTile = _entity.currentTile;
            //float t = 0;
            //for (t = Time.deltaTime / moveAnimDuration; t < 1; t += Time.deltaTime / moveAnimDuration)
            //{
            //    // Lerp position based on the animation curve
            //    transform.position = Vector3.LerpUnclamped(originalGridTile.EntityPivotPosition, targetGridTile.EntityPivotPosition, moveAnimCurve.Evaluate(t));

            //    // wait for the next frame
            //    yield return null;
            //}

            //// Set the position to the tile's worldposition
            //transform.position = targetGridTile.EntityPivotPosition;
            //StopStepMoveRoutine();
            return null;
        }
    }
}
