using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Phantoms.Interfaces
{
    public interface IVisual
    {
        int Width { get; }
        int Height { get; }
        float Rotation { get; set; }
        float Opacity { get; set; }
        float Scale { get; set; }
        Vector2 Origin { get; set; }
        Vector2 Position { get; set; }
    }
}
