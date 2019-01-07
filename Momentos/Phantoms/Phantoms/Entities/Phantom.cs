using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Data;
using Phantoms.Entities.Sprites;
using Phantoms.Inputs;
using Phantoms.Interfaces;
using System.Collections.Generic;
using Phantoms.Manipulators;
using Phantoms.Scenes;
using System;
using System.Linq;

namespace Phantoms.Entities
{
    public class PhantomBot : Phantom
    {
        private List<PhantomTraceLog> traces;
        private int currentTraceIndex = 0;

        public PhantomBot(AnimatedSprite sprite, PhantomBotLog log) : base(sprite, log.Traces[0].Position)
        {
            Initialize(log);
        }

        public PhantomBot(Texture2D spriteSheet, PhantomBotLog log) : base(spriteSheet, log.Traces[0].Position)
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
            base.Update(gameTime);

            if (traces.Count == 0)
            {
                if (!IsDisappearing)
                    Disappear();

                return;
            }

            List<PhantomTraceLog> tracesJumped = new List<PhantomTraceLog>();
            PhantomTraceLog nextTrace = traces[currentTraceIndex];
            tracesJumped.Add(nextTrace);
            foreach (PhantomTraceLog trace in traces)
            {
                if (trace.ElapsedTime <= MainGame.World.ElapsedTime)
                {
                    tracesJumped.Add(trace);
                    nextTrace = trace;
                }
                else
                {
                    if (!trace.IsTeleporting)
                    {
                        Vector2 updatedPosition = trace.Position * Global.ScreenScale;
                        Animation.Change(updatedPosition == Position ? "Idle" : "Walk");
                        MoveTo(updatedPosition);
                    }

                    if (trace.IsTeleporting && !IsTeleporting)
                        Teleport();

                    IsVisible = trace.Place == MainGame.World.PlaceName;
                    break;
                }
            }
            traces.RemoveRange(0, tracesJumped.Count);
        }
    }

    public class Phantom : Body
    {
        private IInput input = new KeyboardInput();

        protected AnimatedSprite Animation { get { return (Sprite as AnimatedSprite); } }

        public float Speed { get { return .6f; } }
        public bool HasDisappeared { get { return !isActive; } private set { isActive = !value; } }
        public bool IsDisappearing { get; private set; }
        public bool IsTeleporting { get; private set; }
        public bool IsBot { get; protected set; }
        public string CurrentPlace { get; set; }

        public Phantom(AnimatedSprite animation, Vector2 position) : base(position, sprite: animation, scale: 5f) { }

        public Phantom(Texture2D spriteSheet, Vector2 position) : base(position, sprite: GetAnimationDefault(spriteSheet), scale: 5f) { }

        public static Phantom New(Texture2D spriteSheet, Vector2 position)
        {
            return new Phantom(GetAnimationDefault(spriteSheet), position);
        }

        protected static AnimatedSprite GetAnimationDefault(Texture2D spriteSheet)
        {
            Dictionary<string, Frame[]> animationFrames = new Dictionary<string, Frame[]>();
            animationFrames.Add("Idle", new Frame[] { new Frame() { Name = "idle", Source = new Rectangle(22, 0, 11, 11), Duration = 500 } });
            animationFrames.Add("Walk", new Frame[]
            {
                new Frame() { Name = "walk_1", Source = new Rectangle(0, 0, 11, 11), Duration = 100 },
                new Frame() { Name = "walk_2", Source = new Rectangle(11, 0, 11, 11), Duration = 100 }
            });

            return new AnimatedSprite(spriteSheet, animationFrames);
        }

        public override void Update(GameTime gameTime)
        {
            if (HasDisappeared)
                return;

            base.Update(gameTime);

            if (IsBot || IsDisappearing || IsTeleporting)
                return;

            input.Update();
            Vector2 direction = input.DirectionalPressing();
            Animation.Change(direction == Vector2.Zero ? "Idle" : "Walk");
            MoveAndSlide(direction * (Speed * Scale * Global.ScreenScale));

            if (input.InteractionJustPressed() && !IsTeleporting && CollidesWith(MainGame.World.Vortex))
                Teleport();
        }

        protected void Teleport()
        {
            IsTeleporting = true;
            Animation.Pause();
            SetOrigin(.5f);
            Spin(FacingDirection);
            FadeOut();
            Shrink(onResizeEnded: (sender, e) =>
            {
                if (!IsBot)
                {
                    MainGame.World.TeleportPlayerTo(MainGame.World.Vortex.Destiny);
                    CurrentPlace = MainGame.World.PlaceName;
                }
                IsTeleporting = false;
                Scale = ScaleDefault;
                StopFade();
                StopSpin();
                Sprite.Opacity = 1f;
                Sprite.Rotation = 0;
                SetOrigin(0);
                // Grow(amount: 0.02f, onResizeEnded: (s, ev) => SetOrigin(0));
                Animation.Play();
            });
        }

        public void Disappear()
        {
            Animation.Pause();
            IsDisappearing = true;
            FadeOut(onFadeEnded: (sender, e) => HasDisappeared = true);
        }
    }
}
