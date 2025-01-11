using UnityEngine;

namespace SansyHuman.TWHG.Core
{
    /// <summary>
    /// Class that manages inputs.
    /// </summary>
    public class InputSystem : MonoBehaviourSingleton<InputSystem>
    {
        private TWHGInputActions _actions;

        void Awake()
        {
            _actions = new TWHGInputActions();
        }

        void OnEnable()
        {
            _actions.Enable();
        }

        void OnDisable()
        {
            _actions.Disable();
        }

        /// <summary>
        /// Gets the TWHG input actions instance.
        /// </summary>
        public static TWHGInputActions Actions => Instance._actions;
    }
}
