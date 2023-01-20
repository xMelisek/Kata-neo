﻿using Microsoft.Xna.Framework;

namespace KataNeo.Data
{
    public class Map
    {
        public string Name { get; set; }
        public string BG { get; set; }
        public Vector2[] SpawnPoses { get; set; }
        public TileData[] Tiles { get; set; }
    }
}