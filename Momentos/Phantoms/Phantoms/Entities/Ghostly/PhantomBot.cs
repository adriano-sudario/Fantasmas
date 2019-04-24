using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Data;
using Phantoms.Entities.Sprites;
using Phantoms.Helpers;
using Phantoms.Scenes;
using System.Collections.Generic;
using System.Linq;

namespace Phantoms.Entities.Ghostly
{
    public class PhantomBot : Phantom
    {
        private List<PhantomTraceLog> traces;
        private int currentTraceIndex = 0;

        public PhantomBot(AnimatedSprite sprite, PhantomBotLog log) : base(sprite, log.Traces.First().Position)
        {
            Initialize(log);
        }

        public PhantomBot(Texture2D spriteSheet, PhantomBotLog log) : base(spriteSheet, log.Traces.First().Position)
        {
            Initialize(log);
        }

        private void Initialize(PhantomBotLog log)
        {
            IsBot = true;
            traces = log.Traces;
            Sprite.Tint(log.Color);
        }

        public static PhantomBot New(Texture2D spriteSheet, PhantomBotLog log)
        {
            return new PhantomBot(GetAnimationDefault(spriteSheet), log);
        }

        public override void Update(GameTime gameTime)
        {
            if (traces.Count == 0)
            {
                if (!IsDisappearing)
                    Disappear();
                else
                    base.Update(gameTime);

                return;
            }

            List<PhantomTraceLog> tracesJumped = new List<PhantomTraceLog>();
            PhantomTraceLog nextTrace = traces[currentTraceIndex];
            tracesJumped.Add(nextTrace);
            World world = SceneManager.GetCurrentScene<World>();

            foreach (PhantomTraceLog trace in traces)
            {
                if (trace.ElapsedTime <= world.ElapsedTime)
                {
                    tracesJumped.Add(trace);
                    nextTrace = trace;
                }
                else
                {
                    Vector2 updatedPosition = trace.Position * Global.ScreenScale;
                    Animation.Change(updatedPosition == Position ? "Idle" : "Walk");
                    Move(updatedPosition);

                    if (!string.IsNullOrEmpty(trace.Expression) && !expression.IsExpressing)
                        expression.ExpressPhantom(trace.Expression);
                    
                    if (string.IsNullOrEmpty(trace.Expression) && expression.IsExpressing)
                        expression.StopExpressing();

                    Scale = trace.Scale;
                    Sprite.Opacity = trace.Opacity;
                    Sprite.Rotation = trace.Rotation;
                    Sprite.Origin = trace.Origin;
                    CurrentPlace = trace.Place;
                    IsVisible = CurrentPlace == world.PlaceName;
                    break;
                }
            }

            traces.RemoveRange(0, tracesJumped.Count);

            base.Update(gameTime);
        }
    }
}
