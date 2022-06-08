using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityAbility : GridObjectAbility<GridEntity>
    {
        [PreviewField]
        [TableColumnWidth(57, Resizable = false)]
        public Texture abilityIcon;

        [TextArea(2, 10)]
        public string abilityDescription;

        public GridEntityAbility[] abilities;

        protected override void Awake()
        {
            base.Awake();
            abilities = GetComponentsInParent<GridEntityAbility>();
        }
    }
}

