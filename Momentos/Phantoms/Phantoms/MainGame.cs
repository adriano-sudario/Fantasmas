using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Phantoms.Data;
using Phantoms.Entities;
using Phantoms.Entities.Sprites;
using Phantoms.Helpers;
using Phantoms.Manipulators;
using Phantoms.Scenes;
using System;
using System.Collections.Generic;

namespace Phantoms
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static World World { get; set; }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Loader.Initialize(Content);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AdjustScreen(true);

            Texture2D phantomTexture = Loader.LoadTexture("fantasminha");
            Phantom player = Phantom.New(phantomTexture, Vector2.Zero);
            List<PhantomBotLog> botLogs = Loader.LoadDeserializedJsonFile<List<PhantomBotLog>>("phatom_bots");
            List<PhantomBot> phantomBots = new List<PhantomBot>();

            foreach (PhantomBotLog botLog in botLogs)
                phantomBots.Add(PhantomBot.New(phantomTexture, botLog.Traces));

            World = new World(player, phantomBots);
        }

        public void ToggleFullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            AdjustScreen();
        }

        public void AdjustScreen(bool isFullScreen)
        {
            graphics.IsFullScreen = isFullScreen;
            AdjustScreen();
        }

        private void AdjustScreen()
        {
            if (graphics.IsFullScreen)
            {
                Global.ScreenWidth = GraphicsDevice.DisplayMode.Width;
                Global.ScreenHeight = GraphicsDevice.DisplayMode.Height;

                graphics.PreferredBackBufferWidth = Global.ScreenWidth;
                graphics.PreferredBackBufferHeight = Global.ScreenHeight;
                graphics.ApplyChanges();

                decimal scaleX = (decimal)Global.ScreenWidth / GraphicsDeviceManager.DefaultBackBufferWidth;
                decimal scaleY = (decimal)Global.ScreenHeight / GraphicsDeviceManager.DefaultBackBufferHeight;
                Global.ScreenScale = (int)Math.Ceiling(scaleX > scaleY ? scaleX : scaleY);
            }
            else
            {
                Global.ScreenWidth = GraphicsDeviceManager.DefaultBackBufferWidth;
                Global.ScreenHeight = GraphicsDeviceManager.DefaultBackBufferHeight;
                Global.ScreenScale = 1f;
            }

            Camera.AreaWidth = World?.Place.Width ?? 0;
            Camera.AreaHeight = World?.Place.Height ?? 0;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || World.Player.HasDisappeared)
            {
                if (World.Player.HasDisappeared)
                {
                    List<PhantomBotLog> botLogs = Loader.LoadDeserializedJsonFile<List<PhantomBotLog>>("phatom_bots");
                    botLogs.Add(World.PlayerLog);
                    if (botLogs.Count > 9)
                        botLogs.RemoveRange(0, botLogs.Count - 9);
                    Loader.SaveJsonFile("phatom_bots", botLogs);
                }
                Exit();
            }

            World.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.ViewMatrix);

            World.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
