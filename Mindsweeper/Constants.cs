using System.Drawing;

namespace Mindsweeper
{
    public static class Constants
    {
        public static class Colors
        {
            public static readonly Color White = Color.FromArgb(255, 255, 255);
            public static readonly Color Bomb = Color.FromArgb(0, 0, 0);
            public static readonly Color Flag = Color.FromArgb(20, 19, 19);
            public static readonly Color BackgroundColor = Color.FromArgb(189, 189, 189);
            public static readonly Color One = Color.FromArgb(0, 0, 255);
            public static readonly Color Two = Color.FromArgb(0, 123, 0);
            public static readonly Color Three = Color.FromArgb(253, 6, 6);
            public static readonly Color Four = Color.FromArgb(6, 6, 125);
            public static readonly Color Five = Color.FromArgb(125, 6, 6);
            public static readonly Color Six = Color.FromArgb(6, 125, 125);
        }

        public static class BombValues
        {
            public const int Bomb = -2;
            public const int Unknown = -1;
            public const int Safe = 0;

        }
    }
}
