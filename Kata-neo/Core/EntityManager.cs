using KataNeo.Animation;
using KataNeo.Entitites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace KataNeo
{
    public class EntityManager
    {
        private GameWindow gameWindow;
        public List<Entity> entities;
        public List<Player> players;
        /// <summary>
        /// Entities to be disposed. They are disposed at the end of the frame
        /// </summary>
        private List<Entity> _toDispose;

        public EntityManager(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;
            entities = new List<Entity>();
            players = new List<Player>();
            _toDispose = new List<Entity>();
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
        
        /// <summary>
        /// Checks if the rect can damage other players.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name=""></param>
        public void CheckDamage(Rectangle rect, Player player)
        {
            foreach (var plr in players)
            {
                if(plr == player) continue;
                if (rect.Intersects(plr.Rect))
                {
                    // Add the player to be damaged
                    _toDispose.Add(plr);
                }
            }
        }

        #region Game Loop Updates
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
            foreach(var entity in _toDispose)
            {
                if (entity.GetType() == typeof(Player))
                {
                    players.Remove((Player)entity);
                    if (players.Count == 1) Debug.WriteLine($"{players[0]} won!");
                }
                else entities.Remove(entity);
            }
            _toDispose.Clear();
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
        #endregion
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
