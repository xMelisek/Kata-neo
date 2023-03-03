using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KataNeo.Data
{
    public class Tile
    {
        public Vector2 position;
        public Vector2 scale = Vector2.One;
        public Texture2D sprite;

        public Tile(Vector2 position, Vector2 scale, Texture2D sprite)
        {
            this.position = position;
            this.scale = scale;
            this.sprite = sprite;
        }

        public Rectangle Rect
        {
            get => new Rectangle((int)(position.X - sprite.Width * scale.X / 2f), (int)(position.Y - sprite.Height * scale.Y / 2f),
            (int)(sprite.Width * scale.X), (int)(sprite.Height * scale.Y));
        }
    }
}