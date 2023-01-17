﻿using KataNeo.Data;
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
        public List<Tile> tiles = new List<Tile>();

        public MapManager()
        {

        }

        #region Map Management
        //Load map from data
        [Obsolete]
        public void LoadMapJSON(string path, GameWindow gameWindow)
        {
            var map = JsonSerializer.Deserialize<Map>(File.ReadAllText(path));
            bg = gameWindow.Content.Load<Texture2D>($"BGs/{map.BG}");
            foreach (var tile in map.Tiles)
            {
                tiles.Add(new Tile(new Vector2(tile.PosX, tile.PosY),
                    gameWindow.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
            }
        }

        [Obsolete]
        public void LoadMapXML(string path, GameWindow gameWindow)
        {
            var serializer = new XmlSerializer(typeof(Map));
            using var fileStream = new FileStream(path, FileMode.Open);
            var map = (Map)serializer.Deserialize(fileStream);
            bg = gameWindow.Content.Load<Texture2D>($"BGs/{map.BG}");
            foreach (var tile in map.Tiles)
            {
                tiles.Add(new Tile(new Vector2(tile.PosX, tile.PosY),
                    gameWindow.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
            }
        }

        public void LoadMap(string path, GameWindow gameWindow)
        {
            try
            {
                var map = JsonSerializer.Deserialize<Map>(File.ReadAllText(path));
                bg = gameWindow.Content.Load<Texture2D>($"BGs/{map.BG}");
                foreach (var tile in map.Tiles)
                {
                    tiles.Add(new Tile(new Vector2(tile.PosX, tile.PosY),
                        gameWindow.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
                }
                return;
            }
            catch (JsonException)
            {
                Debug.WriteLine("File was not in JSON format, trying to parse as XML");
                try
                {
                    var serializer = new XmlSerializer(typeof(Map));
                    using var fileStream = new FileStream(path, FileMode.Open);
                    var map = (Map)serializer.Deserialize(fileStream);
                    bg = gameWindow.Content.Load<Texture2D>($"BGs/{map.BG}");
                    foreach (var tile in map.Tiles)
                    {
                        tiles.Add(new Tile(new Vector2(tile.PosX, tile.PosY),
                            gameWindow.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
                    }
                    return;
                }
                catch (InvalidOperationException)
                {
                    throw new Exception("File format was not in json or xml or it has errors in styntax");
                }
            }
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
                new Vector2(tile.sprite.Width / 2, tile.sprite.Height / 2), 1f, SpriteEffects.None, 0f);
        }
        #endregion
    }
}