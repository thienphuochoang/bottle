using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragPassiveAbility : GridObjectPassiveAbility<GridEntity>
    {
        private GridEntity _controllableMainGridEntity;
        protected override void Start()
        {
            base.Start();
            EventManager.Instance.StartListening("PushAndDragPassiveEvent", BeingPushedOrDragged);
        }

        private void BeingPushedOrDragged(Dictionary<string, object> message)
        {
            bool isTriggeredValue = (bool)message["isTriggered"];
            GridEntity currentInteractingObject = (GridEntity)message["currentInteractingGridObject"];
            _controllableMainGridEntity = (GridEntity)message["controllableMainGridEntity"];
            if (currentInteractingObject == _currentGridObject)
            {
                isPassiveAbilityTriggered = isTriggeredValue;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (isPassiveAbilityTriggered)
            {
                isContinuous = true;
                isPassiveAbilityTriggered = false;
            }
            if (isContinuous == true && isPassiveAbilityTriggered == false && _controllableMainGridEntity != null)
            {

            }
        }

        private void FollowMainGridEntityStep()
        {
        }
    }
}

