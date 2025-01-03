using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.TWHG.Core
{
    /// <summary>
    /// Base class of grid system.
    /// </summary>
    /// <typeparam name="T">Type of tiles to place at grid.</typeparam>
    public abstract class GridSystem<T> : MonoBehaviour
    {
        [Tooltip("The world space size of a grid.")]
        [SerializeField] protected float gridSize = 1.0f;

        protected Dictionary<Vector2Int, T> _tiles;
        
        protected virtual void Start()
        {
            transform.position = Vector3.zero;
            _tiles = new Dictionary<Vector2Int, T>();
        }

        /// <summary>
        /// Adds a tile at the grid index. If the tile already exists at the index, returns null.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        /// <returns>Added tile. If the tile already exists at the index, returns null.</returns>
        public abstract T AddTile(int x, int y);
        
        /// <summary>
        /// Removes a tile at the grid index.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        /// <returns>True if the tile removed. Else, false.</returns>
        public abstract bool RemoveTile(int x, int y);

        /// <summary>
        /// Gets the tile at the grid index.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        /// <returns>Object at the grid index. If does not exist, returns null.</returns>
        public T GetTile(int x, int y)
        {
            if (!DoesTileExist(x, y))
            {
                UnityEngine.Debug.LogWarning($"Tile {x}, {y} does not exist.");
                return default(T);
            }
            
            return _tiles[new Vector2Int(x, y)];
        }
        
        /// <summary>
        /// Gets the grid index of the position in world space.
        /// </summary>
        /// <param name="xPos">X position in world space.</param>
        /// <param name="yPos">Y position in world space.</param>
        /// <returns>Grid index of the position.</returns>
        public Vector2Int GetGridIndex(float xPos, float yPos)
        {
            return (Vector2Int)Math.Floor(new Vector2(xPos / gridSize, yPos / gridSize));
        }
        
        /// <summary>
        /// Gets the center position of the grid in world space.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        /// <returns>Center position of the grid.</returns>
        public Vector2 GetGridPosition(int x, int y)
        {
            float halfGridSize = gridSize / 2;
            return new Vector2(x * gridSize + halfGridSize, y * gridSize + halfGridSize);
        }
        
        /// <summary>
        /// Gets the size of a grid in world space.
        /// </summary>
        public float GridSize => gridSize;
        
        /// <summary>
        /// Gets the half size of a grid in world space.
        /// </summary>
        public float HalfGridSize => gridSize / 2;
        
        /// <summary>
        /// Returns true if the tile exists at the grid index.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        public bool DoesTileExist(int x, int y)
        {
            return _tiles.ContainsKey(new Vector2Int(x, y));
        }
        
        /// <summary>
        /// Returns true if at least one tile exists at the adjacent grids.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        public bool DoesTileExistAtAdjacentGrids(int x, int y)
        {
            return _tiles.ContainsKey(new Vector2Int(x + 1, y))
                   || _tiles.ContainsKey(new Vector2Int(x - 1, y))
                   || _tiles.ContainsKey(new Vector2Int(x, y + 1))
                   || _tiles.ContainsKey(new Vector2Int(x, y - 1));
        }
    }
}
