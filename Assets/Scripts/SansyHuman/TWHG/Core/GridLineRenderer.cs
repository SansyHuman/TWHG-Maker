using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SansyHuman.TWHG.Core
{
    /// <summary>
    /// Low-level renderer that renders grid lines at main camera with world space interval 1.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class GridLineRenderer : MonoBehaviour
    {
        [Tooltip("Material of grid line.")]
        [SerializeField] private Material lineMaterial;
        
        [Tooltip("Color of grid line.")]
        [SerializeField] private Color lineColor = Color.white;
    
        [Tooltip("Width of the grid line in world space.")]
        [SerializeField] private float lineThickness = 0.05f;
        
        [Tooltip("Distance between the adjacent grid lines.")]
        [SerializeField] private float gridInterval = 1f;
        
        private float _xMinInViewport;
        private float _xMaxInViewport;
        private float _yMinInViewport;
        private float _yMaxInViewport;
        private float _xWidthInViewport;
        private float _yWidthInViewport;

        private int _xGridCount;
        private int _yGridCount;

        private Camera _camera;

        void Start()
        {
            _camera = GetComponent<Camera>();
        }
    
        // Update is called once per frame
        void Update()
        {
            Vector3 worldMin = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 worldMax = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
            
            Vector3Int gridWorldMin = Math.Ceil(worldMin / gridInterval);
            Vector3Int gridWorldMax = Math.Floor(worldMax / gridInterval);
            
            _xGridCount = Mathf.Abs(gridWorldMax.x - gridWorldMin.x) + 1;
            _yGridCount = Mathf.Abs(gridWorldMax.y - gridWorldMin.y) + 1;
            
            Vector3 gridViewportMin = _camera.WorldToViewportPoint((Vector3)gridWorldMin * gridInterval);
            Vector3 gridViewportMax = _camera.WorldToViewportPoint((Vector3)gridWorldMax * gridInterval);

            _xMinInViewport = gridViewportMin.x;
            _xMaxInViewport = gridViewportMax.x;
            _yMinInViewport = gridViewportMin.y;
            _yMaxInViewport = gridViewportMax.y;
            
            Vector3 lineWidth = _camera.WorldToViewportPoint(new Vector3(lineThickness, lineThickness, 0)) - _camera.WorldToViewportPoint(Vector3.zero);
            _xWidthInViewport = lineWidth.x;
            _yWidthInViewport = lineWidth.y;
        }

        void OnPostRender()
        {
            if (!lineMaterial)
            {
                UnityEngine.Debug.LogError("Grid line material not found.");
                return;
            }
            
            GL.PushMatrix();
            lineMaterial.SetPass(0);
            
            GL.LoadOrtho();
            
            GL.Begin(GL.QUADS);
            GL.Color(lineColor);
            
            float xDist = _xMaxInViewport - _xMinInViewport;
            float yDist = _yMaxInViewport - _yMinInViewport;

            for (int i = 0; i < _xGridCount; i++)
            {
                float lineX = _xMinInViewport + xDist * i / (_xGridCount - 1);
                float halfWidth = _xWidthInViewport / 2;
                    
                GL.Vertex3(lineX - halfWidth, 0, 0);
                GL.Vertex3(lineX - halfWidth, 1, 0);
                GL.Vertex3(lineX + halfWidth, 1, 0);
                GL.Vertex3(lineX + halfWidth, 0, 0);
            }

            for (int i = 0; i < _yGridCount; i++)
            {
                float lineY = _yMinInViewport + yDist * i / (_yGridCount - 1);
                float halfWidth = _yWidthInViewport / 2;
                
                GL.Vertex3(0, lineY - halfWidth, 0);
                GL.Vertex3(1, lineY - halfWidth, 0);
                GL.Vertex3(1, lineY + halfWidth, 0);
                GL.Vertex3(0, lineY + halfWidth, 0);
            }
            
            GL.End();
            GL.PopMatrix();
        }

        private void OnEnable()
        {
            RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        }
        
        private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            OnPostRender();
        }
    }
}
