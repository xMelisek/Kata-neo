using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KataNeo
{
    public class Tile
    {
        public Vector2 position;
        public Texture2D sprite;

        public Tile(Vector2 position, Texture2D sprite)
        {
            this.position = position;
            this.sprite = sprite;
        }

        public Rectangle Rect
        {
            get => new Rectangle((int)position.X - sprite.Width / 2, (int)position.Y - sprite.Height / 2,
            sprite.Width, sprite.Height);
        }
    }
}