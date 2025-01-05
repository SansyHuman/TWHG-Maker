using System;
using SansyHuman.TWHG.Core;
using UnityEngine;

namespace SansyHuman.TWHG.World
{
    /// <summary>
    /// The class to manage tiles in the grid system.
    /// </summary>
    public class GridTileSystem : GridSystem<GridTileSystem.Tile?>
    {
        /// <summary>
        /// Struct that contains components consisting a tile.
        /// </summary>
        [Serializable]
        public struct Tile
        {
            /// <summary>
            /// Front tile face.
            /// </summary>
            public SpriteRenderer MainTile;
            
            /// <summary>
            /// Tile outline.
            /// </summary>
            public SpriteRenderer Outline;
        }
        
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
        
        public override Tile? AddTile(int x, int y)
        {
            if (_tiles.ContainsKey(new Vector2Int(x, y)))
            {
                UnityEngine.Debug.LogWarning("Tile already exists.");
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

            Tile newTileComps = new Tile { MainTile = newTile, Outline = newOutline };
            
            _tiles.Add(new Vector2Int(x, y), newTileComps);
            
            onTileAdded.Invoke(newTileComps);

            return newTileComps;
        }
        
        public override bool RemoveTile(int x, int y)
        {
            if (!_tiles.ContainsKey(new Vector2Int(x, y)))
            {
                UnityEngine.Debug.LogWarning("Tile does not exists.");
                return false;
            }
            
            Tile tile = _tiles[new Vector2Int(x, y)].Value;
            onTileRemoved.Invoke(tile);
            Destroy(tile.MainTile.gameObject);
            _tiles.Remove(new Vector2Int(x, y));

            return true;
        }

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
                        _tiles[index].Value.MainTile.color = value;
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
                        _tiles[index].Value.MainTile.color = value;
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
                    tile.Value.Outline.transform.localScale = new Vector3(1 + tileOutlineThickness, 1 + tileOutlineThickness, 1);
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
                    tile.Value.Outline.color = value;
                }
            }
        }
    }
}
