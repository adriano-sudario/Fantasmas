using Microsoft.Xna.Framework;
using Phantoms.Entities.Sprites;
using Phantoms.Interfaces;
using System;

namespace Phantoms.Manipulators
{
    public class Fade
    {
        private float amount;
        private float limit;
        private IVisual fadingComponent;

        private event EventHandler onFadeEnded;

        public bool HasEnded { get; private set; }

        public Fade(IVisual fadingComponent, float amount, float from, float to, EventHandler onFadeEnded = null)
        {
            this.fadingComponent = fadingComponent;
            this.amount = amount;
            fadingComponent.Opacity = from;
            limit = to;
            this.onFadeEnded = onFadeEnded;
        }

        public void Update(GameTime gameTime)
        {
            fadingComponent.Opacity += amount;

            if ((Math.Sign(amount) < 0 && fadingComponent.Opacity <= limit) || (Math.Sign(amount) > 0 && fadingComponent.Opacity >= limit))
            {
                fadingComponent.Opacity = limit;
                HasEnded = true;
                onFadeEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
