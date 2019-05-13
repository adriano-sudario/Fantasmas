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
        private List<Writer> writers = new List<Writer>();

        public Opening()
        {
            SpriteFont pressStart2P = Loader.LoadFont("press_start_2p");
            SpriteFont pressStart2PSmall = Loader.LoadFont("press_start_2p_small");

            string text = "[setinhas]: andar\n[barra de espaço]: entrar no vortex\n[1]: amar\n[2]: cantar\n[3]: xolar\n[4]: bodiar\n\n";

            Writer commandsWriter = new Writer(pressStart2PSmall, text,
                onComplete: () =>
                {
                    Dictionary<char, int> customChars = new Dictionary<char, int>();
                    customChars.Add('.', 500);
                    customChars.Add(' ', 0);
                    List<WriterTimeInterval> customTimeIntervals = new List<WriterTimeInterval>();
                    customTimeIntervals.AddRange(WriterTimeInterval.GetSpeedPerChar(customChars, text));

                    text = "a aplicação vai entrar em modo de tela cheia e encerrar automaticamente após o término da música.\n";
                    text += "recomenda-se não fechar manualmente.\n";
                    text += "você está prestes a perder 1 minuto e meio da tua vida.\n";
                    text += "tem certeza disto?";

                    Writer warningWriter = null;
                    warningWriter = new Writer(pressStart2P, text, customTimeIntervals: customTimeIntervals,
                    onComplete: () =>
                    {
                        // Change to make options appear

                        warningWriter.Stop();
                        Action changeScene = () =>
                        {
                            Screen.Adjust(true);
                            SceneManager.Wait(2500, () => SceneManager.AddScene("World", new World(), true));
                        };

                        SceneManager.Wait(1000, changeScene);

                        // SoundTrack.FadeOut(fadeIncrement: .01f, onFadeEnded: changeScene);
                    });
                    writers.Add(warningWriter);
                });

            writers.Add(commandsWriter);

            commandsWriter.Complete(true);

            //writer = new Writer(pressStart2P,
            //    onComplete: () =>
            //    {
            //        // Change to make options appear

            //        writer.Stop();
            //        Action changeScene = () =>
            //        {
            //            Screen.Adjust(true);
            //            SceneManager.Wait(2500, () => SceneManager.AddScene("World", new World(), true));
            //        };

            //        SceneManager.Wait(1000, changeScene);

            //        // SoundTrack.FadeOut(fadeIncrement: .01f, onFadeEnded: changeScene);
            //    });
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Writer writer in writers)
                writer.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (Writer writer in writers)
                writer.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
