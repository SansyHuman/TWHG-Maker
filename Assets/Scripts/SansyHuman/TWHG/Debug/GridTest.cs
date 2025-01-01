using System;
using SansyHuman.TWHG.Core;
using SansyHuman.TWHG.World;
using UnityEngine;

namespace SansyHuman.TWHG.Debug
{
    public class GridTest : MonoBehaviour
    {
        private TWHGInputActions _actions;
        private Camera _mainCamera;

        [SerializeField] private GridSystem _gridSystem;
        [SerializeField] private CameraController _cameraController;

        void Awake()
        {
            _actions = new TWHGInputActions();
            _mainCamera = Camera.main;
            
            _cameraController.enableDrag = false;
            _cameraController.enableMagnification = false;

            _actions.Editor.ScreenClick.performed += context =>
            {
                if (_actions.Editor.Alt.IsPressed())
                {
                    return;
                }
                
                Vector2 mousePos = _actions.Editor.PointerPosition.ReadValue<Vector2>();
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
                Vector2Int gridPos = _gridSystem.GetGridIndex(worldPos.x, worldPos.y);

                _gridSystem.AddTile(gridPos.x, gridPos.y);
            };

            _actions.Editor.ScreenNegClick.performed += context =>
            {
                if (_actions.Editor.Alt.IsPressed())
                {
                    return;
                }
                
                Vector2 mousePos = _actions.Editor.PointerPosition.ReadValue<Vector2>();
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
                Vector2Int gridPos = _gridSystem.GetGridIndex(worldPos.x, worldPos.y);

                _gridSystem.RemoveTile(gridPos.x, gridPos.y);
            };

            _actions.Editor.ScreenClick.canceled += context => UnityEngine.Debug.Log("Mouse released.");

            _actions.Editor.Alt.started += context =>
            {
                _cameraController.enableDrag = true;
                _cameraController.enableMagnification = true;
            };
            
            _actions.Editor.Alt.canceled += context =>
            {
                _cameraController.enableDrag = false;
                _cameraController.enableMagnification = false;
            };
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
