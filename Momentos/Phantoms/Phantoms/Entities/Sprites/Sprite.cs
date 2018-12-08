using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Interfaces;

namespace Phantoms.Entities.Sprites
{
    public class Sprite
    {
        private Texture2D spriteStrip;
        
        public virtual Rectangle Source { get; set; }
        public virtual int Width { get { return spriteStrip.Width; } }
        public virtual int Height { get { return spriteStrip.Height; } }

        public float Rotation { get; set; }
        public float Opacity { get; set; }
        public Vector2 Origin { get; set; }

        public Sprite(Texture2D spriteStrip, Rectangle source = default(Rectangle), float opacity = 1f, Vector2 origin = default(Vector2), float rotation = 0)
        {
            this.spriteStrip = spriteStrip;
            Source = source == default(Rectangle) ? new Rectangle(0, 0, Width, Height) : source;
            Opacity = opacity;
            Origin = origin;
            Rotation = rotation;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, 
            Color color = default(Color), float scale = 1, 
            SpriteEffects effect = SpriteEffects.None, float layerDepth = 0)
        {
            color = color == default(Color) ? Color.White : color;
            spriteBatch.Draw(spriteStrip, position, Source, color * Opacity, Rotation, Origin, scale * Global.ScreenScale, effect, layerDepth);
        }
    }
}
