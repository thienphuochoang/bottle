using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using System.Reflection;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityAbilityController : GridObjectAbility<GridEntity>
    {
        [ListDrawerSettings(ShowPaging = true)]
        public List<GridEntityAbility> availableAbilities = new List<GridEntityAbility>();

        protected override void Awake()
        {
            base.Awake();
            foreach (var ability in availableAbilities)
            {
                ability.currenGridEntity = this._currentGridObject;
                ability.AbilityOnAwake();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (var ability in availableAbilities)
            {
                ability.AbilityOnEnable();
            }
        }

        protected override void Start()
        {
            base.Start();
            foreach (var ability in availableAbilities)
            {
                ability.AbilityStart();
            }
        }

        protected override void Update()
        {
            base.Update();
            foreach (var ability in availableAbilities)
            {
                ability.AbilityUpdate();
            }
        }
    }
    public class GridEntityAbilitySettings
    {

    }
}

