using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Abstracts;
using Phantoms.Helpers;
using Phantoms.Manipulators.Font;
using Phantoms.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phantoms.Scenes
{
    public class Opening : Cyclic
    {
        private Writer writer;

        public Opening(SpriteFont pressStart2P) =>
            writer = new Writer(pressStart2P, 
                onComplete: () => 
                {
                    // Change to make options appear

                    writer.Stop();
                    Action changeScene = () =>
                    {
                        Screen.Adjust(true);
                        SceneManager.Wait(2500, () => SceneManager.AddScene("World", new World(), true));
                    };

                    SceneManager.Wait(1000, changeScene);

                    // SoundTrack.FadeOut(fadeIncrement: .01f, onFadeEnded: changeScene);
                });

        public override void Update(GameTime gameTime) => writer.Update(gameTime);

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            writer.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
