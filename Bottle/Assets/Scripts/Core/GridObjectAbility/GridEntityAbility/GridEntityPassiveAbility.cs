using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
namespace Bottle.Core.GridObjectAbility
{
    [System.Serializable]
    [InlineEditor]
    public class GridEntityPassiveAbility : GridEntityAbility
    {
        protected bool _isTriggered;
        public GridEntity triggerGridEntity;
        public bool isTriggered
        {
            get => _isTriggered;
            set
            {
                if (_isTriggered == value) return;
                _isTriggered = value;
            }
        }
        public override void AbilityOnAwake()
        {
        }

        public override void AbilityOnEnable()
        {
        }

        public override void AbilityStart()
        {
        }

        public override void AbilityUpdate()
        {
        }
    }
}