using System;
using System.Collections.Generic;
using UnityEngine;

namespace SansyHuman.TWHG.World
{
    /// <summary>
    /// The class to manage tiles in the grid system.
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        [Tooltip("The world space size of a tile.")]
        [SerializeField] private float tileSize = 1.0f;
        
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

        private Dictionary<Vector2Int, SpriteRenderer> _tiles;

        private void Start()
        {
            transform.position = Vector3.zero;
            _tiles = new Dictionary<Vector2Int, SpriteRenderer>();
            
            // For debug.
            /*
            AddTile(0, 0);
            AddTile(0, 1);
            AddTile(0, 2);
            AddTile(1, 2);
            */
        }

        /// <summary>
        /// Add a tile at the grid index. If the tile already exists at the index, returns null.
        /// </summary>
        /// <param name="x">X grid index.</param>
        /// <param name="y">Y grid index.</param>
        /// <returns>Added tile. If the tile already exists at the index, returns null.</returns>
        public SpriteRenderer AddTile(int x, int y)
        {
            if (_tiles.ContainsKey(new Vector2Int(x, y)))
            {
                Debug.Log("Tile already exists.");
                return null;
            }
            
            float halfTileSize = tileSize / 2;
            Vector3 tilePosition = new Vector3(x * tileSize + halfTileSize, y * tileSize + halfTileSize, 0.0f);
            
            SpriteRenderer newTile = Instantiate(tileSprite, tilePosition, Quaternion.identity, transform);
            SpriteRenderer newOutline = Instantiate(tileSprite, tilePosition, Quaternion.identity, newTile.transform);
            
            newTile.color = (x + y) % 2 == 0 ? tileColor1 : tileColor2;
            newTile.transform.localScale = new Vector3(tileSize, tileSize, 1);
            newTile.sortingOrder = tileRenderOrder;
            newTile.name = $"Tile {x}, {y}";
            
            newOutline.color = tileOutlineColor;
            newOutline.transform.localScale = new Vector3(1 + tileOutlineThickness, 1 + tileOutlineThickness, 1);
            newOutline.sortingOrder = tileRenderOrder - 1;
            newOutline.name = $"Outline";
            
            _tiles.Add(new Vector2Int(x, y), newTile);

            return newTile;
        }
    }
}
