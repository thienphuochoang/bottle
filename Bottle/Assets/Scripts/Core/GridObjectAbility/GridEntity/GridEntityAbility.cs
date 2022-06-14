using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using System.Reflection;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityAbility : GridObjectAbility<GridEntity>
    {
        [ListDrawerSettings(ShowPaging = true)]
        public List<GridObjectAbilitySettings<GridEntity>> availableAbilities = new List<GridObjectAbilitySettings<GridEntity>>();

        protected override void Awake()
        {
            base.Awake();
            foreach (var ability in availableAbilities)
            {
                ability._currentGridObject = this._currentGridObject;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (var ability in availableAbilities)
            {
                ability.AbilityEnable();
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
}

