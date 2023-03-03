using KataNeo.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace KataNeo
{
    public class MapManager
    {
        public delegate void LoadCallback();
        private Texture2D bg;
        public Vector2[] spawnPoses;
        public List<BgObj> bgObjs = new List<BgObj>();
        public List<Tile> tiles = new List<Tile>();
        public List<FgObj> fgObjs = new List<FgObj>();
        string currentMap;
        List<string> maps;

        public MapManager() 
        {
            maps = Directory.GetFiles("Maps").ToList();
            maps.AddRange(Directory.GetFiles("Custom", "*.knm"));
            LoadMapData(maps[0]);
            maps.RemoveAt(0);
        }

        #region Map Management
        //Load map from data
        public void LoadMapData(string path)
        {
            if (!path.EndsWith(".knm")) path += ".knm";
            currentMap = path;
            try
            {
                Map map;
                //Serialize map into Map class
                using (var fileStream = new FileStream(path, FileMode.Open))
                    map = (Map)new XmlSerializer(typeof(Map)).Deserialize(fileStream);
                //Set map data to manager vars
                spawnPoses = map.SpawnPoses;
                if (map.Type == "Builtin")
                {
                    bg = MonoHelp.Content.Load<Texture2D>($"BGs/{map.BG}");
                    foreach (var tile in map.Tiles)
                    {
                        tiles.Add(new Tile(tile.Position, tile.Scale,
                            MonoHelp.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
                    }
                }
                else if (map.Type == "Custom")
                {
                    using (var fileStream = new FileStream($"Custom/Content/BGs/{map.BG}.png", FileMode.Open))
                        bg = Texture2D.FromStream(MonoHelp.GameWindow.GraphicsDevice, fileStream);
                    foreach (var bgObj in map.Background)
                    {
                        using (var fileStream = new FileStream($"Custom/Content/Tiles/{bgObj.Sprite}.png", FileMode.Open))
                            bgObjs.Add(new BgObj(bgObj.Position, bgObj.Scale,
                                Texture2D.FromStream(MonoHelp.GameWindow.GraphicsDevice, fileStream)));
                    }
                    foreach (var tile in map.Tiles)
                    {
                        using (var fileStream = new FileStream($"Custom/Content/Tiles/{tile.Sprite}.png", FileMode.Open))
                            tiles.Add(new Tile(tile.Position, tile.Scale,
                                Texture2D.FromStream(MonoHelp.GameWindow.GraphicsDevice, fileStream)));
                    }
                    foreach (var fgObj in map.Foreground)
                    {
                        using (var fileStream = new FileStream($"Custom/Content/Tiles/{fgObj.Sprite}.png", FileMode.Open))
                            fgObjs.Add(new FgObj(fgObj.Position, fgObj.Scale,
                                Texture2D.FromStream(MonoHelp.GameWindow.GraphicsDevice, fileStream)));
                    }
                }
                else throw new Exception("Invalid map type");
                return;
            }
            catch (InvalidOperationException)
            {
                Debug.WriteLine("File was not in XML format, trying to parse as JSON");
                try
                {
                    var map = JsonSerializer.Deserialize<Map>(File.ReadAllText(path));
                    bg = MonoHelp.Content.Load<Texture2D>($"BGs/{map.BG}");
                    spawnPoses = map.SpawnPoses;
                    foreach (var tile in map.Tiles)
                    {
                        tiles.Add(new Tile(tile.Position, tile.Scale,
                            MonoHelp.Content.Load<Texture2D>($"Tiles/{tile.Sprite}")));
                    }
                    return;
                }
                catch (JsonException)
                {
                    throw new Exception("File format was not in json/xml or it has errors in syntax");
                }
            }
        }

        public void ExportMap(string path)
        {
            path += ".knm";
            var map = new Map();
            map.Type = "Custom";
            map.Name = "PlaygroundMap";
            map.SpawnPoses = new Vector2[] { new Vector2(64, 540), new Vector2(192, 540), new Vector2(1728, 540), new Vector2(1856, 540) };
            map.BG = "Default";
            map.Background = new BgData[1] { new BgData() };
            map.Background[0].Position = new Vector2(960, 520);
            map.Background[0].Scale = new Vector2(1, 1);
            map.Background[0].Sprite = "Default";
            map.Tiles = new TileData[1] { new TileData() };
            map.Tiles[0].Position = new Vector2(960, 540);
            map.Tiles[0].Scale = new Vector2(1, 1);
            map.Tiles[0].Sprite = "Default";
            map.Foreground = new FgData[1] { new FgData() };
            map.Foreground[0].Position = new Vector2(960, 520);
            map.Foreground[0].Scale = new Vector2(1, 1);
            map.Foreground[0].Sprite = "Default";
            var serializer = new XmlSerializer(typeof(Map));
            TextWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, map);
            writer.Dispose();
        }

        public void LoadMap(LoadCallback callback)
        {
            bg = null;
            spawnPoses = null;
            tiles.Clear();
            if (maps.Count == 0)
            {
                maps = Directory.GetFiles("Maps").ToList();
                maps.AddRange(Directory.GetFiles("Custom", "*.knm"));
            }
            LoadMapData(maps[0]);
            maps.RemoveAt(0);
            callback();
        }

        public void ReloadMap()
        {
            bg = null;
            spawnPoses = null;
            tiles.Clear();
            LoadMapData(currentMap);
        }
        #endregion

        #region Game Loop Updates
        public void Update(GameTime gameTime)
        {
            if (MonoHelp.GetKeyDown(Keys.R)) ReloadMap();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw bg
            spriteBatch.Draw(bg, Vector2.Zero, Color.White);
            //Then tiles
            foreach(var bgObj in bgObjs)
                spriteBatch.Draw(bgObj.sprite, bgObj.position, null, Color.White, 0f,
                new Vector2(bgObj.sprite.Width / 2, bgObj.sprite.Height / 2), bgObj.scale, SpriteEffects.None, 0f);
            foreach (var tile in tiles)
                spriteBatch.Draw(tile.sprite, tile.position, null, Color.White, 0f,
                new Vector2(tile.sprite.Width / 2, tile.sprite.Height / 2), tile.scale, SpriteEffects.None, 0f);
            foreach (var fgObj in fgObjs)
                spriteBatch.Draw(fgObj.sprite, fgObj.position, null, Color.White, 0f,
                new Vector2(fgObj.sprite.Width / 2, fgObj.sprite.Height / 2), fgObj.scale, SpriteEffects.None, 0f);
        }
        #endregion
    }
}