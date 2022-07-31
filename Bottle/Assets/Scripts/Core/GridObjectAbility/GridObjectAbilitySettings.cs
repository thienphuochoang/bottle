using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Extensions.Helper;
namespace Bottle.Core.GridObjectAbility
{
    //[CreateAssetMenu(fileName = "GridObjectAbilitySettings", menuName = "Bottle/Ability/GridObjectAbilitySettings", order = 1)]
    [System.Serializable]
    [InlineEditor]
    public abstract class GridObjectAbilitySettings : ScriptableObject
    {
        [BoxGroup("Grid Object Ability Settings")]
        [PreviewField]
        public Texture abilityIcon;

        [BoxGroup("Grid Object Ability Settings")]
        [TextArea(2, 10)]
        public string abilityDescription;

        [BoxGroup("Grid Object Ability Settings")]
        [TextArea(2, 10)]
        public string abilityTip;
    }
}

