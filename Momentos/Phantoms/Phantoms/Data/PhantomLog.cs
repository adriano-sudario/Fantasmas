using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Phantoms.Data
{
    public class PhantomBotLog
    {
        public string Type { get; set; }
        public List<PhantomTraceLog> Traces { get; set; }
    }

    public class PhantomTraceLog
    {
        public float ElapsedTime { get; set; }
        public string Place { get; set; }
        public Vector2 Position { get; set; }
        public bool IsTeleporting { get; set; }
    }
}
