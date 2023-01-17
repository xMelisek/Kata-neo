using KataNeo.Animation;
using KataNeo.Entitites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KataNeo
{
    public class EntityManager
    {
        private GameWindow gameWindow;
        private List<Entity> _entities;
        public List<Player> _players;

        public EntityManager(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;
            _entities = new List<Entity>();
            _players = new List<Player>();
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        /// <summary>
        /// Adds a player to the game. Call this only after LoadContent was called
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="mapManager"></param>
        public void AddPlayer(ControlType controlType, MapManager mapManager)
        {
            _players.Add(new Player(controlType, mapManager, GetAllPlayerAnims()));
        }

        //Helper functions for player Initializing
        private AnimData GetAllPlayerAnims()
        {
            //Get all of the animations with directories to them
            var sheets = MonoHelp.LoadAllContent<Texture2D>(gameWindow, "Player", true, true);
            List<Anim> anims = new List<Anim>();
            //Add all of the animations and get the intervals from their directories
            for (int i = 0; i < sheets.Item1.Length; i++)
            {
                anims.Add(new Anim(sheets.Item1[i], GetIntervals(sheets.Item2[i])));
            }
            if (anims.Count == 0) throw new System.Exception("There are no animations in directory");
            List<string> keys = new List<string>();
            foreach (var dir in sheets.Item2)
            {
                int slash = dir.LastIndexOf('/');
                int backslash = dir.LastIndexOf('\\');
                keys.Add(backslash > slash ? dir.Remove(0, backslash + 1) : dir.Remove(0, slash + 1));
            }
            return new AnimData(anims.ToArray(), keys.ToArray());
        }

        private float[] GetIntervals(string path)
        {
            return JsonSerializer.Deserialize<float[]>(File.ReadAllText("Data/Animations" + path.Remove(0, 7) + "/intervals.json"));
        }

        public void Update(GameTime gametime, InputState inputState)
        {
            foreach (var player in _players)
            {
                if (player.controlType == ControlType.Keyboard)
                    player.InputUpdate(inputState.keyboardState, gametime);
                else
                    player.InputUpdate(inputState.gamePads[(int)player.controlType - 1], gametime);
                player.Update(gametime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var entity in _entities)
            {
                entity.Draw(gameTime, spriteBatch);
            }

            foreach (var player in _players)
            {
                player.Draw(gameTime, spriteBatch);
            }
        }
    }

    public struct InputState
    {
        public KeyboardState keyboardState;
        public GamePadState[] gamePads;

        public InputState(KeyboardState keyboardState, GamePadState[] gamePads)
        {
            this.keyboardState = keyboardState;
            this.gamePads = gamePads;
        }
    }

    public enum ControlType
    {
        Keyboard,
        //Keyboard2, //TODO: Implement it later
        Gamepad1,
        Gamepad2,
        Gamepad3,
        Gamepad4,
    }
}
