using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phantoms.Helpers
{
    public static class Screen
    {
        private static GraphicsDeviceManager graphics;
        private static GraphicsDevice graphicsDevice;

        public static void Initialize(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
        {
            Screen.graphics = graphics;
            Screen.graphicsDevice = graphicsDevice;
        }

        public static void ToggleFullScreen(Action afterToggle = null)
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            AdjustScreen();
            afterToggle?.Invoke();
        }

        public static void Adjust(bool isFullScreen, Action afterAdjustment = null)
        {
            graphics.IsFullScreen = isFullScreen;
            AdjustScreen();
            afterAdjustment?.Invoke();
        }

        private static void AdjustScreen()
        {
            if (graphics.IsFullScreen)
            {
                Global.ScreenWidth = graphicsDevice.DisplayMode.Width;
                Global.ScreenHeight = graphicsDevice.DisplayMode.Height;

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
        }
    }
}
