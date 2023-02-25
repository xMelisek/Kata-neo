using KataNeo.Entities;
using KataNeo.Entitites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
        /// Checks if the attack can damage other players.
        /// </summary>
        /// <param name="attack">The attack from a player</param>
        /// <param name="player">Player who casted this rect so he will be ingored in damage check</param>
        /// <param name="val">Damage value</param>
        public void CheckDamage(Attack attack, Player player, int val)
        {
            foreach (var plr in players)
            {
                if (plr == player) continue;
                if (attack.Rect.Intersects(plr.Rect) && !attack.playersHit.Contains(plr))
                {
                    attack.playersHit.Add(plr);
                    if (plr.TakeDamage(val)) _toDispose.Add(plr);
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
            foreach (var entity in _toDispose)
            {
                if (entity.GetType() == typeof(Player))
                {
                    players.Remove((Player)entity);
                    if (players.Count == 1) MonoHelp.GameWindow.Transition();
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
