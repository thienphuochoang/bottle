using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragAbility : GridEntityInteractAbility
    {
        protected override void Update()
        {
            base.Update();
        }
        protected override void Start()
        {
            base.Start();
            interactButtonStates[KeyCode.E].ButtonDownHandler += Interact;
        }
    }

}
