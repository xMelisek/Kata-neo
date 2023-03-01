using Microsoft.Xna.Framework;

namespace KataNeo.Data
{
    public class Map
    {
        public Map(string type, string name, string bG, Vector2[] spawnPoses, TileData[] tiles)
        {
            Type = type;
            Name = name;
            BG = bG;
            SpawnPoses = spawnPoses;
            Tiles = tiles;
        }

        public Map()
        {

        }

        public string Type { get; set; }
        public string Name { get; set; }
        public string BG { get; set; }
        public Vector2[] SpawnPoses { get; set; }
        public TileData[] Tiles { get; set; }
    }
}