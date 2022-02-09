namespace MonogameBase
{
    public class Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Solid { get; private set; }

        public Tile(int x, int y, TileType type)
        {
            X = x; Y = y; Solid = type == TileType.Wall;
        }
    }
}