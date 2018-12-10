using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Abstracts;
using Phantoms.Data;
using Phantoms.Entities;
using Phantoms.Entities.Sprites;
using Phantoms.Helpers;
using Phantoms.Manipulators;
using Phantoms.Sounds;
using System;
using System.Collections.Generic;

namespace Phantoms.Scenes
{
    public class World : Cyclic
    {
        public enum Local { Paradise, GasStation }

        public string PlaceName { get; private set; }
        public Body Place { get; private set; }
        public Vortex Vortex { get; private set; }
        public Phantom Player { get; private set; }
        public PhantomBotLog PlayerLog { get; private set; }
        public List<PhantomBot> PhantomBots { get; private set; }
        public float ElapsedTime { get; set; }

        public World(Phantom player, List<PhantomBot> phantomBots)
        {
            PhantomBots = new List<PhantomBot>();
            Player = player;
            PhantomBots = phantomBots;
            Vortex = new Vortex("vortex", Vector2.Zero);
            SetPlace(Local.Paradise);
            Player.CurrentPlace = PlaceName;
            Random random = new Random();
            player.MoveTo(new Vector2(random.Next(Camera.AreaWidth - player.Width + 1), random.Next(Camera.AreaHeight - player.Height + 1)));
            PlayerLog = new PhantomBotLog()
            {
                Type = "pixelzinho",
                Traces = new List<PhantomTraceLog>() { new PhantomTraceLog() { ElapsedTime = 0, Position = player.Position } }
            };

            SoundEffect soundTrack = Loader.LoadSound("dominos_revisitado");
            SoundTrack.Load(soundTrack, play: true, playOnLoop: false);
        }

        public void TeleportPlayerTo(Local local)
        {
            SetPlace(local);
            Player.MoveTo(Vortex.Position, false);
        }

        public Local GetCurrentLocal()
        {
            switch (PlaceName)
            {
                case "Paradise":
                    return Local.Paradise;

                case "Gas Station":
                    return Local.GasStation;

                default:
                    throw new Exception("kd esse luga meu xapa");
            }
        }

        private void SetPlace(Local local)
        {
            switch (local)
            {
                case Local.Paradise:
                    PlaceName = "Paradise";
                    Place = new Body(Vector2.Zero, sprite: new Sprite(Loader.LoadTexture("paisagi")));
                    break;

                case Local.GasStation:
                    PlaceName = "Gas Station";
                    Place = new Body(Vector2.Zero, sprite: new Sprite(Loader.LoadTexture("posto")));
                    break;
            }

            Camera.AreaWidth = Place.Width;
            Camera.AreaHeight = Place.Height;
            Vortex.StopSpin();
            Vortex.SetOrigin(0);
            Vortex.MoveTo(GetVortexPlacePosition(local) * Global.ScreenScale, false);
            Vortex.SetOrigin(.5f);
            Vortex.Spin(Global.HorizontalDirection.Left, 2f);
        }

        private Vector2 GetVortexPlacePosition(Local local)
        {
            switch (local)
            {
                case Local.Paradise:
                    return new Vector2(1115, 200);

                case Local.GasStation:
                    return new Vector2(48, 845);

                default:
                    throw new Exception("kd esse luga meu xapa");
            }
        }

        public override void Update(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            Player.Update(gameTime);
            Camera.Update(Player);
            Vortex.Update(gameTime);

            if (!Player.IsDisappearing)
                PlayerLog.Traces.Add(
                    new PhantomTraceLog()
                    {
                        ElapsedTime = ElapsedTime,
                        Place = PlaceName,
                        Position = Player.Position / Global.ScreenScale,
                        IsTeleporting = Player.IsTeleporting
                    });

            if (SoundTrack.HasEnded && !Player.IsDisappearing)
                Player.Disappear();

            foreach (PhantomBot phantomBot in PhantomBots)
                phantomBot.Update(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            Place.Draw(spriteBatch);
            Vortex.Draw(spriteBatch);

            foreach (PhantomBot phantomBot in PhantomBots)
                phantomBot.Draw(spriteBatch);

            Player.Draw(spriteBatch);
        }
    }
}
