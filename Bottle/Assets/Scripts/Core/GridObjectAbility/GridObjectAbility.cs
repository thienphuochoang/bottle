using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.GridObjectData;
using System;
namespace Bottle.Core.GridObjectAbility
{
    public class GridObjectAbility<T> : SerializedMonoBehaviour where T : Component
    {
        //[BoxGroup("Ability Settings")]
        //[Tooltip("If true, this ability can perform as usual, if not, it'll be ignored. We can use this to unlock abilities over time for example")]
        //public bool isAbilityLocked = false;

        protected T _currentGridObject;

        protected virtual void OnEnable()
        {
            //if (isAbilityLocked)
            //    this.enabled = false;
        }

        protected virtual void Awake()
        {
            _currentGridObject = this.gameObject.GetComponentInParent<T>();
        }

        protected virtual void Start()
        {
            
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }
    }
    //[CreateAssetMenu(fileName = "GridObjectAbilityGeneralDescription", menuName = "Bottle/Ability/GridObjectAbilityGeneralDescription", order = 1)]
    [Serializable]
    public abstract class GridObjectAbilitySettings<T> : ScriptableObject where T : Component
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

        [BoxGroup("Grid Object Ability Settings")]
        public T currentGridObject;

        public abstract void AbilityStart();
        public abstract void AbilityUpdate();
    }
}

