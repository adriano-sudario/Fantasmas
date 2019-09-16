using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phantoms.Manipulators.Font
{
    public class WriterTimeInterval
    {
        public int From { get; private set; }
        public int To { get; private set; }
        public int Speed { get; private set; }

        public WriterTimeInterval(int from, int to, int speed)
        {
            From = from;
            To = to;
            Speed = speed;
        }

        public WriterTimeInterval(string snippet, string text, int speed)
        {
            From = text.IndexOf(snippet);
            To = From + snippet.Length - 1;
            Speed = speed;
        }

        public WriterTimeInterval KeepSameIndex(bool fromBegin)
        {
            if (fromBegin)
                To = From;
            else
                From = To;

            return this;
        }

        public static List<WriterTimeInterval> GetSpeedPerChar(Dictionary<char, int> speedPerChar, string text)
        {
            List<WriterTimeInterval> timeIntervals = new List<WriterTimeInterval>();

            for (int i = 0; i < text.Length; i++)
                if (speedPerChar.ContainsKey(text[i]))
                    timeIntervals.Add(new WriterTimeInterval(i, i, speedPerChar[text[i]]));

            return timeIntervals;
        }
    }
}
