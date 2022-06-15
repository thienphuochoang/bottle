using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.GridObjectData;
using System;
using Bottle.Extensions.Helper;
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
}

