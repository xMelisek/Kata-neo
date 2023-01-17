using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KataNeo.Entitites
{
    public abstract class Entity
    {
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
