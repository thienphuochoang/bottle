using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityInteractAbility : GridObjectAbility<GridEntity>
    {
        private static Dictionary<KeyCode, InputButton> interactButtonStates => InputManager.Instance.buttonStates;
        private GridObject interactiveGridObject;
        protected override void Start()
        {
            interactButtonStates[KeyCode.E].ButtonDownHandler += Interact;
        }

        protected override void Update()
        {

        }

        private void Interact(InputButton.States state, KeyCode keyCode)
        {

        }
    }

}