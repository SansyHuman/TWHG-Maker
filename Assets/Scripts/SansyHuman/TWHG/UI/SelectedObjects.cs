using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Controll for selected objects box.
    /// </summary>
    public class SelectedObjects : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer leftBottomCorner;
        [SerializeField] private SpriteRenderer leftTopCorner;
        [SerializeField] private SpriteRenderer rightTopCorner;
        [SerializeField] private SpriteRenderer rightBottomCorner;
        
        [SerializeField] private LineRenderer leftSide;
        [SerializeField] private LineRenderer topSide;
        [SerializeField] private LineRenderer rightSide;
        [SerializeField] private LineRenderer bottomSide;
        
        private HashSet<GameObject> _selectedObjects;
        private Vector2 _minPos;
        private Vector2 _maxPos;
        
        // Start is called before the first frame update
        void Awake()
        {
            SetEnabled(false);
            _selectedObjects = new HashSet<GameObject>();
            _minPos = new Vector2(0, 0);
            _maxPos = new Vector2(0, 0);

            transform.position = Vector3.zero;
        }

        /// <summary>
        /// Adds selected object.
        /// </summary>
        /// <param name="selectedObject">Object selected.</param>
        public void AddSelectedObject(GameObject selectedObject)
        {
            if (_selectedObjects.Contains(selectedObject))
            {
                UnityEngine.Debug.LogWarning("Selected object already added: " + selectedObject.name);
                return;
            }
            
            _selectedObjects.Add(selectedObject);
            UpdateBox();
        }

        /// <summary>
        /// Remove selected object.
        /// </summary>
        /// <param name="selectedObject">Object deselected.</param>
        public void RemoveSelectedObject(GameObject selectedObject)
        {
            if (!_selectedObjects.Contains(selectedObject))
            {
                UnityEngine.Debug.LogWarning("Selected object not selected: " + selectedObject.name);
                return;
            }
            
            _selectedObjects.Remove(selectedObject);
            UpdateBox();
        }

        /// <summary>
        /// Updates the box position and size.
        /// </summary>
        public void UpdateBox()
        {
            if (_selectedObjects.Count == 0)
            {
                SetEnabled(false);
                return;
            }

            SetEnabled(true);
            
            _minPos = new Vector2(float.MaxValue, float.MaxValue);
            _maxPos = new Vector2(float.MinValue, float.MinValue);

            foreach (var obj in _selectedObjects)
            {
                Bounds bounds = GetBoundingBox(obj);
                Vector2 boundsMin = bounds.min;
                Vector2 boundsMax = bounds.max;

                if (boundsMin.x < _minPos.x)
                {
                    _minPos.x = boundsMin.x;
                }

                if (boundsMin.y < _minPos.y)
                {
                    _minPos.y = boundsMin.y;
                }

                if (boundsMax.x > _maxPos.x)
                {
                    _maxPos.x = boundsMax.x;
                }

                if (boundsMax.y > _maxPos.y)
                {
                    _maxPos.y = boundsMax.y;
                }
            }
            
            Vector2 leftTop = new Vector2(_minPos.x, _maxPos.y);
            Vector2 rightBottom = new Vector2(_maxPos.x, _minPos.y);

            leftBottomCorner.transform.position = _minPos;
            leftTopCorner.transform.position = leftTop;
            rightTopCorner.transform.position = _maxPos;
            rightBottomCorner.transform.position = rightBottom;
            
            NativeArray<Vector3> leftSidePoints = new NativeArray<Vector3>(2, Allocator.Temp);
            leftSidePoints[0] = _minPos;
            leftSidePoints[1] = leftTop;
            
            NativeArray<Vector3> topSidePoints = new NativeArray<Vector3>(2, Allocator.Temp);
            topSidePoints[0] = leftTop;
            topSidePoints[1] = _maxPos;
            
            NativeArray<Vector3> rightSidePoints = new NativeArray<Vector3>(2, Allocator.Temp);
            rightSidePoints[0] = _maxPos;
            rightSidePoints[1] = rightBottom;
            
            NativeArray<Vector3> bottomSidePoints = new NativeArray<Vector3>(2, Allocator.Temp);
            bottomSidePoints[0] = rightBottom;
            bottomSidePoints[1] = _minPos;
            
            leftSide.SetPositions(leftSidePoints);
            topSide.SetPositions(topSidePoints);
            rightSide.SetPositions(rightSidePoints);
            bottomSide.SetPositions(bottomSidePoints);
            
            leftSidePoints.Dispose();
            topSidePoints.Dispose();
            rightSidePoints.Dispose();
            bottomSidePoints.Dispose();
        }

        private Bounds GetBoundingBox(GameObject obj)
        {
            SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
            if (!sprite)
            {
                return new Bounds(obj.transform.position, Vector3.one);
            }
            
            return sprite.bounds;
        }

        private void SetEnabled(bool enable)
        {
            leftBottomCorner.enabled = enable;
            leftTopCorner.enabled = enable;
            rightTopCorner.enabled = enable;
            rightBottomCorner.enabled = enable;
            
            leftSide.enabled = enable;
            topSide.enabled = enable;
            rightSide.enabled = enable;
            bottomSide.enabled = enable;
        }
    }
}
