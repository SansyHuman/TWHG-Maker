using System;
using SansyHuman.TWHG.Core;
using UnityEngine;

namespace SansyHuman.TWHG.Objects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        private TWHGInputActions _actions;
        private Rigidbody2D _rigidbody;
        
        [Tooltip("Maximum speed of the player.")]
        [SerializeField] private float _maxSpeed = 3.0f;
        
        private void Awake()
        {
            _actions = new TWHGInputActions();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_actions.Game.Move.IsPressed())
            {
                Vector2 dir = _actions.Game.Move.ReadValue<Vector2>();
                _rigidbody.velocity = dir * _maxSpeed;
            }
            else
            {
                _rigidbody.velocity = Vector2.zero;
            }
        }

        private void OnEnable()
        {
            _actions.Game.Enable();
        }

        private void OnDisable()
        {
            _actions.Game.Disable();
        }
    }
}
