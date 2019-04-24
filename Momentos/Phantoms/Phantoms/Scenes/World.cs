using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Abstracts;
using Phantoms.CosmosDb.Collections;
using Phantoms.Data;
using Phantoms.Entities;
using Phantoms.Entities.Ghostly;
using Phantoms.Entities.Sprites;
using Phantoms.Helpers;
using Phantoms.Manipulators;
using Phantoms.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phantoms.Scenes
{
    public class World : Cyclic
    {
        private bool isSaving;
        private bool hasSaved;

        public enum Local { Paradise, GasStation, Lake, LittleHouse, BrownGrass }

        public string PlaceName { get; private set; }
        public Body Place { get; private set; }
        public Vortex Vortex { get; private set; }
        public Phantom Player { get; private set; }
        public PhantomBotLog PlayerLog { get; private set; }
        public List<PhantomBot> PhantomBots { get; private set; }
        public float ElapsedTime { get; set; }

        public static Local[] ExistingPlaces { get; } = (Local[])Enum.GetValues(typeof(Local));

        public World()
        {
            Initialize();
        }

        private void Initialize()
        {
            //Screen.Adjust(true);
            LoadPhantoms();
            Vortex = new Vortex("vortex", Vector2.Zero);
            Random random = new Random();
            Color phantomColor = new Color(random.Next(256), random.Next(256), random.Next(256));
            Player.Sprite.Tint(phantomColor);
            SetPlace(ExistingPlaces[random.Next(ExistingPlaces.Length)]);
            Player.CurrentPlace = PlaceName;
            Player.MoveTo(new Vector2(random.Next(Camera.AreaWidth - Player.Width + 1), random.Next(Camera.AreaHeight - Player.Height + 1)));
            PlayerLog = new PhantomBotLog()
            {
                Color = phantomColor,
                Traces = new List<PhantomTraceLog>() { new PhantomTraceLog() { ElapsedTime = 0, Position = Player.Position, Expression = "" } }
            };
            
            SoundEffect soundTrack = Loader.LoadSound("dominos_revisitado");
            SoundTrack.Load(soundTrack, play: true, playOnLoop: false);
        }

        private void LoadPhantoms()
        {
            Texture2D phantomTexture = Loader.LoadTexture("fantasminha_white");
            Phantom player = Phantom.New(phantomTexture, Vector2.Zero);

            List<PhantomBot> phantomBots = new List<PhantomBot>();

            foreach (PhantomBotLog botLog in Global.BotLogs)
                phantomBots.Add(PhantomBot.New(phantomTexture, botLog));

            Player = player;
            PhantomBots = phantomBots;
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

            AdjustCamera();
            Vortex.StopSpin();
            Vortex.SetOrigin(0);
            Vortex.MoveTo(GetVortexPlacePosition(local) * Global.ScreenScale, false);
            Vortex.SetOrigin(.5f);
            Vortex.Spin(Global.HorizontalDirection.Left, 2f);
            Vortex.SetRandomDestiny(GetCurrentLocal());
        }

        private void AdjustCamera()
        {
            Camera.AreaWidth = Place.Width;
            Camera.AreaHeight = Place.Height;
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

        private async void SavePlayerLog()
        {
            isSaving = true;

            try
            {
                SavePlayerLogLocally();

                await BotLogCollection.CreateAsync(PlayerLog);
                Global.BotLogs = await BotLogCollection.GetAsync();

                if (Global.BotLogs.Count() > 9)
                    await BotLogCollection.DeleteAsync(Global.BotLogs.Take(Global.BotLogs.Count() - 9));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            isSaving = false;
            hasSaved = true;
        }

        private void SavePlayerLogLocally()
        {
            List<PhantomBotLog> botLogsList = Global.BotLogs.ToList();
            botLogsList.Add(PlayerLog);
            Loader.SaveJsonFile("phatom_bots", botLogsList);
        }

        public override void Update(GameTime gameTime)
        {
            if (isSaving)
                return;

            if (Player.IsDisappearing && !isSaving && !hasSaved)
                SavePlayerLog();
            else if (Player.HasDisappeared)
                MainGame.Quit();

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
                        Expression = Player.GetCurrentExpressionName(),
                        Scale = Player.Scale,
                        Opacity = Player.Sprite.Opacity,
                        Rotation = Player.Sprite.Rotation,
                        Origin = Player.Sprite.Origin
                    });

            if (SoundTrack.HasEnded && !Player.IsDisappearing)
                Player.Disappear();

            foreach (PhantomBot phantomBot in PhantomBots)
                phantomBot.Update(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.ViewMatrix);

            DrawEntities(spriteBatch);

            spriteBatch.End();
        }

        private void DrawEntities(SpriteBatch spriteBatch)
        {
            Place.Draw(spriteBatch);
            Vortex.Draw(spriteBatch);

            foreach (PhantomBot phantomBot in PhantomBots)
                phantomBot.Draw(spriteBatch);

            Player.Draw(spriteBatch);
        }
    }
}
