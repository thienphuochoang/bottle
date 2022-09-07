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
        public List<GridObjectAbilitySettings> gridEntityAbilitySettings;
        [HideInInspector]
        public GridEntityAbilityController gridEntityAbilityController;
        [HideInInspector]
        public GridEntity currentGridEntity;
        public abstract void AbilityOnAwake();
        public abstract void AbilityOnEnable();
        public abstract void AbilityStart();
        public abstract void AbilityUpdate();
        public static bool CheckIfAbilityExist<T>(GridEntity targetGridEntity)
        {
            var isPassiveAbilityExist = GetGridEntityAbility<T>(targetGridEntity);
            if (isPassiveAbilityExist != null)
                return true;
            return false;
        }

        public static GridEntityAbility GetGridEntityAbility<T>(GridEntity targetGridEntity)
        {
            var entityAbility = targetGridEntity.GetComponent<GridEntityAbilityController>()
                .availableAbilities.Find(gridEntityAbility =>
                    gridEntityAbility.GetType() == typeof(T));
            return entityAbility;
        }
    }
}
