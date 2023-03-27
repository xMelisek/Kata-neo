using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace KataNeo
{
    public static class KDebug
    {
        public static Texture2D debugOutline;

        [Conditional("DEBUG")]
        public static void Draw(MapManager mapManager, EntityManager entityManager, SpriteBatch spriteBatch)
        {
            foreach (var tile in mapManager.tiles)
            {
                spriteBatch.Draw(debugOutline, tile.Rect, Color.Green);
            }
            foreach (var player in entityManager.players)
            {
                spriteBatch.Draw(debugOutline, player.Rect, Color.Green);
                if (player.attack != null)
                    spriteBatch.Draw(debugOutline, player.attack.Rect, Color.Red);
            }
        }
    }
}
