using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Phantoms.Abstracts;
using Phantoms.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantoms.Manipulators.Font
{
    public class Writer : Cyclic
    {
        private string text;
        private int currentCharIndex;
        private int currentLineIndex;
        private int customTimeIntervalIndex;
        private int timeInterval;
        private int defaultTimeInterval = 50;
        private bool isWriting;
        private SpriteFont font;
        private List<LineWriter> lineWriters;
        private List<WriterTimeInterval> customTimeIntervals;
        private LineWriter CurrenLineWriter => lineWriters[currentLineIndex];

        public Action OnComplete { private get; set; }
        public bool IsComplete { get; private set; }
        public int LineSpacing { get; private set; }

        public Writer(SpriteFont font, string text, Vector2 position = default(Vector2), int maxWidth = 800, List<WriterTimeInterval> customTimeIntervals = null,
            bool autoStart = true, int lineSpacing = 5, int defaultTimeInterval = 50, Action onComplete = null)
        {
            this.font = font;
            LineSpacing = lineSpacing;
            this.defaultTimeInterval = defaultTimeInterval;
            OnComplete = onComplete;
            customTimeIntervals = new List<WriterTimeInterval>();
            this.text = text;
            //customTimeIntervals.Add(new WriterTimeInterval("direcionais", "Setinhas direcionais para andar.", 500));
            this.customTimeIntervals = customTimeIntervals?.OrderBy(cti => cti.From).ToList() ?? new List<WriterTimeInterval>();
            lineWriters = new List<LineWriter>();
            LoadLineWriters(position, maxWidth);
            UpdateTimeInterval();

            if (autoStart)
                Start();
        }

        private void LoadLineWriters(Vector2 initialPosition, int maxWidth)
        {
            StringBuilder currentLineText = new StringBuilder();

            int lastSpaceIndex = 0;
            int linesCount = 0;
            bool addNewLine = false;
            char[] textChars = text.ToCharArray();

            for (int i = 0; i < textChars.Length; i++)
            {
                if (textChars[i] == ' ')
                    lastSpaceIndex = i;

                if (textChars[i] != '\n')
                    currentLineText.Append(textChars[i]);

                Vector2 lineMeasure = font.MeasureString(currentLineText.ToString());

                if (lineMeasure.X > maxWidth)
                {
                    int removeLength = i - lastSpaceIndex + 1;
                    currentLineText.Remove(currentLineText.Length - removeLength, i - lastSpaceIndex + 1);
                    i = lastSpaceIndex;
                }

                addNewLine = textChars[i] == '\n' || i == textChars.Length - 1 || lineMeasure.X > maxWidth;

                if (addNewLine)
                {
                    addNewLine = false;
                    Vector2 position;

                    if (lineWriters.Count == 0)
                        position = initialPosition;
                    else
                    {
                        Vector2 lastPosition = lineWriters.Last().Position;
                        Vector2 lastMeasure = lineWriters.Last().GetMeasure(font);
                        position = new Vector2(lastPosition.X, lastPosition.Y + lastMeasure.Y + LineSpacing);
                    }

                    LineWriter line = new LineWriter(currentLineText.ToString(), position);
                    lineWriters.Add(line);
                    currentLineText = new StringBuilder();
                    linesCount++;
                }
            }
        }

        public void Reset()
        {
            foreach (LineWriter lineWriter in lineWriters)
                lineWriter.Reset();
            
            currentCharIndex = 0;
            currentLineIndex = 0;
            customTimeIntervalIndex = 0;
            IsComplete = false;
            UpdateTimeInterval();
        }

        public void Complete(bool executeOnComplete = false)
        {
            IsComplete = true;

            foreach (LineWriter lineWriter in lineWriters)
                lineWriter.Complete();

            if (executeOnComplete)
                OnComplete?.Invoke();
        }

        public void Start()
        {
            isWriting = true;
        }

        public void Pause()
        {
            isWriting = false;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public Rectangle GetArea()
        {
            Vector2 measure = Vector2.Zero;
            Vector2 position = new Vector2(lineWriters.First().Position.X, lineWriters.First().Position.Y);

            foreach (LineWriter lineWriter in lineWriters)
            {
                Vector2 lineMeasure = lineWriter.GetMeasure(font);
                float width = lineMeasure.X > measure.X ? lineMeasure.X : measure.X;
                float height = measure.Y + lineMeasure.Y + LineSpacing;
                measure = new Vector2(width, height);
            }

            return new Rectangle((int)position.X, (int)position.Y, (int)measure.X, (int)measure.Y);
        }

        public void SetPosition(Vector2 position)
        {
            for (int i = 0; i < lineWriters.Count; i++)
            {
                position = i == 0 ? position :
                    new Vector2(position.X, lineWriters[i - 1].Position.Y + lineWriters[i - 1].GetMeasure(font).Y + LineSpacing);
                lineWriters[i].Position = position;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (IsComplete || !isWriting)
                return;
            
            SceneManager.Wait(timeInterval, () =>
            {
                CurrenLineWriter.EnterNextCharacter();
                IncrementChar();

                if (!CurrenLineWriter.IsComplete)
                    return;
                
                IncrementChar();

                SceneManager.Wait(timeInterval, () =>
                {
                    UpdateTimeInterval();
                    currentLineIndex++;
                    IsComplete = currentLineIndex >= lineWriters.Count;

                    if (IsComplete)
                        SceneManager.Wait(timeInterval, () => Complete(true));
                });
            });
        }

        private void IncrementChar()
        {
            UpdateTimeInterval();
            currentCharIndex++;
        }

        public void UpdateTimeInterval()
        {
            timeInterval = defaultTimeInterval;

            bool isCustomTimeInterval = customTimeIntervals.Count != 0 &&
                customTimeIntervalIndex < customTimeIntervals.Count &&
                (customTimeIntervals[customTimeIntervalIndex].From <= currentCharIndex);

            if (isCustomTimeInterval)
            {
                timeInterval = customTimeIntervals[customTimeIntervalIndex].Speed;

                if (customTimeIntervals[customTimeIntervalIndex].To < currentCharIndex)
                    customTimeIntervalIndex++;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (LineWriter lineWriter in lineWriters)
                lineWriter.Display(spriteBatch, font, Color.White);
        }
    }
}
