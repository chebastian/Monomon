using MonogameBase;
using System;

namespace MonoGameBase.Editor.Commands
{
    public class ClearTileCommand : IEditCommand
    {
        private (int x, int y) _at;
        private (uint visual, TileType type) _from;
        private IMapWriter _map;

        public ClearTileCommand((int x, int y) at, (uint visual, TileType type) from, IMapWriter map)
        {
            _at = at;
            _from = from;
            _map = map;
        }

        public void Execute()
        {
            _map.RemoveEntityAt(_at.x, _at.y);
        }

        public void Undo()
        {
        }
    }

    public class CustomCommand : IEditCommand
    {
        private Action _cmd;
        private Action _undo;

        public CustomCommand(Action cmd, Action undo)
        {
            _cmd = cmd;
            _undo = undo;
        }

        public void Execute()
        {
            _cmd();
        }

        public void Undo()
        {
            _undo();
        }
    }



    public class ChangeTileCommand : IEditCommand
    {
        private TileData _data;
        private IMapWriter _writer;
        private (int x, int y) _at;
        private (uint visual, TileType type) _to;
        private (uint visual, TileType type) _from;

        public ChangeTileCommand(TileData data, (int x, int y) at, (uint visual, TileType type) from, IMapWriter map)
        {
            _writer = map;
            _at = at;
            _from = from;
            _data = data;
        }

        public void Execute()
        {
            _writer.SetTileAt(_at.x, _at.y, _data);
        }

        public void Undo()
        {
            _writer.SetTileAt(_at.x, _at.y, new TileData(_from.visual, _from.type));
        }
    }
}
