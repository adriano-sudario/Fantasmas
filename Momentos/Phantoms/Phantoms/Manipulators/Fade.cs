using Microsoft.Xna.Framework;
using Phantoms.Entities.Sprites;
using System;

namespace Phantoms.Manipulators
{
    public class Fade
    {
        private float amount;
        private float limit;
        private Sprite fadingSprite;

        private event EventHandler onFadeEnded;

        public bool HasEnded { get; private set; }

        public Fade(Sprite fadingSprite, float amount, float from, float to, EventHandler onFadeEnded = null)
        {
            this.fadingSprite = fadingSprite;
            this.amount = amount;
            fadingSprite.Opacity = from;
            limit = to;
            this.onFadeEnded = onFadeEnded;
        }

        public void Update(GameTime gameTime)
        {
            fadingSprite.Opacity += amount;

            if ((Math.Sign(amount) < 0 && fadingSprite.Opacity <= limit) || (Math.Sign(amount) > 0 && fadingSprite.Opacity >= limit))
            {
                fadingSprite.Opacity = limit;
                HasEnded = true;
                onFadeEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
