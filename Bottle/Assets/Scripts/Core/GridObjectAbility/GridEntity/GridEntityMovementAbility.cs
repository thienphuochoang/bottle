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
        private Dictionary<KeyCode, InputButton> movementButtonStates => InputManager.Instance.buttonStates;

        public delegate void ButtonDownDelegate(InputButton.States state);
        public delegate void ButtonPressedDelegate();
        public delegate void ButtonUpDelegate();

        public event ButtonDownDelegate ButtonDownHandler;
        public event ButtonPressedDelegate ButtonPressedHandler;
        public event ButtonUpDelegate ButtonUpHandler;

        private void DetectChange()
        {
            Debug.Log("Detect Changes");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();
            movementButtonStates.Remove(KeyCode.E);
            //this.ButtonDownHandler += DetectChange;
        }

        protected override void Update()
        {
            base.Update();
            if (movementButtonStates[KeyCode.W].currentState == InputButton.States.BUTTON_DOWN)
            {
                Debug.Log("ahihi");
            }
        }
    }

}
