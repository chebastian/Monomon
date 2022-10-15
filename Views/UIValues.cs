namespace Monomon.Views.Constants
{
    internal static class UIValues
    {
        public static int TileSz = 16;
        public static int WindowTileSz = 9;
        public static int PlayerMenuX = 5 * TileSz;
        public static int PlayerMenuH = 3;
        public static int PlayerMenuY = WindowTileSz - PlayerMenuH;

        public static int PlayerPoirtraitY = 3 * TileSz;
        public static int PlayerPoirtraitX = 0;
        public static int PortraitW = 4 * TileSz;
        public static int PortraitH = 3 * TileSz;


        public static int WinH = 144;
        public static int WinW = 160;
        public static int PlayerHudX => 0;
        public static int PlayerHudY => 0;
        public static int OponentHudX = 6 * TileSz;
        public static int BattleMessageY = 6 * TileSz;

        public static int ListMinW = TileSz * 4;
    }
}