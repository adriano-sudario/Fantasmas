using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Interfaces;
using System;

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

        public void Tint(Color tint)
        {
            Color[] pixels = new Color[spriteStrip.Width * spriteStrip.Height];
            spriteStrip.GetData(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                byte r = (byte)MathHelper.Clamp(((tint.R - 255) + pixels[i].R), 0, 255);
                byte g = (byte)MathHelper.Clamp(((tint.G - 255) + pixels[i].G), 0, 255);
                byte b = (byte)MathHelper.Clamp(((tint.B - 255) + pixels[i].B), 0, 255);
                pixels[i] = new Color(r, g, b, pixels[i].A);
            }
            Texture2D tintedTexture = new Texture2D(spriteStrip.GraphicsDevice, spriteStrip.Width, spriteStrip.Height);
            tintedTexture.SetData(pixels);
            spriteStrip = tintedTexture;
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
