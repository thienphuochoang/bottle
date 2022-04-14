using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bottle.Extensions
{
    /// <summary>
    ///     A persistence scriptable object to be used as persistent singleton
    ///     <list type="bullet">
    ///         <item>Can be inherited.</item>
    ///         <item>Can have multiple SO reference from this class.</item>
    ///         <item>Can use for independent component.</item>
    ///     </list>
    /// </summary>
    public class PersistentScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        // Variables
        // ------------------------------------------------------------------------
        protected static T _instance;

        // Property
        // ------------------------------------------------------------------------
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var type = typeof(T);

                    // TODO(Nghia Lam): This is not a good way for loading SO at
                    // run-time. Try using Addressable.
                    var allInstances = Resources.LoadAll<T>(string.Empty);
                    _instance = allInstances.FirstOrDefault();

                    if (_instance == null)
                        Debug.LogErrorFormat("[PersistentSO] No instance of {0} found!", type.ToString());
                    else if (allInstances.Count() > 1)
                        Debug.LogErrorFormat("[PersistentSO] Multiple instances of {0} found!", type.ToString());
                    else
                        Debug.LogFormat("[PersistentSO] An instance of {0} was found!", type.ToString());
                }

                return _instance;
            }
        }

        // Methods
        // ------------------------------------------------------------------------

        public virtual void Initialize() { }
    }
}