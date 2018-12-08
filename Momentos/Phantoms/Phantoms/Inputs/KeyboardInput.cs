using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Phantoms.Interfaces;

namespace Phantoms.Inputs
{
    public class KeyboardInput : IInput
    {
        public KeyboardState currentKeyboardState;
        public KeyboardState previousKeyboardState;

        Keys leftKey;
        Keys rightKey;
        Keys upKey;
        Keys downKey;
        Keys interactionKey;

        public KeyboardInput()
        {
            SetDefaultKeysByIndex();
        }

        public void SetDefaultKeysByIndex()
        {
            leftKey = Keys.Left;
            rightKey = Keys.Right;
            upKey = Keys.Up;
            downKey = Keys.Down;
            interactionKey = Keys.Space;
        }

        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }

        public Vector2 DirectionalPressing()
        {
            float x = currentKeyboardState.IsKeyDown(leftKey) ? -1 : currentKeyboardState.IsKeyDown(rightKey) ? 1 : 0;
            float y = currentKeyboardState.IsKeyDown(upKey) ? -1 : currentKeyboardState.IsKeyDown(downKey) ? 1 : 0;
            return new Vector2(x, y);
        }

        public bool InteractionJustPressed()
        {
            return previousKeyboardState.IsKeyUp(interactionKey) && currentKeyboardState.IsKeyDown(interactionKey);
        }
    }
}
