using KataNeo.Animation;
using KataNeo.Entitites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace KataNeo.Entities
{
    public class Attack
    {
        Player player;
        Texture2D sprite;
        Vector2 position;
        Vector2 dir;
        Vector2 offset;
        Animator animator;
        Vector2 scale = new Vector2(2, 2);
        public Rectangle Rect
        {
            get => new Rectangle((int)(position.X - sprite.Width * scale.X / 2f), (int)(position.Y - sprite.Height * scale.Y / 2f),
            (int)(sprite.Width * scale.X), (int)(sprite.Height * scale.Y));
        }

        // List of players hit so they won't get hit every frame instakilling them
        public List<Player> playersHit = new List<Player>();

        public Attack(Vector2 offset, Player player)
        {
            this.player = player;
            dir = offset;
            offset.Y = -offset.Y;
            this.offset = offset;
            AnimData animData = MonoHelp.GetAllAnims("Attack");
            animator = new Animator(ref sprite, animData.GetAnim(0), UpdateTex);
        }

        void UpdateTex(Texture2D tex) => sprite = tex;

        #region Game Loop Updates
        public void Update(GameTime gameTime, Vector2 pos)
        {
            position = pos + offset;
            MonoHelp.GameWindow.entityManager.CheckDamage(this, player, 35);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Get Attack position and rotation for the attack
            float rotation;
            if (dir.X > 0)
            {
                if (dir.Y > 0) rotation = -45f;
                else if (dir.Y < 0) rotation = 45f;
                else rotation = 0f;
            }
            else if (dir.X < 0)
            {
                if (dir.Y > 0) rotation = 45f;
                else if (dir.Y < 0) rotation = -45f;
                else rotation = 0f;
            }
            else
            {
                if (dir.Y > 0) rotation = -90f;
                else if (dir.Y < 0) rotation = 90f;
                else rotation = 0f;
            }
            spriteBatch.Draw(sprite, position, null, Color.White, rotation,
                new Vector2(sprite.Width / 2, sprite.Height / 2), scale, offset.X >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
        #endregion
    }
}