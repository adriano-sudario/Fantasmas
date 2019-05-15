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
        enum Location { Portuguese, English }

        abstract class Text
        {
            public abstract string Tutorial { get; }
            public abstract string Warning { get; }
            public abstract string StartButton { get; }
            public virtual string PortugueseButton => "hein?";
            public virtual string EnglishButton => "huh?";
            public virtual string FrenchButton => "quoi?";
            public virtual string GermanButton => "was?";
            public virtual string SpanishButton => "¿qué?";
            public abstract string ExitButton { get; }
        }

        class BrazilianText : Text
        {
            public override string Tutorial => "[SETINHAS]: andar [BARRA DE ESPAÇO]: entrar no vortex\n[1]: amar [2]: cantar [3]: xolar [4]: bodiar\n\n";

            public override string Warning => 
                "a aplicação vai entrar em modo de tela cheia e fechar automaticamente após o término da música.\n" +
                "você está prestes a perder cerca de 1 minuto e meio da tua vida.\n" + 
                "tem certeza disto?";

            public override string StartButton => "tá";
            public override string ExitButton => "eu msm n, vai pra lá";

            public override string PortugueseButton => "";
        }

        class EnglishText : Text
        {
            public override string Tutorial => "[ARROW KEYS]: walk [SPACE BAR]: enter the void\n[1]: love [2]: sing [3]: cry [4]: get bored\n\n";

            public override string Warning =>
                "the application will enter full-screen mode and close automatically after the song ends.\n" +
                "you're about to lose around 1 minute and a half of your life.\n" +
                "are you sure about that?";

            public override string StartButton => "yea, watevs";
            public override string ExitButton => "thanks but no thanks";

            public override string EnglishButton => "";
        }

        private Writer commandsWriter;
        private Writer warningWriter;
        private Selector selector;
        private SpriteFont pressStart2P;
        private SpriteFont pressStart2PSmall;
        private Text text;

        public Opening()
        {
            pressStart2P = Loader.LoadFont("press_start_2p");
            pressStart2PSmall = Loader.LoadFont("press_start_2p_small");
            Load(Location.Portuguese);
        }

        private void Load(Location location)
        {
            SetText(location);
            selector = null;
            commandsWriter = new Writer(pressStart2PSmall, text.Tutorial, position: new Vector2(20, 20), maxWidth: 760,
                onComplete: () =>
                {
                    Dictionary<char, int> customChars = new Dictionary<char, int>();
                    customChars.Add('.', 500);
                    customChars.Add(' ', 0);
                    List<WriterTimeInterval> customTimeIntervals = new List<WriterTimeInterval>();
                    customTimeIntervals.AddRange(WriterTimeInterval.GetSpeedPerChar(customChars, text.Warning));
                    
                    Rectangle commandsWriterArea = commandsWriter.GetArea();
                    warningWriter = new Writer(pressStart2P, text.Warning, position: new Vector2(20, commandsWriterArea.Bottom + 20), maxWidth: 760, customTimeIntervals: customTimeIntervals,
                    onComplete: () =>
                    {
                        // Change to make options appear

                        // BeginSceneTransition();

                        LoadSelector();

                        //warningWriter.Stop();
                    });
                });

            commandsWriter.Complete(true);

            if (!SoundTrack.IsPlaying)
                SoundTrack.Load(Loader.LoadSound("crujoa"), play: true);
        }

        private void SetText(Location location)
        {
            switch (location)
            {
                case Location.Portuguese:
                    text = new BrazilianText();
                    break;

                case Location.English:
                    text = new EnglishText();
                    break;
            }
        }

        private void LoadSelector()
        {
            List<Selection> options = GetOptions();
            options.First().IsHovered = true;
            selector = new Selector(options, new Vector2(20, warningWriter.GetArea().Bottom + warningWriter.LineSpacing + 20), 
                isEnabled: false);
            selector.FadeIn(onFadeEnded: (sender, e) => selector.IsEnabled = true);
        }

        private List<Selection> GetOptions()
        {
            List<Selection> options = new List<Selection>();
            AddOption(ref options, text.StartButton, BeginSceneTransition);
            AddOption(ref options, text.PortugueseButton, () => Load(Location.Portuguese));
            AddOption(ref options, text.EnglishButton, () => Load(Location.English));
            AddOption(ref options, text.ExitButton, () => MainGame.Quit());
            //AddOption(ref options, text.FrenchButton, () => Load(Location.Portuguese));
            //AddOption(ref options, text.GermanButton, () => Load(Location.Portuguese));
            //AddOption(ref options, text.SpanishButton, () => Load(Location.Portuguese));

            return options;
        }

        private void AddOption(ref List<Selection> options, string text, Action onSelected)
        {
            if (string.IsNullOrEmpty(text))
                return;

            options.Add(new Selection(text, onSelected));
        }

        private void BeginSceneTransition()
        {
            Action changeScene = () =>
            {
                Screen.Adjust(true);
                SceneManager.Wait(2500, () => SceneManager.AddScene("World", new World(), true));
            };

            selector.IsEnabled = false;
            commandsWriter.FadeOut();
            warningWriter.FadeOut();

            if (SoundTrack.IsPlaying)
            {
                selector.FadeOut();
                SoundTrack.FadeOut(fadeIncrement: .01f, onFadeEnded: changeScene);
            }
            else
                selector.FadeOut(onFadeEnded: (sender, e) => changeScene());
        }

        public override void Update(GameTime gameTime)
        {
            commandsWriter?.Update(gameTime);
            warningWriter?.Update(gameTime);
            selector?.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            commandsWriter?.Draw(spriteBatch);
            warningWriter?.Draw(spriteBatch);
            selector?.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
