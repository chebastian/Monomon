using Common.Game.Math;
using MonogameBase;
using System;
using System.Collections.Generic;

namespace MonoGameBase.Editor.Commands
{
    public class DrawLineCommand : IEditCommand
    {
        private Action _action;
        private IMapWriter _map;
        private IEnumerable<Vec2> _tiles;
        private List<(Vec2 pos, (uint visual, TileType type))> _history;

        public DrawLineCommand(IEnumerable<Vec2> tiles, IMapWriter map, Action setTiles)
        {
            _action = setTiles;
            _map = map;
            _tiles = tiles;
        }

        public void Execute()
        {
            _history = new List<(Vec2 pos, (uint visual, TileType type))>();
            foreach (var tile in _tiles)
            {
                _history.Add((tile, _map.GetTile((int)tile.X, (int)tile.Y)));
            }

            _action();
        }

        public void Undo()
        {
            foreach (var tile in _history)
            {
                _map.SetTileAt((int)tile.pos.X, (int)tile.pos.Y, new TileData(tile.Item2.visual, tile.Item2.type));
            }
        }
    }
}
