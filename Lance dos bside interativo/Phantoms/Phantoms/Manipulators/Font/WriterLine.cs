using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Phantoms.Manipulators.Font
{
    public class LineWriter
    {
        private int currentDisplayCharIndex;
        private StringBuilder textDisplayed = new StringBuilder();

        public Vector2 Position { get; set; }
        public string Text { get; private set; }
        public string TextDisplayed => textDisplayed.ToString();
        public bool IsComplete { get; private set; }

        public LineWriter(string text, Vector2 position, bool isCompleted = false)
        {
            Text = text;
            Position = position;

            if (isCompleted)
                Complete();
        }

        public void EnterNextCharacter()
        {
            if (IsComplete)
                return;

            if (Text.Length > 0)
            {
                string textToAppend = Text.Substring(currentDisplayCharIndex, 1);
                textDisplayed.Append(textToAppend);
            }
            
            currentDisplayCharIndex++;
            IsComplete = currentDisplayCharIndex >= Text.Length;
        }

        public void Reset()
        {
            textDisplayed.Clear();
            currentDisplayCharIndex = 0;
            IsComplete = false;
        }

        public void Complete()
        {
            textDisplayed.Clear();
            textDisplayed.Append(Text);
            IsComplete = true;
        }

        public Vector2 GetMeasure(SpriteFont font)
        {
            return font.MeasureString(Text);
        }

        public void Display(SpriteBatch spriteBatch, SpriteFont font, Color color, float opacity = 1)
        {
            spriteBatch.DrawString(font, textDisplayed, Position, color * opacity);
        }
    }
}
