using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KataNeo.Data
{
    public class Map
    {
        public Map()
        {

        }

        public Map(string type, string name, string bG, Vector2[] spawnPoses, BgData[] background, TileData[] tiles, FgData[] foreground)
        {
            Type = type;
            Name = name;
            BG = bG;
            SpawnPoses = spawnPoses;
            Background = background;
            Tiles = tiles;
            Foreground = foreground;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public string BG { get; set; }
        public Vector2[] SpawnPoses { get; set; }
        public BgData[] Background { get; set; }
        public TileData[] Tiles { get; set; }
        public FgData[] Foreground { get; set; }
    }

    /// <summary>
    /// Foreground object. Renders on top of tiles
    /// </summary>
    public struct FgObj
    {
        public Vector2 position;
        public Vector2 scale;
        public Texture2D sprite;

        public FgObj(Vector2 position, Vector2 scale, Texture2D sprite)
        {
            this.position = position;
            this.scale = scale;
            this.sprite = sprite;
        }
    }

    /// <summary>
    /// Background object. Renders behind tiles
    /// </summary>
    public struct BgObj
    {
        public Vector2 position;
        public Vector2 scale;
        public Texture2D sprite;

        public BgObj(Vector2 position, Vector2 scale, Texture2D sprite)
        {
            this.position = position;
            this.scale = scale;
            this.sprite = sprite;
        }
    }
}