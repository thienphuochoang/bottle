using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
using UnityEditor;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
namespace Bottle.Core.Manager
{
    [System.Serializable]
    public class InputButton
    {
        public enum States { BUTTON_OFF, BUTTON_BEING_HELD, BUTTON_DOWN, BUTTON_UP }
        public States currentState;
        public KeyCode currentKeyCode;
        public delegate void ButtonDownDelegate(States state, KeyCode keyCode);
        public delegate void ButtonUpDelegate(States state, KeyCode keyCode);
        public delegate void ButtonBeingHeldDelegate(States state, KeyCode keyCode);
        public event ButtonDownDelegate ButtonDownHandler;
        public event ButtonUpDelegate ButtonUpHandler;
        public event ButtonBeingHeldDelegate ButtonBeingHeldHandler;

        public InputButton() { }

        public InputButton(InputButton userInputButton)
        {
            this.currentKeyCode = userInputButton.currentKeyCode;
            this.currentState = userInputButton.currentState;
        }

        public void ChangeState(States newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                switch (currentState)
                {
                    case States.BUTTON_BEING_HELD:
                        if (ButtonBeingHeldHandler != null)
                            ButtonBeingHeldHandler(currentState, this.currentKeyCode);
                        break;
                    case States.BUTTON_DOWN:
                        if (ButtonDownHandler != null)
                            ButtonDownHandler(currentState, this.currentKeyCode);
                        break;
                    case States.BUTTON_UP:
                        if (ButtonUpHandler != null)
                            ButtonUpHandler(currentState, this.currentKeyCode);
                        break;
                }
            }
        }
    }

    public class InputManager : PersistentObject<InputManager>
    {

        public enum MovementDirections { NONE, UP, DOWN, LEFT, RIGHT, FORWARD, BACK }
        [ShowInInspector]
        [ReadOnly]
        private static KeyCode[] registeredButtons = {KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.E };
        public Dictionary<KeyCode, InputButton> buttonStates = new Dictionary<KeyCode, InputButton>();
        public float idleThreshold = 0.05f;


        private void DetermineUserInput()
        {
            #region Button_W_KeyCode_Determination
            if (Input.GetButton("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                buttonStates[KeyCode.W].currentState = InputButton.States.BUTTON_BEING_HELD;

            }
            if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                if (buttonStates[KeyCode.W].currentState == InputButton.States.BUTTON_BEING_HELD)
                {
                    buttonStates[KeyCode.W].ChangeState(InputButton.States.BUTTON_DOWN);
                }
            }
            if (Input.GetButtonUp("Vertical") && Input.GetAxis("Vertical") > 0)
            {
                buttonStates[KeyCode.W].currentState = InputButton.States.BUTTON_UP;
                if (buttonStates[KeyCode.W].currentState == InputButton.States.BUTTON_UP)
                {
                    buttonStates[KeyCode.W].ChangeState(InputButton.States.BUTTON_OFF);
                }
            }
            #endregion

            #region Button_S_KeyCode_Determination
            if (Input.GetButton("Vertical") && Input.GetAxis("Vertical") < 0)
            {
                buttonStates[KeyCode.S].currentState = InputButton.States.BUTTON_BEING_HELD;

            }
            if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") < 0)
            {
                if (buttonStates[KeyCode.S].currentState == InputButton.States.BUTTON_BEING_HELD)
                {
                    buttonStates[KeyCode.S].ChangeState(InputButton.States.BUTTON_DOWN);
                }
            }
            if (Input.GetButtonUp("Vertical") && Input.GetAxis("Vertical") < 0)
            {
                buttonStates[KeyCode.S].currentState = InputButton.States.BUTTON_UP;
                if (buttonStates[KeyCode.S].currentState == InputButton.States.BUTTON_UP)
                {
                    buttonStates[KeyCode.S].ChangeState(InputButton.States.BUTTON_OFF);
                }
            }
            #endregion

            #region Button_A_KeyCode_Determination
            if (Input.GetButton("Horizontal") && Input.GetAxis("Horizontal") < 0)
            {
                buttonStates[KeyCode.A].currentState = InputButton.States.BUTTON_BEING_HELD;
            }
            if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") < 0)
            {
                if (buttonStates[KeyCode.A].currentState == InputButton.States.BUTTON_BEING_HELD)
                {
                    buttonStates[KeyCode.A].ChangeState(InputButton.States.BUTTON_DOWN);
                }
            }
            if (Input.GetButtonUp("Horizontal") && Input.GetAxis("Horizontal") < 0)
            {
                buttonStates[KeyCode.A].currentState = InputButton.States.BUTTON_UP;
                if (buttonStates[KeyCode.A].currentState == InputButton.States.BUTTON_UP)
                {
                    buttonStates[KeyCode.A].ChangeState(InputButton.States.BUTTON_OFF);
                }
            }
            #endregion

            #region Button_D_KeyCode_Determination
            if (Input.GetButton("Horizontal") && Input.GetAxis("Horizontal") > 0)
            {
                buttonStates[KeyCode.D].currentState = InputButton.States.BUTTON_BEING_HELD;
            }
            if (Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0)
            {
                if (buttonStates[KeyCode.D].currentState == InputButton.States.BUTTON_BEING_HELD)
                {
                    buttonStates[KeyCode.D].ChangeState(InputButton.States.BUTTON_DOWN);
                }
            }
            if (Input.GetButtonUp("Horizontal") && Input.GetAxis("Horizontal") > 0)
            {
                buttonStates[KeyCode.D].currentState = InputButton.States.BUTTON_UP;
                if (buttonStates[KeyCode.D].currentState == InputButton.States.BUTTON_UP)
                {
                    buttonStates[KeyCode.D].ChangeState(InputButton.States.BUTTON_OFF);
                }
            }
            #endregion
            #region Button_E_KeyCode_Determination
            DetermineKeyCodeStateInput("Interact", KeyCode.E);
            #endregion
        }

        private void Initialize()
        {
            foreach (var registeredKey in registeredButtons)
            {
                InputButton defaultInputButton = new InputButton();
                defaultInputButton.currentState = InputButton.States.BUTTON_OFF;
                defaultInputButton.currentKeyCode = registeredKey;
                buttonStates.Add(registeredKey, defaultInputButton);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            DetermineUserInput();
        }
        private void DetermineKeyCodeStateInput(string inputName, KeyCode inputKeyCode)
        {
            if (Input.GetButton(inputName))
            {
                buttonStates[inputKeyCode].currentState = InputButton.States.BUTTON_BEING_HELD;
            }
            if (Input.GetButtonDown(inputName))
            {
                if (buttonStates[inputKeyCode].currentState == InputButton.States.BUTTON_BEING_HELD)
                {
                    buttonStates[inputKeyCode].ChangeState(InputButton.States.BUTTON_DOWN);
                }
            }
            if (Input.GetButtonUp(inputName))
            {
                buttonStates[inputKeyCode].currentState = InputButton.States.BUTTON_UP;
                if (buttonStates[inputKeyCode].currentState == InputButton.States.BUTTON_UP)
                {
                    buttonStates[inputKeyCode].ChangeState(InputButton.States.BUTTON_OFF);
                }
            }
        }
    }
}

