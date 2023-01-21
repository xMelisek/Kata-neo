using KataNeo.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace KataNeo
{
    public class MapManager
    {
        private Texture2D bg;
        public Vector2[] spawnPoses;
        public List<Tile> tiles = new List<Tile>();

        #region Map Management
        //Load map from data
        public void LoadMap(string path, GameWindow gameWindow)
        {
            path += ".knm";
            try
            {
                var serializer = new XmlSerializer(typeof(Map));
                using var fileStream = new FileStream(path, FileMode.Open);
                var map = (Map)serializer.Deserialize(fileStream);
                bg = gameWindow.Content.Load<Texture2D>($"BGs/{map.BG}");
                spawnPoses = map.SpawnPoses;
                foreach (var tile in map.Tiles)
                {
                    tiles.Add(new Tile(tile.Position, tile.Scale,
                        gameWindow.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
                }
                return;
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("File was not in XML format, trying to parse as JSON");
                try
                {
                    var map = JsonSerializer.Deserialize<Map>(File.ReadAllText(path));
                    bg = gameWindow.Content.Load<Texture2D>($"BGs/{map.BG}");
                    spawnPoses = map.SpawnPoses;
                    foreach (var tile in map.Tiles)
                    {
                        tiles.Add(new Tile(tile.Position, tile.Scale,
                            gameWindow.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
                    }
                    return;
                }
                catch (JsonException)
                {
                    throw new Exception("File format was not in json/xml or it has errors in styntax");
                }
            }
        }

        public void ExportMap(string path)
        {
            path += ".knm";
            var map = new Map();
            map.Name = "PlaygroundMap";
            map.SpawnPoses = new Vector2[] { new Vector2(64, 540), new Vector2(192, 540), new Vector2(1728, 540), new Vector2(1856, 540) };
            map.BG = "Default";
            map.Tiles = new TileData[1] { new TileData() };
            map.Tiles[0].Position = new Vector2(960, 540);
            map.Tiles[0].Scale = new Vector2(1, 1);
            map.Tiles[0].Sprite = "Default";
            var serializer = new XmlSerializer(typeof(Map));
            TextWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, map);
        }

        public void StartMap()
        {

        }

        public void SwitchMap()
        {

        }
        #endregion

        #region Game Loop Updates
        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw bg
            spriteBatch.Draw(bg, Vector2.Zero, Color.White);
            //Then tiles
            foreach (var tile in tiles)
                spriteBatch.Draw(tile.sprite, tile.position, null, Color.White, 0f,
                new Vector2(tile.sprite.Width / 2, tile.sprite.Height / 2), tile.scale, SpriteEffects.None, 0f);
        }
        #endregion
    }
}