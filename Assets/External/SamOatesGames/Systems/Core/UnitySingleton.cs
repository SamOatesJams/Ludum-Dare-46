//#define USE_ODIN

#if USE_ODIN
using Sirenix.OdinInspector;
#endif

using UnityEngine;

namespace SamOatesGames.Systems.Core
{
    public interface IUnitySingleton
    {
        void ResolveSystems();
    }

#if USE_ODIN
    public abstract class UnitySingleton<T> : SerializedMonoBehaviour, IUnitySingleton where T : SerializedMonoBehaviour
#else
    public abstract class UnitySingleton<T> : MonoBehaviour, IUnitySingleton where T : MonoBehaviour
#endif
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        private static T s_instance;

        /// <summary>
        /// Used to destroy multiple system managers.
        /// If an instance does not exist, it forces the instance to be this.
        /// </summary>
        protected virtual void Awake()
        {
            if (Exists())
            {
                Destroy(gameObject);
                return;
            }

            SetInstance((T) (object) this);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ResolveSystems()
        {
        }

        /// <summary>
        /// Create a new singleton instance of the requested type.
        /// The component will be added to an owning game object.
        /// The owner will have the name '{Type}_singleton'.
        /// </summary>
        /// <returns>The mono behaviour instance.</returns>
        private static T CreateSingletonInstance(GameObject parent)
        {
            var instanceType = typeof(T);

            var owner = new GameObject();
            s_instance = owner.AddComponent<T>();
            owner.name = string.Format("{0}_singleton", instanceType.Name);
            owner.transform.SetParent(parent.transform);

            var rootObject = parent.transform;
            while (rootObject.parent != null)
            {
                rootObject = rootObject.parent;
            }

            DontDestroyOnLoad(rootObject.gameObject);

            Debug.LogFormat("Create a new instance of '{0}' with name '{1}'.", instanceType, owner.name);
            return s_instance;
        }

        /// <summary>
        /// Gets or creates the instance for this singleton type.
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="parent">The game object which should own this singleton</param>
        protected static void CreateInstanceIfRequired(GameObject parent)
        {
            s_instance = s_instance ? s_instance : CreateSingletonInstance(parent);
        }

        /// <summary>
        /// Does the singleton instance exist?
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            return s_instance != null;
        }

        /// <summary>
        /// Force the instance to an already existing instance
        /// </summary>
        public static void SetInstance(T instance)
        {
            if (s_instance != null)
            {
                Debug.LogWarningFormat("An instance for type '{0}' already exists!", typeof(T).Name);
                return;
            }

            s_instance = instance;

            // Don't destroy on load can only be set on a root game object.
            var root = s_instance.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

            DontDestroyOnLoad(root.gameObject);
            Debug.LogFormat("The instance '{0}' has been set.", typeof(T).Name);
        }

        /// <summary>
        /// Get access to an already created singleton instance.
        /// </summary>
        /// <returns>The singleton instance (can be null, if instance does not exist).</returns>
        public static T GetInstance()
        {
            if (s_instance == null)
            {
                Debug.LogWarningFormat("The instance for type '{0}' was requested, but it does not exist!",
                    typeof(T).Name);
            }

            return s_instance;
        }
    }
}
