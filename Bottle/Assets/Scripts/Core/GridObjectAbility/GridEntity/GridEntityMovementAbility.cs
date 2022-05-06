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
        private enum MovementDirections { NONE, UP, DOWN , LEFT, RIGHT};
        [ShowInInspector]
        private MovementDirections _currentMovementDirection;

        private void DetectMovementDirection(InputButton.States state, KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.W:
                    _currentMovementDirection = MovementDirections.UP;
                    break;
            }
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();
            _movementButtonStates[KeyCode.W].ButtonDownHandler += DetectMovementDirection;
            //this.ButtonDownHandler += DetectChange;
        }

        protected override void Update()
        {
            base.Update();
            if (_movementButtonStates[KeyCode.W].currentState == InputButton.States.BUTTON_DOWN)
            {
            }
        }
    }
}
