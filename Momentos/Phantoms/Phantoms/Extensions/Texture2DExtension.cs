using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phantoms.Extensions
{
    public static class Texture2DExtension
    {
        public static Texture2D Tint(this Texture2D spriteStrip, Color color)
        {
            Color[] pixels = new Color[spriteStrip.Width * spriteStrip.Height];
            spriteStrip.GetData(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                byte r = (byte)MathHelper.Clamp(((color.R - 255) + pixels[i].R), 0, 255);
                byte g = (byte)MathHelper.Clamp(((color.G - 255) + pixels[i].G), 0, 255);
                byte b = (byte)MathHelper.Clamp(((color.B - 255) + pixels[i].B), 0, 255);
                pixels[i] = new Color(r, g, b, pixels[i].A);
            }
            Texture2D tintedTexture = new Texture2D(spriteStrip.GraphicsDevice, spriteStrip.Width, spriteStrip.Height);
            tintedTexture.SetData(pixels);
            spriteStrip = tintedTexture;
            return spriteStrip;
        }
    }
}
