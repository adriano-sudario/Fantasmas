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
        public enum Local { Paradise, GasStation, Lake, LittleHouse, BrownGrass }

        public string PlaceName { get; private set; }
        public Body Place { get; private set; }
        public Vortex Vortex { get; private set; }
        public Phantom Player { get; private set; }
        public PhantomBotLog PlayerLog { get; private set; }
        public List<PhantomBot> PhantomBots { get; private set; }
        public float ElapsedTime { get; set; }

        public static Local[] ExistingPlaces { get; } = (Local[])Enum.GetValues(typeof(Local));

        public World(Phantom player, List<PhantomBot> phantomBots)
        {
            PhantomBots = new List<PhantomBot>();
            Player = player;
            PhantomBots = phantomBots;
            Vortex = new Vortex("vortex", Vector2.Zero);
            Random random = new Random();
            Color phantomColor = new Color(random.Next(256), random.Next(256), random.Next(256));
            Player.Sprite.Tint(phantomColor);
            SetPlace(ExistingPlaces[random.Next(ExistingPlaces.Length)]);
            Player.CurrentPlace = PlaceName;
            player.MoveTo(new Vector2(random.Next(Camera.AreaWidth - player.Width + 1), random.Next(Camera.AreaHeight - player.Height + 1)));
            PlayerLog = new PhantomBotLog()
            {
                Color = phantomColor,
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

                case "Lake":
                    return Local.Lake;

                case "Little House":
                    return Local.LittleHouse;

                case "Brown Grass":
                    return Local.BrownGrass;

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

                case Local.Lake:
                    PlaceName = "Lake";
                    Place = new Body(Vector2.Zero, sprite: new Sprite(Loader.LoadTexture("lago")));
                    break;

                case Local.LittleHouse:
                    PlaceName = "Little House";
                    Place = new Body(Vector2.Zero, sprite: new Sprite(Loader.LoadTexture("casinha")));
                    break;

                case Local.BrownGrass:
                    PlaceName = "Brown Grass";
                    Place = new Body(Vector2.Zero, sprite: new Sprite(Loader.LoadTexture("matagal")));
                    break;
            }

            Camera.AreaWidth = Place.Width;
            Camera.AreaHeight = Place.Height;
            Vortex.StopSpin();
            Vortex.SetOrigin(0);
            Vortex.MoveTo(GetVortexPlacePosition(local) * Global.ScreenScale, false);
            Vortex.SetOrigin(.5f);
            Vortex.Spin(Global.HorizontalDirection.Left, 2f);
            Vortex.SetRandomDestiny(GetCurrentLocal());
        }

        private Vector2 GetVortexPlacePosition(Local local)
        {
            switch (local)
            {
                case Local.Paradise:
                    return new Vector2(1115, 200);

                case Local.GasStation:
                    return new Vector2(48, 845);

                case Local.Lake:
                    return new Vector2(606, 169);

                case Local.LittleHouse:
                    return new Vector2(840, 192);

                case Local.BrownGrass:
                    return new Vector2(389, 339);

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
