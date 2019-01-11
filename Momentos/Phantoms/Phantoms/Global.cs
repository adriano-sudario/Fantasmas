namespace Phantoms
{
    public static class Global
    {
        public enum HorizontalDirection { Left, Right }
        public enum VerticalDirection { Up, Down }

        public static float ScreenScale { get; set; }
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
    }
}
