using System;
using System.Collections;
using System.Collections.Generic;
using SansyHuman.TWHG.Core;
using SansyHuman.TWHG.UI;
using SansyHuman.TWHG.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SansyHuman.TWHG.Debug
{
    public class GridTest : MonoBehaviour
    {
        private TWHGInputActions _actions;
        private Camera _mainCamera;

        [SerializeField] private GridTileSystem gridTileSystem;
        [SerializeField] private GridWallSystem gridWallSystem;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private RectTransform tileUiImage;
        [SerializeField] private SelectedObjects selectedObjects;
        [SerializeField] private RectTransform objectHierarcy;

        void Awake()
        {
            _actions = new TWHGInputActions();
            _mainCamera = Camera.main;
            
            cameraController.enableDrag = false;
            cameraController.enableMagnification = false;

            _actions.Editor.ScreenClick.canceled += context =>
            {
                if (EventSystem.current.IsPointerOverGameObject() || _actions.Editor.Alt.IsPressed())
                {
                    return;
                }
                
                Vector2 mousePos = _actions.Editor.PointerPosition.ReadValue<Vector2>();
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
                Vector2Int gridPos = gridTileSystem.GetGridIndex(worldPos.x, worldPos.y);

                if (!gridTileSystem.AddTile(gridPos.x, gridPos.y).HasValue)
                {
                    return;
                }

                selectedObjects.AddSelectedObject(gridTileSystem.GetTile(gridPos.x, gridPos.y).Value.MainTile.gameObject);

                if (gridWallSystem.DoesTileExist(gridPos.x, gridPos.y))
                {
                    gridWallSystem.RemoveTile(gridPos.x, gridPos.y);
                }

                if (!gridTileSystem.DoesTileExist(gridPos.x + 1, gridPos.y) && !gridWallSystem.DoesTileExist(gridPos.x + 1, gridPos.y))
                {
                    gridWallSystem.AddTile(gridPos.x + 1, gridPos.y);
                }
                
                if (!gridTileSystem.DoesTileExist(gridPos.x - 1, gridPos.y) && !gridWallSystem.DoesTileExist(gridPos.x - 1, gridPos.y))
                {
                    gridWallSystem.AddTile(gridPos.x - 1, gridPos.y);
                }
                
                if (!gridTileSystem.DoesTileExist(gridPos.x, gridPos.y + 1) && !gridWallSystem.DoesTileExist(gridPos.x, gridPos.y + 1))
                {
                    gridWallSystem.AddTile(gridPos.x, gridPos.y + 1);
                }
                
                if (!gridTileSystem.DoesTileExist(gridPos.x, gridPos.y - 1) && !gridWallSystem.DoesTileExist(gridPos.x, gridPos.y - 1))
                {
                    gridWallSystem.AddTile(gridPos.x, gridPos.y - 1);
                }
            };

            _actions.Editor.ScreenNegClick.canceled += context =>
            {
                if (EventSystem.current.IsPointerOverGameObject() || _actions.Editor.Alt.IsPressed())
                {
                    return;
                }
                
                Vector2 mousePos = _actions.Editor.PointerPosition.ReadValue<Vector2>();
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
                Vector2Int gridPos = gridTileSystem.GetGridIndex(worldPos.x, worldPos.y);

                GridTileSystem.Tile? tile = gridTileSystem.GetTile(gridPos.x, gridPos.y);
                if (!tile.HasValue)
                {
                    return;
                }
                selectedObjects.RemoveSelectedObject(tile.Value.MainTile.gameObject);
                gridTileSystem.RemoveTile(gridPos.x, gridPos.y);

                if (gridWallSystem.DoesTileExist(gridPos.x + 1, gridPos.y) && !gridTileSystem.DoesTileExistAtAdjacentGrids(gridPos.x + 1, gridPos.y))
                {
                    gridWallSystem.RemoveTile(gridPos.x + 1, gridPos.y);
                }
                
                if (gridWallSystem.DoesTileExist(gridPos.x - 1, gridPos.y) && !gridTileSystem.DoesTileExistAtAdjacentGrids(gridPos.x - 1, gridPos.y))
                {
                    gridWallSystem.RemoveTile(gridPos.x - 1, gridPos.y);
                }
                
                if (gridWallSystem.DoesTileExist(gridPos.x, gridPos.y + 1) && !gridTileSystem.DoesTileExistAtAdjacentGrids(gridPos.x, gridPos.y + 1))
                {
                    gridWallSystem.RemoveTile(gridPos.x, gridPos.y + 1);
                }
                
                if (gridWallSystem.DoesTileExist(gridPos.x, gridPos.y - 1) && !gridTileSystem.DoesTileExistAtAdjacentGrids(gridPos.x, gridPos.y - 1))
                {
                    gridWallSystem.RemoveTile(gridPos.x, gridPos.y - 1);
                }

                if (gridTileSystem.DoesTileExistAtAdjacentGrids(gridPos.x, gridPos.y))
                {
                    gridWallSystem.AddTile(gridPos.x, gridPos.y);
                }
            };

            _actions.Editor.ScreenClick.canceled += context => UnityEngine.Debug.Log("Mouse released.");

            _actions.Editor.Alt.started += context =>
            {
                cameraController.enableDrag = true;
                cameraController.enableMagnification = true;
                StartCoroutine(RebuildLayout(objectHierarcy));
            };
            
            _actions.Editor.Alt.canceled += context =>
            {
                cameraController.enableDrag = false;
                cameraController.enableMagnification = false;
            };
        }

        private IEnumerator RebuildLayout(RectTransform tr)
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr);
            Canvas.ForceUpdateCanvases();
        }

        private void Update()
        {
            Vector2 mousePos = _actions.Editor.PointerPosition.ReadValue<Vector2>();
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
            Vector2Int gridPos = gridTileSystem.GetGridIndex(worldPos.x, worldPos.y);
            Vector2 tileUiPos = gridTileSystem.GetGridPosition(gridPos.x, gridPos.y);
            
            tileUiImage.anchoredPosition = tileUiPos;
        }

        private void OnEnable()
        {
            _actions.Editor.Enable();
        }

        private void OnDisable()
        {
            _actions.Editor.Disable();
        }

        public void OnTileAdded(GridTileSystem.Tile? tile)
        {
            UnityEngine.Debug.Log("Tile added: " + tile.Value.MainTile.gameObject.name);
        }

        public void OnTileRemoved(GridTileSystem.Tile? tile)
        {
            UnityEngine.Debug.Log("Tile removed: " + tile.Value.MainTile.gameObject.name);
        }

        public void OnWallAdded(GridWallSystem.Wall? wall)
        {
            UnityEngine.Debug.Log("Wall added: " + wall.Value.Collider.gameObject.name);
        }

        public void OnWallRemoved(GridWallSystem.Wall? wall)
        {
            UnityEngine.Debug.Log("Wall removed: " + wall.Value.Collider.gameObject.name);
        }
    }
}
