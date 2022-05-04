using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
using UnityEditor;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
namespace Bottle.Core.Manager
{
    public class InputButton
    {
        public enum States { OFF, BUTTON_DOWN, BUTTON_PRESSED, BUTTON_UP }
        public KeyCode currentKeyCode;
        public States currentState;

        public InputButton(InputButton userInputButton)
        {
            this.currentKeyCode = userInputButton.currentKeyCode;
            this.currentState = userInputButton.currentState;
        }
    }

    public class InputManager : PersistentObject<InputManager>
    {

        public enum MovementDirections { NONE, UP, DOWN, LEFT, RIGHT, FORWARD, BACK }
        public InputButton currentPressingButton;
        public float idleThreshold = 0.05f;



        private void DetermineUserInput()
        {
            if (Input.GetAxis("Vertical") > 0 && Mathf.Abs(Input.GetAxis("Vertical")) > idleThreshold)
            {
                Debug.Log("W is pressing");
            }
            else if (Input.GetAxis("Vertical") < 0 && Mathf.Abs(Input.GetAxis("Vertical")) > idleThreshold)
            {
                Debug.Log("S is pressing");
            }
            if (Input.GetAxis("Horizontal") < 0 && Mathf.Abs(Input.GetAxis("Horizontal")) > idleThreshold)
            {
                Debug.Log("A is pressing");
            }
            else if (Input.GetAxis("Horizontal") > 0 && Mathf.Abs(Input.GetAxis("Horizontal")) > idleThreshold)
            {
                Debug.Log("D is pressing");
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            DetermineUserInput();
        }
    }
}

