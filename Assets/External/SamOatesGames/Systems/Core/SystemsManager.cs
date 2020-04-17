using UnityEngine;

namespace SamOatesGames.Systems.Core
{
    public class SystemsManager : UnitySingleton<SystemsManager>
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Enable children
            foreach (var child in GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.SetActive(true);
            }
        }

        protected void Start()
        {
            foreach (var child in GetComponentsInChildren<Transform>(true))
            {
                foreach (var unitySingleton in child.GetComponents<IUnitySingleton>())
                {
                    unitySingleton.ResolveSystems();
                }
            }
        }
    }
}
