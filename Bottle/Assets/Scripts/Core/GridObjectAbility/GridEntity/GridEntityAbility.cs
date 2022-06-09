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
    public class GridEntityAbility : GridObjectAbility<GridEntity>
    {
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<System.Type, GridEntityAbility> abilities = new Dictionary<System.Type, GridEntityAbility>();

        protected override void Awake()
        {
            base.Awake();
            foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var tp in assemb.GetTypes())
                    if (tp.BaseType == typeof(GridEntityAbility))
                    {
                        if (abilities.ContainsKey(tp))
                        {
                        }
                        else
                        {
                            ConstructorInfo constructor = tp.GetConstructor(System.Type.EmptyTypes);
                            //Component new_weapon = (tp)(object)constructor.Invoke(null);
                            //abilities.Add(tp, (GridEntityAbility)new_weapon);
                        }
                    }
            }
        }
    }
    public class GridEntityAbilitySetting
    {
        public bool isEnabled;

        [PreviewField]
        public Texture abilityIcon;

        [TextArea(2, 10)]
        public string abilityDescription;
    }
}

