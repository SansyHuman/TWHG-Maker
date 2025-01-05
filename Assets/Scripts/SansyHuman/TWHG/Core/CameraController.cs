using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SansyHuman.TWHG.Core
{
    /// <summary>
    /// Component that controls camera movement by user input.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private TWHGInputActions _actions;
        
        private Camera _cameraComponent;
        private Vector3 _cameraPosition;
        private Vector3 _clickPosition;
        
        [Tooltip("The ratio to move the camera by drag and drop.")]
        [SerializeField] private float dragRatio = 1.0f;
        
        [Tooltip("The speed to change the magnification of the screen.")]
        [SerializeField] private float magnificationSpeed = 0.05f;

        [Tooltip("Enables the camera movement by drag and drop.")]
        public bool enableDrag = true;
        
        [Tooltip("Enables the magnification of the screen.")]
        public bool enableMagnification = true;

        private void Awake()
        {
            _actions = new TWHGInputActions();
        }
        
        private void Start()
        {
            _cameraComponent = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // Ignore input if the pointer is on UI.
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            if (enableDrag)
            {
#if UNITY_STANDALONE || UNITY_EDITOR
                MouseMovement();
#elif UNITY_ANDROID || UNITY_IOS
                TouchMovement();
#endif
            }

            if (enableMagnification)
            {
                ScreenMagnification();
            }
        }

        private void MouseMovement()
        {
            if (_actions.Editor.ScreenClick.WasPressedThisFrame())
            {
                _clickPosition = _actions.Editor.PointerPosition.ReadValue<Vector2>();
                _cameraPosition = _cameraComponent.transform.position;
            }
            else if (_actions.Editor.ScreenClick.IsPressed())
            {
                Vector3 displacement = _cameraComponent.ScreenToWorldPoint(_clickPosition) - _cameraComponent.ScreenToWorldPoint(_actions.Editor.PointerPosition.ReadValue<Vector2>());
                _cameraComponent.transform.position = _cameraPosition + displacement * dragRatio;
            }
        }

        private void TouchMovement()
        {
            if (Input.touchCount != 1)
                return;
            
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _clickPosition = touch.position;
                _cameraPosition = _cameraComponent.transform.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 displacement = _cameraComponent.ScreenToWorldPoint(_clickPosition) - _cameraComponent.ScreenToWorldPoint(touch.position);
                _cameraComponent.transform.position = _cameraPosition + displacement * dragRatio;
            }
        }

        private void ScreenMagnification()
        {
            Vector3 mag = _actions.Editor.ScreenMag.ReadValue<Vector2>();
            mag.z = 0;
            
            mag = _cameraComponent.ScreenToWorldPoint(mag) - _cameraComponent.ScreenToWorldPoint(Vector3.zero);
            float magSize = mag.magnitude * magnificationSpeed;

            if (mag.y > 0)
            {
                _cameraComponent.orthographicSize -= magSize;
            }
            else
            {
                _cameraComponent.orthographicSize += magSize;
            }
        }

        private void OnEnable()
        {
            _actions.Editor.Enable();
        }

        private void OnDisable()
        {
            _actions.Editor.Disable();
        }
    }
}