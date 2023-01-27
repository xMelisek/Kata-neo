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
        public List<Entity> entities;
        public List<Player> players;

        public EntityManager(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;
            entities = new List<Entity>();
            players = new List<Player>();
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        /// <summary>
        /// Adds a player to the game. Call this only after LoadContent was called
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="mapManager"></param>
        public void AddPlayer(ControlType controlType, MapManager mapManager, Vector2 position)
        {
            players.Add(new Player(controlType, mapManager, MonoHelp.GetAllAnims("Player"), position));
        }

        public void Update(GameTime gametime)
        {
            foreach (var player in players)
            {
                if (player.controlType == ControlType.Keyboard)
                    player.KeyboardUpdate(gametime);
                else
                    player.GamepadUpdate(gametime);
                player.Update(gametime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
            {
                entity.Draw(gameTime, spriteBatch);
            }

            foreach (var player in players)
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
