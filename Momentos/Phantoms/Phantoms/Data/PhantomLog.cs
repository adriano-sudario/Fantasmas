using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Phantoms.Data
{
    public class PhantomBotLog
    {
        public string Id { get; set; }
        public Color Color { get; set; }
        public List<PhantomTraceLog> Traces { get; set; }
    }

    public class PhantomTraceLog
    {
        public float ElapsedTime { get; set; }
        public string Place { get; set; }
        public string Expression { get; set; }
        public float Scale { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
    }
}
