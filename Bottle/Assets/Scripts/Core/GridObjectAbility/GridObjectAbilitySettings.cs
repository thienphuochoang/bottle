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
        //[OnInspectorGUI("SwitchInputType")]
        //public TextureInputReference abilityIcon = new TextureInputReference();
        //[ReadOnly]
        [PreviewField]
        public Texture abilityIcon;

        [BoxGroup("Grid Object Ability Settings")]
        [TextArea(2, 10)]
        //[OnInspectorGUI("SwitchInputType")]
        //public StringInputReference abilityDescription = new StringInputReference();
        //[ReadOnly]
        public string abilityDescription;

        [BoxGroup("Grid Object Ability Settings")]
        [TextArea(2, 10)]
        //[OnInspectorGUI("SwitchInputType")]
        //public StringInputReference abilityTip = new StringInputReference();
        //[ReadOnly]
        public string abilityTip;

        //[HideInInspector]
        //public T _currentGridObject;

        //public abstract void AbilityStart();
        //public abstract void AbilityUpdate();
        //public void OnEnable()
        //{
        //SwitchInputType();
        //}
        //private void SwitchInputType()
        //{
        //    if (_abilityIcon == null)
        //        _abilityIcon = default(TextureInputReference);
        //    if (_abilityIcon != null)
        //        _abilityIcon = abilityIcon;

        //    if (_abilityDescription == null)
        //        _abilityDescription = default(StringInputReference);
        //    if (_abilityDescription != null)
        //        _abilityDescription = abilityDescription;

        //    if (_abilityTip == null)
        //        _abilityTip = default(StringInputReference);
        //    if (_abilityTip != null)
        //        _abilityTip = abilityTip;
        //}
    }
}

