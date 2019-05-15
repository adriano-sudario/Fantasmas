using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phantoms.Manipulators
{
    public class Selection
    {
        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public Action OnSelected { get; set; }
        public bool IsHovered { get; set; }

        public Selection(string text, Action onSelected)
        {
            Text = text;
            OnSelected = onSelected;
        }

        public void Display(SpriteBatch spriteBatch, SpriteFont font, Color color, float opacity = 1)
        {
            spriteBatch.DrawString(font, Text, Position, color * opacity);
        }
    }
}
