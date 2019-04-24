using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Entities.Sprites;
using Phantoms.Helpers;
using Phantoms.Inputs;
using Phantoms.Interfaces;
using Phantoms.Scenes;
using System.Collections.Generic;

namespace Phantoms.Entities.Ghostly
{
    public class Phantom : Body
    {
        private readonly IInput input = new KeyboardInput();

        protected PhantomExpression expression;
        protected AnimatedSprite Animation { get { return (Sprite as AnimatedSprite); } }

        public float Speed { get { return .6f; } }
        public bool HasDisappeared { get { return !isActive; } private set { isActive = !value; } }
        public bool IsDisappearing { get; private set; }
        public bool IsTeleporting { get; private set; }
        public bool IsBot { get; protected set; }
        public string CurrentPlace { get; set; }

        public Phantom(AnimatedSprite animation, Vector2 position) : base(position, sprite: animation, scale: 5f)
        {
            expression = new PhantomExpression(this);
        }

        public Phantom(Texture2D spriteSheet, Vector2 position) : base(position, sprite: GetAnimationDefault(spriteSheet), scale: 5f)
        {
            expression = new PhantomExpression(this);
        }

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

            if (expression.IsExpressing && !IsDisappearing)
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

            if (input.InteractionJustPressed() && !IsTeleporting)
            {
                if (CollidesWith(SceneManager.GetCurrentScene<World>().Vortex))
                    BeginTeleport();
                else
                    expression.ExpressPhantom();
            }
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
            if (expression.IsExpressing)
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
                World world = SceneManager.GetCurrentScene<World>();
                world.TeleportPlayerTo(world.Vortex.Destiny);
                CurrentPlace = world.PlaceName;
            }
            IsTeleporting = false;
            Scale = ScaleDefault;
            StopFade();
            StopSpin();
            Sprite.Opacity = 1f;
            Sprite.Rotation = 0;
            SetOrigin(0);
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
}
