using MonogameBase;
using MonoGameBase.Editor;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameBase.Editor.Commands
{
    public class FillCommand : IEditCommand
    {
        private (int x, int y) _pos;
        private (uint visual, TileType type) _to;
        private IMapWriter _map;
        private (uint visual, TileType type) _from;

        public FillCommand((int x, int y) pos, (uint visual, TileType type) to, (uint visual, TileType type) from, IMapWriter map)
        {
            _pos = pos;
            _to = to;
            _map = map;
            _from = from;
        }

        public void Execute()
        {
            FillAtPosition(_pos, _to, _map);
        }

        public void Undo()
        {
            FillAtPosition(_pos, _from, _map);
        }

        private static void FillAtPosition((int x, int y) mouse, (uint visual, TileType type) proto, IMapWriter map)
        {
            var dirs = new[] { (-1, 0), (0, -1), (1, 0), (0, 1) };
            var typeAtPos = map.GetTile(mouse.x, mouse.y).visual;

            IEnumerable<(uint tile, bool visited, (int x, int y) pos)> aroundPoint = Around(mouse, dirs, typeAtPos);

            var visited = new List<(int x, int y)>();

            //foreach (var itemAround in aroundPoint)
            //{
            //    _map.SetVisibleTile(itemAround.pos.x, itemAround.pos.y, tileIdx);
            //}

            SetTilesArund(aroundPoint.Select(x => x.pos).ToList(), visited);

            void SetTilesArund(List<(int x, int y)> toSet, List<(int x, int y)> history)
            {
                foreach (var itemAround in toSet.Except(history))
                {
                    visited.Add(itemAround);
                    map.SetTileAt(itemAround.x, itemAround.y, new TileData(proto.visual, proto.type));
                    SetTilesArund(Around(itemAround, dirs, typeAtPos).Select(x => x.pos).ToList(), visited);
                }
            }

            IEnumerable<(uint tile, bool visited, (int x, int y) pos)> Around((int x, int y) mouse, (int, int)[] dirs, uint typeAtPos)
            {
                return dirs.Select(x => (tile: map.GetTile(mouse.x + x.Item1, mouse.y + x.Item2).visual, visited: false, pos: (x: mouse.x + x.Item1, y: mouse.y + x.Item2))).
                    Where(x => x.tile == typeAtPos);
            }
        }
    }
}
