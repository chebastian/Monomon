using Common.Game.Math;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MonogameBase
{
    public record EntityDataDTO(EntityIds Id, Vec2 Pos, Heading Faceing);
    public record StairDataDTO(Vec2 Pos, Heading Faceing,Vec2 length) : EntityDataDTO(EntityIds.Stair,Pos,Faceing);
    public record LevelGateDTO(string Level, Vec2 Pos,string Name,string SpawnPoint) : EntityDataDTO(EntityIds.LevelGate,Pos,Heading.Left);

    public record WriteEntityData(uint visualIndex, TileType type, Heading heading) : TileData(visualIndex,type);

    public enum EntityIds
    {
        Heart,
        Candle,
        Walker,
        None,
        Medusa,
        Tower,
        Stair,
        LevelGate,
        SpawnPoint,
        Dragola,
        DoubleJumpPickup,
        Door,
        Key
    }

    public static class MapHelper
    {
        public static uint To1DIndex(Vec2 pos, int mapw)
        {
            return (uint)(pos.Y * mapw + pos.X);
        }

        public static Vec2 To2DIndex(uint index, int mapw)
        {
            if (index == 0)
                return new Vec2(0, 0);

            return new Vec2(index % mapw, index / mapw);
        }
    }

    public class TileMap
    {
        public TileMap()
        {
            _tiles = CreateLevel(100,50);
        }

        public (int x, int y) ToTileIndex(int x, int y) => ((int)x / TileW, (int)y / TileH);

        public TileType[][] CreateLevel(int w, int h)
        {
            var res = new List<List<TileType>>();

            var filled = Enumerable.Range(0, w).Select(x => TileType.Wall).ToList();
            var midr = new List<TileType>();

            midr.Add(TileType.Wall);
            midr.AddRange(Enumerable.Repeat(TileType.None, w - 2));
            midr.Add(TileType.Wall);


            res.Add(filled);
            res.AddRange(Enumerable.Repeat(midr, h - 2));
            res.Add(filled);


            var arr =  res.Select(x => x.ToArray()).ToArray();
            VisibleTiles = TilesToVisual(arr);
            return arr;
        }

        private uint[][] TilesToVisual(TileType[][] tiles)
        {
            return tiles.Select(x => x.Select(y => (uint)y).ToArray()).ToArray();
        }

        public TileType[][] CreateLevel()
        {
            var level = new[]
            {
                new[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
                new[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                new[]{1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,2,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,1},
                new[]{1,1,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,1},
                new[]{1,1,2,2,0,0,0,0,0,0,0,0,0,1,2,2,2,2,2,2,2,1,0,0,0,0,0,0,0,0,3,0,1,1},
                new[]{1,1,2,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,3,0,1,1},
                new[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                new[]{1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,0,0,1,1,1,1,1,1,1,1,1,0,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,1},
                new[]{1,1,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,1},
                new[]{1,1,0,1,1,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,1,0,0,0,1,0,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1},
                new[]{1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,1,1},
                new[]{1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
                new[]{1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,1},
                new[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                new[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                new[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            };


            var res = level.Select(x => x.Select(x => x switch { 1 => TileType.Wall, 0 => TileType.None, 2 => TileType.Bridge, 3 => TileType.Ladder }).ToArray()).ToArray();
            VisibleTiles = res.Select(x => x.Select(y => (uint)y).ToArray()).ToArray();
            return res;
        }

        public void CreateLevel(uint[][] visual, TileType[][] collision)
        {
            _tiles = collision;
            VisibleTiles = visual;
        }

        public uint[][] VisibleTiles { get; private set; }


        internal void CreateLevel(TileType[][] tiles)
        {
            _tiles = tiles;
            VisibleTiles = tiles.Select(x => x.Select(y => (uint)y).ToArray()).ToArray();
        }

        public void SetVisibleTile(int x, int y, uint type)
        {
            if (x < 0 || y < 0)
                return;
            if (y >= VisibleTiles.Length)
            {
                return;
            }
            if (x >= VisibleTiles[0].Length)
                return;

            VisibleTiles[y][x] = type;
        }



        public void ToggleTileWall((int x, int y) mouse, TileType wall)
        {
            try
            {
                _tiles[mouse.y][mouse.x] = wall;
            }
            catch { }
        }

        public TileType GetTileAt(int x, int y)
        {
            if (y >= Tiles.Length || y < 0)
                return TileType.None;
            if (x <= 0 || Tiles[0].Length <= x)
                return TileType.None;

            return Tiles[y][x];
        }

        public (uint visual, TileType type) GetTile(int x, int y)
        {
            return (GetVisibleTileAt(x, y), GetTileAt(x, y));
        }
        public uint GetVisibleTileAt(int x, int y)
        {
            if (y >= VisibleTiles.Length || y < 0)
                return uint.MaxValue;
            if (x < 0 || VisibleTiles[0].Length <= x)
                return uint.MaxValue;

            return VisibleTiles[y][x];
        }

        private int TileW = Constants.TileW;
        private int TileH = Constants.TileH;

        public List<(Rect rect, TileType type, bool solid)> GetTiles()
        {
            var tiles = Tiles.Select((row, y) =>
            row.Select((tile, x) =>
            (new Rect(x * TileW, y * TileH, TileW, TileH), tile, tile == TileType.Wall))).ToList();

            var flat = tiles.SelectMany(x => x.Select(y => y)).ToList();
            return flat;
        }

        public List<TileInfo> GetTilesInside(Rect r)
        {
            var res = new List<TileInfo>();
            var worldH = Tiles.Length;
            var worldW = Tiles[0].Length;

            (int x, int y) start = ToTileIndex((int)r.X, (int)r.Y);
            (int x, int y) end = ToTileIndex((int)r.Right, (int)r.Bottom);

            for (int y = start.y; y < worldH && y <= end.y; y++)
            {
                for (int x = start.x; x < worldW && x <= end.x; x++)
                {
                    if (GetTileAt(x, y) == TileType.Bridge)
                    {
                        var solid = true;
                        res.Add(new TileInfo(new Rect(x * TileW, y * TileH, TileW, TileH), GetTileAt(x, y), solid));
                    }
                    else
                    {
                        res.Add(new TileInfo(new Rect(x * TileW, y * TileH, TileW, TileH), GetTileAt(x, y), GetTileAt(x, y) == TileType.Wall));
                    }
                }
            }

            res = res.Distinct().ToList();

            return res;
        }

        private TileType[][] _tiles;
        public TileType[][] Tiles { get => _tiles; }
    }
}