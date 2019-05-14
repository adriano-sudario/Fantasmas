using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Abstracts;
using Phantoms.Helpers;
using Phantoms.Interfaces;
using Phantoms.Manipulators;
using Phantoms.Manipulators.Font;
using Phantoms.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phantoms.Scenes
{
    public class Opening : Cyclic
    {
        private List<Writer> writers = new List<Writer>();

        public Opening()
        {
            SpriteFont pressStart2P = Loader.LoadFont("press_start_2p");
            SpriteFont pressStart2PSmall = Loader.LoadFont("press_start_2p_small");

            SoundTrack.Load(Loader.LoadSound("crujoa"), play: true);

            string text = "[SETINHAS]: andar [BARRA DE ESPAÇO]: entrar no vortex\n[1]: amar [2]: cantar [3]: xolar [4]: bodiar\n\n";
            Writer commandsWriter = null;

            commandsWriter = new Writer(pressStart2PSmall, text, position: new Vector2(20, 20), maxWidth: 760,
                onComplete: () =>
                {
                    text = "a aplicação vai entrar em modo de tela cheia e fechar automaticamente após o término da música.\n";
                    //text += "recomenda-se não fechar manualmente.\n";
                    text += "você está prestes a perder cerca de 1 minuto e meio da tua vida.\n";
                    text += "tem certeza disto?";

                    Dictionary<char, int> customChars = new Dictionary<char, int>();
                    customChars.Add('.', 500);
                    customChars.Add(' ', 0);
                    List<WriterTimeInterval> customTimeIntervals = new List<WriterTimeInterval>();
                    customTimeIntervals.AddRange(WriterTimeInterval.GetSpeedPerChar(customChars, text));

                    Writer warningWriter = null;
                    Rectangle commandsWriterArea = commandsWriter.GetArea();
                    warningWriter = new Writer(pressStart2P, text, position: new Vector2(20, commandsWriterArea.Bottom + 20), maxWidth: 760, customTimeIntervals: customTimeIntervals,
                    onComplete: () =>
                    {
                        // Change to make options appear

                        BeginSceneTransition();

                        //warningWriter.Stop();
                    });
                    writers.Add(warningWriter);
                });

            commandsWriter.Complete(true);
            writers.Add(commandsWriter);
        }

        private void BeginSceneTransition()
        {
            // todo

            //Action changeScene = () =>
            //{
            //    Screen.Adjust(true);
            //    SceneManager.Wait(2500, () => SceneManager.AddScene("World", new World(), true));
            //};

            foreach (Writer writer in writers)
                writer.FadeOut();

            SoundTrack.FadeOut(fadeIncrement: .01f, onFadeEnded: null);
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
