using Microsoft.Xna.Framework;

namespace Phantoms.Interfaces
{
    public interface IInput
    {
        void Update();
        Vector2 DirectionalPressing();
        bool InteractionJustPressed();
        bool ActivateExpressionOne();
        bool ActivateExpressionTwo();
        bool ActivateExpressionThree();
        bool ActivateExpressionFour();
    }
}
