using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Phantoms.Abstracts;
using Phantoms.CosmosDb.Collections;
using Phantoms.Data;
using Phantoms.Entities.Ghostly;
using Phantoms.Helpers;
using Phantoms.Manipulators;
using Phantoms.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phantoms
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Action Quit { get; private set; }
        //public static Action Quit { get; private set; }

        //private static bool applyFullScreenAdjustment;
        //private static bool isFullScreen;
        //private static Action onFullScreenAdjust;

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
            base.Initialize();
            Quit = Exit;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Loader.Initialize(Content);
            Screen.Initialize(graphics, GraphicsDevice);
            Screen.Adjust(false);
            SceneManager.AddScene("Opening", new Opening());
            SetBotLogsAsync();
        }

        public async void SetBotLogsAsync()
        {
            try
            {
                Global.BotLogs = await BotLogCollection.GetAsync();
                Global.BotLogs = Global.BotLogs.Reverse().Take(9);
            }
            catch
            {
                Global.BotLogs = Loader.LoadDeserializedJsonFile<List<PhantomBotLog>>("phantom_bots");
            }
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
            //if (botLogs != null && World == null)
            //    LoadWorld();

            //if (World == null || isSaving)
            //    return;

            //if (World.Player.IsDisappearing && !isSaving && !hasSaved)
            //    SavePlayerLog();
            //else if (World.Player.HasDisappeared)
            //    Exit();
            
            SceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        //private async void SavePlayerLog()
        //{
        //    isSaving = true;

        //    try
        //    {
        //        SavePlayerLogLocally();

        //        await BotLogCollection.CreateAsync(World.PlayerLog);
        //        botLogs = await BotLogCollection.GetAsync();

        //        if (botLogs.Count() > 9)
        //            await BotLogCollection.DeleteAsync(botLogs.Take(botLogs.Count() - 9));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }

        //    isSaving = false;
        //    hasSaved = true;
        //}

        //private void SavePlayerLogLocally()
        //{
        //    List<PhantomBotLog> botLogsList = botLogs.ToList();
        //    botLogsList.Add(World.PlayerLog);
        //    Loader.SaveJsonFile("phatom_bots", botLogsList);
        //}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SceneManager.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
