using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Phantoms.Interfaces;
using System;
using System.Collections.Generic;

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
        Keys expressionOneKey;
        Keys expressionTwoKey;
        Keys expressionThreeKey;
        Keys expressionFourKey;

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
            Dictionary<int, Keys> expressions = new Dictionary<int, Keys>();
            List<Keys> expressionsOptions = new List<Keys>() { Keys.D1, Keys.D2, Keys.D3, Keys.D4 };
            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                Keys expression = expressionsOptions[random.Next(expressionsOptions.Count)];
                expressions.Add(i, expression);
                expressionsOptions.Remove(expression);
            }

            expressionOneKey = expressions[0];
            expressionTwoKey = expressions[1];
            expressionThreeKey = expressions[2];
            expressionFourKey = expressions[3];
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

        public bool ActivateExpressionOne()
        {
            return previousKeyboardState.IsKeyUp(expressionOneKey) && currentKeyboardState.IsKeyDown(expressionOneKey);
        }

        public bool ActivateExpressionTwo()
        {
            return previousKeyboardState.IsKeyUp(expressionTwoKey) && currentKeyboardState.IsKeyDown(expressionTwoKey);
        }

        public bool ActivateExpressionThree()
        {
            return previousKeyboardState.IsKeyUp(expressionThreeKey) && currentKeyboardState.IsKeyDown(expressionThreeKey);
        }

        public bool ActivateExpressionFour()
        {
            return previousKeyboardState.IsKeyUp(expressionFourKey) && currentKeyboardState.IsKeyDown(expressionFourKey);
        }
    }
}
