using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.GridObjectData;
namespace Bottle.Core.GridObjectAbility
{
    [System.Serializable]
    [InlineEditor]
    public abstract class GridEntityAbility
    {
        public GridObjectAbilitySettings gridEntityAbilitySettings;
        public System.Type gridEntityAbilitySettingsType;
        [HideInInspector]
        public GridEntity currenGridEntity;
        public abstract void AbilityOnAwake();
        public abstract void AbilityOnEnable();
        public abstract void AbilityStart();
        public abstract void AbilityUpdate();
    }
}
