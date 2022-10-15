namespace Monomon.Views.Constants
{
    internal static class UIValues
    {
        public const int TileSz = 16;
        public const int WindowTileSz = 9;
        public const int PlayerMenuX = 4;
        public const int PlayerMenuH = 3;
        public const int PlayerMenuY = WindowTileSz - PlayerMenuH;

        public const int PlayerPoirtraitY = 3 * TileSz;
        public const int PlayerPoirtraitX = 0;
        public const int PortraitW = 4 * TileSz;
        public const int PortraitH = 3 * TileSz;


        public const int WinH = 144;
        public const int WinW = 160;
        public static int PlayerHudX => 0;
        public static int PlayerHudY => 0;
        public static int OponentHudX => 6 * TileSz;
        public static int BattleMessageY => 50;
    }
}