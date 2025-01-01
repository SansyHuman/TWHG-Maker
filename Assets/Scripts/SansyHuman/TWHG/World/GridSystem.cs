using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Math = SansyHuman.TWHG.Core.Math;

namespace SansyHuman.TWHG.World
{
    /// <summary>
    /// The class to manage tiles in the grid system.
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        /// <summary>
        /// Struct that contains components consisting a tile.
        /// </summary>
        public struct Tile
        {
            /// <summary>
            /// Front tile face.
            /// </summary>
            public SpriteRenderer tile;
            
            /// <summary>
            /// Tile outline.
            /// </summary>
            public SpriteRenderer outline;
        }
        
        [Tooltip("The world space size of a tile.")]
        [SerializeField] private float gridSize = 1.0f;
        
        [Tooltip("The sprite prefab to use as tiles. It should be 1 x 1 size in world space.")]
        [SerializeField] private SpriteRenderer tileSprite;

        [Tooltip("The first color of tiles.")]
        [SerializeField] private Color tileColor1 = Color.white;

        [Tooltip("The second color of tiles.")]
        [SerializeField] private Color tileColor2 = Color.gray;

        [Tooltip("The thickness of the outline of tiles relative to the tile size.")]
        [SerializeField] private float tileOutlineThickness = 0.2f;
        
        [Tooltip("The color of the outline of tiles.")]
        [SerializeField] private Color tileOutlineColor = Color.black;

        [Tooltip("The render order of the tile.")]
        [SerializeField] private int tileRenderOrder = -1;

        private Dictionary<Vector2Int, Tile> _tiles;

        private void Start()
        {
            transform.position = Vector3.zero;
            _tiles = new Dictionary<Vector2Int, Tile>();
            
            // For debug.
            /*
            AddTile(0, 0);
            AddTile(0, 1);
            AddTile(0, 2);
            AddTile(1, 2);
            */
        }

        /// <summary>
        /// Adds a tile at the grid index. If the tile already exists at the index, returns null.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        /// <returns>Added tile. If the tile already exists at the index, returns null.</returns>
        public Tile? AddTile(int x, int y)
        {
            if (_tiles.ContainsKey(new Vector2Int(x, y)))
            {
                UnityEngine.Debug.Log("Tile already exists.");
                return null;
            }
            
            Vector3 tilePosition = GetGridPosition(x, y);
            
            SpriteRenderer newTile = Instantiate(tileSprite, tilePosition, Quaternion.identity, transform);
            SpriteRenderer newOutline = Instantiate(tileSprite, tilePosition, Quaternion.identity, newTile.transform);
            
            newTile.color = (x + y) % 2 == 0 ? tileColor1 : tileColor2;
            newTile.transform.localScale = new Vector3(gridSize, gridSize, 1);
            newTile.sortingOrder = tileRenderOrder;
            newTile.name = $"Tile {x}, {y}";
            
            newOutline.color = tileOutlineColor;
            newOutline.transform.localScale = new Vector3(1 + tileOutlineThickness, 1 + tileOutlineThickness, 1);
            newOutline.sortingOrder = tileRenderOrder - 1;
            newOutline.name = "Outline";

            Tile newTileComps = new Tile { tile = newTile, outline = newOutline };
            
            _tiles.Add(new Vector2Int(x, y), newTileComps);

            return newTileComps;
        }

        /// <summary>
        /// Removes a tile at the grid index.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        public void RemoveTile(int x, int y)
        {
            if (!_tiles.ContainsKey(new Vector2Int(x, y)))
            {
                return;
            }
            
            Tile tile = _tiles[new Vector2Int(x, y)];
            Destroy(tile.tile.gameObject);
            _tiles.Remove(new Vector2Int(x, y));
        }

        /// <summary>
        /// Gets the grid index of the position in world space.
        /// </summary>
        /// <param name="xPos">X position in world space.</param>
        /// <param name="yPos">Y position in world space.</param>
        /// <returns>Grid index of the position.</returns>
        public Vector2Int GetGridIndex(float xPos, float yPos)
        {
            return (Vector2Int)Math.Floor(new Vector2(xPos, yPos));
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
        /// Gets and sets the first color of the tile. The first color is applied to tiles where the sum
        /// of x and y grid indices are even.
        /// </summary>
        public Color TileColor1
        {
            get => tileColor1;
            set
            {
                tileColor1 = value;
                foreach (var index in _tiles.Keys)
                {
                    if ((index.x + index.y) % 2 == 0)
                    {
                        _tiles[index].tile.color = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets and sets the second color of the tile. The second color is applied to tiles where the sum
        /// of x and y grid indices are even.
        /// </summary>
        public Color TileColor2
        {
            get => tileColor2;
            set
            {
                tileColor2 = value;
                foreach (var index in _tiles.Keys)
                {
                    if ((index.x + index.y) % 2 == 1)
                    {
                        _tiles[index].tile.color = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets and sets the thickness of the tile outline relative to the tile size.
        /// </summary>
        public float TileOutlineThickness
        {
            get => tileOutlineThickness;
            set
            {
                tileOutlineThickness = value;
                foreach (var tile in _tiles.Values)
                {
                    tile.outline.transform.localScale = new Vector3(1 + tileOutlineThickness, 1 + tileOutlineThickness, 1);
                }
            }
        }

        /// <summary>
        /// Gets and sets the color of the tile outline.
        /// </summary>
        public Color TileOutlineColor
        {
            get => tileOutlineColor;
            set
            {
                tileOutlineColor = value;
                foreach (var tile in _tiles.Values)
                {
                    tile.outline.color = value;
                }
            }
        }
    }
}
