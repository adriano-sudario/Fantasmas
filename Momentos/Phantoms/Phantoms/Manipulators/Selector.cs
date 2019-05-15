using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Abstracts;
using Phantoms.Helpers;
using Phantoms.Inputs;
using Phantoms.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phantoms.Manipulators
{
    public class Selector : Cyclic, IVisual
    {
        private readonly IInput input = new KeyboardInput();

        private SpriteFont pressStart2P;
        private Fade fade = null;
        private List<Selection> options;
        private float selectionChangeSecondsDelay = .5f;
        private float elapsedTime;
        private bool canChange = true;

        public bool IsEnabled { get; set; } = false;
        public int VerticalPadding { get; private set; }
        public int Width => GetArea().Width;
        public int Height => GetArea().Height;
        public float Rotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float Opacity { get; set; } = 1;
        public float Scale { get; set; } = 1;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Vector2 Position { get; set; }

        public Selector(List<Selection> options, Vector2 position, int verticalPadding = 10, bool isEnabled = true)
        {
            pressStart2P = Loader.LoadFont("press_start_2p");
            this.options = options;
            IsEnabled = isEnabled;
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

        public override void Update(GameTime gameTime)
        {
            fade?.Update(gameTime);

            if (!canChange)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                if (elapsedTime >= selectionChangeSecondsDelay * 1000)
                {
                    canChange = true;
                    elapsedTime = 0;
                }
            }

            if (!IsEnabled)
                return;

            input.Update();

            Vector2 directionalPressing = input.DirectionalPressing();

            if (directionalPressing.Y != 0 && canChange)
            {
                canChange = false;

                for (int i = 0; i < options.Count; i++)
                {
                    if (options[i].IsHovered)
                    {
                        options[i].IsHovered = false;
                        int selectedIndex = directionalPressing.Y > 0 ? i + 1 : i - 1;
                        if (selectedIndex < 0)
                            selectedIndex = options.Count - 1;
                        else if (selectedIndex >= options.Count)
                            selectedIndex = 0;
                        options[selectedIndex].IsHovered = true;
                        break;
                    }
                }
            }
            else if (directionalPressing.Y == 0 && !canChange)
            {
                canChange = true;
                elapsedTime = 0;
            }

            if (input.InteractionJustPressed() || input.EnterJustPressed())
                options.Find(o => o.IsHovered).OnSelected?.Invoke();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Selection option in options)
                option.Display(spriteBatch, pressStart2P, option.IsHovered ? Color.Yellow : Color.White, Opacity);
        }
    }
}
