using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityAbilityController : GridObjectAbility<GridEntity>
    {
        [ListDrawerSettings(ShowPaging = true)]
        public List<GridEntityAbility> availableAbilities = new List<GridEntityAbility>();

        public override void Awake()
        {
            base.Awake();
            foreach (var ability in availableAbilities)
            {
                ability.currentGridEntity = this._currentGridObject;
                ability.gridEntityAbilityController = this;
                ability.AbilityOnAwake();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            foreach (var ability in availableAbilities)
            {
                ability.AbilityOnEnable();
            }
        }

        public override void Start()
        {
            base.Start();
            foreach (var ability in availableAbilities)
            {
                ability.AbilityStart();
            }
        }

        public override void Update()
        {
            base.Update();
            foreach (var ability in availableAbilities)
            {
                ability.AbilityUpdate();
            }
        }
    }
}

