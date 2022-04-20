using UnityEngine;
using Sirenix.OdinInspector;
namespace Bottle.Extensions.Singleton
{
    /// <summary>
    ///     A persistence object that dont be destroyed when loading another
    ///     scene, which can be inherited and make sure there's only one
    ///     instance in the scene.
    /// </summary>
    public class PersistentObject<T> : SerializedMonoBehaviour where T : Component
    {
        // Variables
        // ------------------------------------------------------------------------
        protected bool _enabled;
        protected static T _instance;

        // Property
        // ------------------------------------------------------------------------

        /// <summary>
        ///     The instance of this PersistentObject. Using Singleton Design Pattern.
        /// </summary>
        /// <value>The Instance</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<T>();

                        // Setting name for better management
                        string[] typeName = obj.GetComponent<T>().ToString().Split('.', ')');
                        _instance.name = typeName[typeName.Length - 2];
                    }
                }
                return _instance;
            }
        }

        // Methods
        // -----------------------------------------------------------------------

        public virtual void Initialize() { }

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;

            if (_instance == null)
            {
                // If this is the first instance, make it singleton.
                _instance = this as T;
                _enabled = true;
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void Start()
        {
            // If another Singleton already exists, destroy this one.
            if (_instance != null && _instance != this)
                Destroy(this);
        }

        protected void OnApplicationQuit()
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
