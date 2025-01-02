using SansyHuman.TWHG.Core;
using UnityEngine;

namespace SansyHuman.TWHG.World
{
    public class GridWallSystem : GridSystem<GridWallSystem.Wall?>
    {
        /// <summary>
        /// Struct that contains components consisting a wall.
        /// </summary>
        public struct Wall
        {
            /// <summary>
            /// Collider of the wall.
            /// </summary>
            public Collider2D Collider;
        
            /// <summary>
            /// Sprite for debug.
            /// </summary>
            public SpriteRenderer DebugSprite;
        }

        [Tooltip("The wall prefab to use. It should be a 1 x 1 size static collider.")]
        [SerializeField] private Collider2D wall;

        [Tooltip("The sprite to use as a debug sprite.")]
        [SerializeField] private Sprite debugSprite;

        [Tooltip("The material to use to a debug sprite.")]
        [SerializeField] private Material debugMaterial;
    
        [Tooltip("The color of debug sprite to denote the wall.")]
        [SerializeField] private Color debugColor;

        [Tooltip("The render order of the debug sprite.")]
        [SerializeField] private int debugRenderOrder = 0;

        [Tooltip("If true show the debug sprites.")]
        [SerializeField] private bool debug = false;


        public override Wall? AddTile(int x, int y)
        {
            if (_tiles.ContainsKey(new Vector2Int(x, y)))
            {
                UnityEngine.Debug.LogWarning("Tile already exists.");
                return null;
            }
            
            Vector3 tilePosition = GetGridPosition(x, y);
            
            Collider2D newWall = Instantiate(wall, tilePosition, Quaternion.identity, transform);
            newWall.transform.localScale = new Vector3(gridSize, gridSize, 1);
            newWall.name = $"Wall {x}, {y}";

            SpriteRenderer newSprite = newWall.GetComponent<SpriteRenderer>();
            if (!newSprite)
            {
                newSprite = newWall.gameObject.AddComponent<SpriteRenderer>();
                newSprite.sprite = debugSprite;
                newSprite.material = debugMaterial;
            }
            Color debugC = debugColor;
            debugC.a *= debug ? 1 : 0;
            newSprite.color = debugC;
            
            Wall newWallComps = new Wall() { Collider = newWall, DebugSprite = newSprite };
            
            _tiles.Add(new Vector2Int(x, y), newWallComps);

            return newWallComps;
        }

        public override bool RemoveTile(int x, int y)
        {
            if (!_tiles.ContainsKey(new Vector2Int(x, y)))
            {
                UnityEngine.Debug.LogWarning("Tile does not exists.");
                return false;
            }
            
            Wall wall = _tiles[new Vector2Int(x, y)].Value;
            Destroy(wall.Collider.gameObject);
            _tiles.Remove(new Vector2Int(x, y));

            return true;
        }

        /// <summary>
        /// Gets and sets the debug mode.
        /// </summary>
        public bool Debug
        {
            get => debug;
            set
            {
                debug = value;
                Color debugC = debugColor;
                debugC.a *= debug ? 1 : 0;
                
                foreach (var wall in _tiles.Values)
                {
                    wall.Value.DebugSprite.color = debugC;
                }
            }
        }
    }
}
