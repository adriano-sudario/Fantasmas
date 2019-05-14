using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Helpers;
using Phantoms.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phantoms.Manipulators
{
    public class Selector : IVisual
    {
        private SpriteFont pressStart2P;
        private Fade fade = null;
        private List<Selection> options;

        public int VerticalPadding { get; private set; }
        public int Width => GetArea().Width;
        public int Height => GetArea().Height;
        public float Rotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float Opacity { get; set; } = 1;
        public float Scale { get; set; } = 1;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Vector2 Position { get; set; }

        public Selector(List<Selection> options, Vector2 position, int verticalPadding = 10)
        {
            pressStart2P = Loader.LoadFont("press_start_2p");
            this.options = options;
            Position = position;
            VerticalPadding = verticalPadding;
            SetOptionsPosition();
        }

        private void SetOptionsPosition()
        {
            Vector2 position = Position;

            foreach (Selection option in options)
            {
                position.Y += VerticalPadding;
                option.Position = position;
                position.Y += pressStart2P.MeasureString(option.Text).Y + VerticalPadding;
            }
        }

        public Rectangle GetArea()
        {
            Vector2 measure = Vector2.Zero;
            Vector2 position = Position;

            foreach (Selection option in options)
            {
                Vector2 optionMeasure = pressStart2P.MeasureString(option.Text);
                float width = optionMeasure.X > measure.X ? optionMeasure.X : measure.X;
                float height = measure.Y + optionMeasure.Y + (VerticalPadding * 2);
                measure = new Vector2(width, height);
            }

            return new Rectangle((int)position.X, (int)position.Y, (int)measure.X, (int)measure.Y);
        }

        public void FadeIn(float amount = .01f, EventHandler onFadeEnded = null) => Fade(Math.Abs(amount), 0, 1, onFadeEnded);

        public void FadeOut(float amount = .01f, EventHandler onFadeEnded = null) => Fade(-Math.Abs(amount), 1, 0, onFadeEnded);

        private void Fade(float amount, float from, float to, EventHandler onFadeEnded = null)
        {
            fade = new Fade(this, amount, from, to, (sender, e) =>
            {
                StopFade();
                onFadeEnded?.Invoke(sender, EventArgs.Empty);
            });
        }

        public void StopFade() => fade = null;
    }
}
