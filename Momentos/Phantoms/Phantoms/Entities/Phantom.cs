using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Data;
using Phantoms.Entities.Sprites;
using Phantoms.Helpers;
using Phantoms.Inputs;
using Phantoms.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phantoms.Entities
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
                    Vector2 updatedPosition = trace.Position * Global.ScreenScale;
                    Animation.Change(updatedPosition == Position ? "Idle" : "Walk");
                    Move(updatedPosition);

                    if (!string.IsNullOrEmpty(trace.Expression) && !expression.IsExpressing)
                        expression.ExpressPhantom(trace.Expression);

                    Scale = trace.Scale;
                    Sprite.Opacity = trace.Opacity;
                    Sprite.Rotation = trace.Rotation;
                    Sprite.Origin = trace.Origin;
                    CurrentPlace = trace.Place;
                    IsVisible = CurrentPlace == MainGame.World.PlaceName;
                    break;
                }
            }
            traces.RemoveRange(0, tracesJumped.Count);

            base.Update(gameTime);
        }
    }

    public class Phantom : Body
    {
        private IInput input = new KeyboardInput();

        protected PhantomExpression expression;
        protected AnimatedSprite Animation { get { return (Sprite as AnimatedSprite); } }

        public float Speed { get { return .6f; } }
        public bool HasDisappeared { get { return !isActive; } private set { isActive = !value; } }
        public bool IsDisappearing { get; private set; }
        public bool IsTeleporting { get; private set; }
        public bool IsBot { get; protected set; }
        public string CurrentPlace { get; set; }

        public Phantom(AnimatedSprite animation, Vector2 position) : base(position, sprite: animation, scale: 5f) 
            => expression = new PhantomExpression(this);

        public Phantom(Texture2D spriteSheet, Vector2 position) : base(position, sprite: GetAnimationDefault(spriteSheet), scale: 5f) 
            => expression = new PhantomExpression(this);

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

            if (expression.IsExpressing)
                expression.Update(gameTime);

            if (IsBot || IsDisappearing || IsTeleporting)
                return;

            input.Update();
            Vector2 direction = input.DirectionalPressing();
            Animation.Change(direction == Vector2.Zero ? "Idle" : "Walk");
            Move(direction * (Speed * Scale * Global.ScreenScale));

            if (input.ActivateExpressionOne())
                expression.ExpressPhantom(PhantomExpression.Expression.Bored);
            else if (input.ActivateExpressionTwo())
                expression.ExpressPhantom(PhantomExpression.Expression.Cry);
            else if (input.ActivateExpressionThree())
                expression.ExpressPhantom(PhantomExpression.Expression.Love);
            else if (input.ActivateExpressionFour())
                expression.ExpressPhantom(PhantomExpression.Expression.Sing);

            if (input.InteractionJustPressed() && !IsTeleporting && CollidesWith(MainGame.World.Vortex))
                BeginTeleport();
        }

        public void Move(Vector2 movement)
        {
            if (IsBot)
                MoveTo(movement);
            else
                MoveAndSlide(movement);

            if (expression.IsExpressing)
                expression.UpdatePosition();
        }

        protected void BeginTeleport()
        {
            IsTeleporting = true;
            expression.StopExpressing();
            Animation.Pause();
            SetOrigin(.5f);
            Spin(FacingDirection);
            FadeOut();
            Shrink(onResizeEnded: (sender, e) => Teleport());
        }

        protected void Teleport()
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
        }

        public string GetCurrentExpressionName()
        {
            return expression.GetCurrentExpressionName();
        }

        public void Disappear()
        {
            Animation.Pause();
            IsDisappearing = true;
            expression.StopExpressing();
            FadeOut(onFadeEnded: (sender, e) => HasDisappeared = true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            if (expression.IsExpressing && !IsTeleporting)
                expression.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }
    }

    public class PhantomExpression
    {
        public enum Expression { Cry, Love, Bored, Sing }

        private static Texture2D _expressionSheet;
        private static Texture2D ExpressionSheet { get => GetTexture(); }

        private Body expressionsBody;
        private Phantom phantom;
        private AnimatedSprite Animation { get => ((AnimatedSprite)expressionsBody.Sprite); }

        public bool IsExpressing { get => Animation.IsPlaying; }

        public PhantomExpression(Phantom phantom)
        {
            Initialize(phantom);
        }

        public void ExpressPhantom(Expression expression)
        {
            ExpressPhantom(GetExpressionName(expression));
        }

        public void ExpressPhantom(string expression)
        {
            if (IsExpressing)
                return;

            UpdatePosition();
            Animation.Stop();
            Animation.Change(expression);
            Animation.Play();
        }

        public string GetCurrentExpressionName()
        {
            if (!IsExpressing)
                return "";

            return Animation.CurrentName;
        }

        public void UpdatePosition()
        {
            Vector2 position = new Vector2(phantom.Position.X + (phantom.Width * .5f), phantom.Position.Y - (Animation.Height * .5f) - 30);
            expressionsBody.MoveTo(position, setFacingDirection: false);
        }

        public void Update(GameTime gameTime)
        {
            Animation.Update(gameTime);
        }

        public void StopExpressing()
        {
            if (IsExpressing)
                Animation.Stop();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            expressionsBody.Draw(spriteBatch);
        }

        private string GetExpressionName(Expression expression)
        {
            switch (expression)
            {
                case Expression.Cry:
                    return "Cry";

                case Expression.Love:
                    return "Love";

                case Expression.Bored:
                    return "Bored";

                case Expression.Sing:
                    return "Sing";

                default:
                    throw new Exception("ops");
            }
        }

        private void Initialize(Phantom phantom)
        {
            Dictionary<string, Frame[]> expressionsFrames = new Dictionary<string, Frame[]>();
            expressionsFrames.Add("Cry", GetCryFrames());
            expressionsFrames.Add("Love", GetLoveFrames());
            expressionsFrames.Add("Bored", GetBoredFrames());
            expressionsFrames.Add("Sing", GetSingFrames());

            AnimatedSprite animation = null;
            int ciclesCount = 0;
            animation = new AnimatedSprite(ExpressionSheet, expressionsFrames, onFrameChange: (sender, e) =>
            {
                int totalCicles = animation.CurrentName == "Love" || animation.CurrentName == "Sing" ? 3 : 1;

                if (e.HasCompletedCicle)
                    ciclesCount++;

                if (ciclesCount >= totalCicles)
                {
                    ciclesCount = 0;
                    animation.Stop();
                }
            }, autoPlay: false);

            expressionsBody = new Body(Vector2.Zero, animation, scale: 3f);
            expressionsBody.SetOrigin(.5f);
            this.phantom = phantom;
        }

        private Frame[] GetCryFrames()
        {
            return new Frame[]
            {
                new Frame() { Name = "cry_1", Source = new Rectangle(0, 12, 7, 7), Duration = 100 },
                new Frame() { Name = "cry_2", Source = new Rectangle(7, 12, 7, 7), Duration = 100 },
                new Frame() { Name = "cry_3", Source = new Rectangle(14, 12, 7, 7), Duration = 100 },
                new Frame() { Name = "cry_3", Source = new Rectangle(0, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_4", Source = new Rectangle(21, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_5", Source = new Rectangle(0, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_6", Source = new Rectangle(21, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_7", Source = new Rectangle(0, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_8", Source = new Rectangle(28, 12, 7, 7), Duration = 100 },
                new Frame() { Name = "cry_9", Source = new Rectangle(35, 12, 7, 7), Duration = 100 },
                new Frame() { Name = "cry_10", Source = new Rectangle(0, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_11", Source = new Rectangle(21, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_12", Source = new Rectangle(0, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_13", Source = new Rectangle(21, 12, 7, 7), Duration = 200 },
                new Frame() { Name = "cry_14", Source = new Rectangle(0, 12, 7, 7), Duration = 200 },
            };
        }

        private Frame[] GetLoveFrames()
        {
            return new Frame[]
            {
                new Frame() { Name = "love_1", Source = new Rectangle(0, 0, 14, 12), Duration = 250 },
                new Frame() { Name = "love_2", Source = new Rectangle(14, 0, 14, 12), Duration = 250 }
            };
        }

        private Frame[] GetBoredFrames()
        {
            return new Frame[]
            {
                new Frame() { Name = "bored_1", Source = new Rectangle(42, 11, 8, 8), Duration = 500 },
                new Frame() { Name = "bored_2", Source = new Rectangle(50, 11, 8, 8), Duration = 250 },
                new Frame() { Name = "bored_3", Source = new Rectangle(42, 11, 8, 8), Duration = 500 },
                new Frame() { Name = "bored_4", Source = new Rectangle(58, 11, 8, 8), Duration = 1000 }
            };
        }

        private Frame[] GetSingFrames()
        {
            return new Frame[]
            {
                new Frame() { Name = "sing_1", Source = new Rectangle(28, 0, 5, 9), Duration = 250 },
                new Frame() { Name = "sing_2", Source = new Rectangle(33, 0, 5, 9), Duration = 250 },
                new Frame() { Name = "sing_3", Source = new Rectangle(38, 0, 5, 9), Duration = 125 },
                new Frame() { Name = "sing_4", Source = new Rectangle(43, 0, 5, 9), Duration = 125 },
                new Frame() { Name = "sing_5", Source = new Rectangle(48, 0, 5, 9), Duration = 125 },
                new Frame() { Name = "sing_6", Source = new Rectangle(53, 0, 5, 9), Duration = 125 }
            };
        }

        private static Texture2D GetTexture()
        {
            if (_expressionSheet == null)
                _expressionSheet = Loader.LoadTexture("phantom_expressions_sheet");

            return _expressionSheet;
        }
    }
}
