using KataNeo.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace KataNeo.Entities
{
    public class Attack
    {
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

        public Attack(Vector2 offset)
        {
            dir = offset;
            offset.Y = -offset.Y;
            this.offset = offset;
            AnimData animData = MonoHelp.GetAllAnims("Attack");
            animator = new Animator(ref sprite, animData.GetAnim(0));
        }

        public void Update(GameTime gameTime, Vector2 pos)
        {
            position = pos + offset;
            sprite = animator.Update(gameTime);
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
            else if(dir.X < 0)
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
    }
}