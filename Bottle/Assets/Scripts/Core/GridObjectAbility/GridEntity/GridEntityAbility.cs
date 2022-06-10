using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
using System.Reflection;
namespace Bottle.Core.GridObjectAbility
{
    [ExecuteInEditMode]
    public class GridEntityAbility : GridObjectAbility<GridEntity>, GridObjectAbilityGeneralDescription
    {
        public string abilityDescription { get; set; }
        [PreviewField]
        public Texture abilityIcon { get; set; }
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<System.Type, GridEntityInteractAbility> abilities = new Dictionary<System.Type, GridEntityInteractAbility>();

        protected override void Awake()
        {
            base.Awake();
            abilities = new Dictionary<System.Type, GridEntityInteractAbility>();
            foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var tp in assemb.GetTypes())
                    if (tp.BaseType == typeof(GridEntityInteractAbility))
                    {
                        if (abilities.ContainsKey(tp))
                        {
                        }
                        else
                        {
                            var instance = System.Activator.CreateInstance(tp);
                            //Component new_weapon = (tp)(object)constructor.Invoke(null);
                            abilities.Add(tp, (GridEntityInteractAbility)instance);
                        }
                    }
            }
        }
    }
}

