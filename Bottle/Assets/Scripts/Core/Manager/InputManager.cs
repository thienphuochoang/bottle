using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
using Bottle.Core.InputSystem;
using UnityEngine.InputSystem;
using UnityEditor;
using Bottle.Core.GridObjectData;
namespace Bottle.Core.Manager
{
    [CustomEditor(typeof(GridObject))]
    public class InputManager : Editor
    {
        SerializedProperty lookAtPoint;
        private UserInputAction _userInputAction;
        public float mouseScrollSpeed;
        private void Awake()
        {
            _userInputAction = new UserInputAction();
            _userInputAction.Enable();
            _userInputAction.Editor.RotateAroundYAxis.performed += x => mouseScrollSpeed = x.ReadValue<float>();
        }

        private void OnEnable()
        {
            lookAtPoint = serializedObject.FindProperty("gridPosition");
        }

        private void Start()
        {

        }

        public void OnSceneGUI()
        {
            Debug.Log(Event.current.type);
        }
    }
}

