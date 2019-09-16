using Microsoft.Xna.Framework;
using Phantoms.Entities;

namespace Phantoms.Manipulators
{
    public static class Camera
    {
        private static Vector3 position;

        public static Matrix ViewMatrix { get; set; }
        public static int AreaWidth { get; set; }
        public static int AreaHeight { get; set; }

        public static void Update()
        {
            ViewMatrix = Matrix.CreateTranslation(position);
        }

        public static void Update(Vector2 followPosition, int followWidth, int followHeight)
        {
            AdjustPosition(followPosition, followWidth, followHeight);
            Update();
        }

        public static void Update(Body body)
        {
            Vector2 spriteSource = (body.Sprite?.Origin ?? Vector2.Zero) * (body.Scale * Global.ScreenScale);
            AdjustPosition(body.Position - spriteSource, body.Width, body.Height);
            Update();
        }

        public static void ScrollHorizontally(Vector2 followPosition, int followWidth, int scrollIncrement)
        {
            if (followPosition.X + (followWidth / 2) >= (Global.ScreenWidth / 2) &&
                followPosition.X + (followWidth / 2) <= AreaWidth - (Global.ScreenWidth / 2))
                position.X -= scrollIncrement;
        }

        public static void ScrollVertically(Vector2 followPosition, int followHeight, int scrollIncrement)
        {
            if (followPosition.Y + (followHeight / 2) >= (Global.ScreenHeight / 2) &&
                followPosition.Y + (followHeight / 2) <= AreaHeight - (Global.ScreenHeight / 2))
                position.Y -= scrollIncrement;
        }

        private static void AdjustPosition(Vector2 followPosition, int followWidth, int followHeight)
        {
            float positionHorizontal = -(followPosition.X - (Global.ScreenWidth / 2) + (followWidth / 2));
            float minWidth = -(AreaWidth - Global.ScreenWidth);
            float maxWidth = 0;
            float positionVertical = -(followPosition.Y - (Global.ScreenHeight / 2) + (followHeight / 2));
            float minHeight = -(AreaHeight - Global.ScreenHeight);
            float maxHeight = 0;
            position.X = MathHelper.Clamp(positionHorizontal, minWidth, maxWidth);
            position.Y = MathHelper.Clamp(positionVertical, minHeight, maxHeight);
        }
    }
}
