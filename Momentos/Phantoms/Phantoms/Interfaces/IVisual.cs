using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Phantoms.Interfaces
{
    public interface IVisual
    {
        int Width { get; }
        int Height { get; }

        void Draw(SpriteBatch spriteBatch, Vector2 position,
            Color color = default(Color), float rotation = 0, Vector2 origin = default(Vector2),
            float scale = 1, SpriteEffects effect = SpriteEffects.None, float layerDepth = 0, 
            float opacity = 1f);
    }
}
