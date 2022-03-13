using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Common.Game.Math;
using MonogameBase;
using MonoGameBase.Camera;
using MonoGameBase.Editor.Commands;
using MonoGameBase.Editor;
using MonoGameBase.Content;
using MonoGameBase.Logging;
using MonoGameBase.Drawing;
using MonoGameBase.Input;
using MonogameBase.Level;

namespace MonoGameBase.Level
{
    public class SerializedLevelData
    {
        public TileType[][] Tiles { get; set; }
        public uint[][] VisibleTiles { get; set; }
        public List<EntityDataDTO> Entities { get; set; }
        public List<StairDataDTO> Stairs { get; set; }
        public List<LevelGateDTO> Gates { get; set; }
        public Vec2 SpawnPoint { get; set; }

        public SerializedLevelData()
        {
            Entities = new List<EntityDataDTO>();
            Stairs = new List<StairDataDTO>();
            Gates = new List<LevelGateDTO>();
        }
    }

    public class EditorController
    {
        private IINputHandler _inputHandler;
        private LevelDataBase<EntityIds> _level;
        private TileMap _map;
        private IContentProvider _provider;
        private ILogger _logger;
        private Func<Rect> _winFunc;
        private Func<RenderMode> _renderer;
        private RenderMode _renderMode;
        private Func<string> _readUserInput;
        private UserQuery _queryUser;
        private IMapWriter _writer;
        List<IEditCommand> _history;
        private IRenderer _render;


        uint To1DIndex(Vec2 p)
        {
            return MapHelper.To1DIndex(p, Constants.SpriteMapW);
        }

        (int x, int y) ToTileIndex((float x, float y) p)
        {
            var worldx = p.x;
            var worldy = p.y;
            return _map.ToTileIndex((int)worldx, (int)worldy);
        }

        public EditorController(LevelDataBase<EntityIds> level, IContentProvider provider, ILogger logger, IINputHandler input, IMapWriter writer, IRenderer render, Func<RenderMode> getWindow, UserQuery query)
        {
            _queryUser = query;
            _writer = writer;
            _history = new List<IEditCommand>();
            _inputHandler = input;
            _level = level;
            _map = level.Map;
            _provider = provider;
            _logger = logger;
            _winFunc = () => getWindow().Window();
            _renderer = getWindow;
            _render = render;
        }

        private TileType IndexToType(uint index)
        {
            var type = _level.TypeMap.Where(x => x.Item1.start >= index && x.Item1.start + x.Item1.len <= index);
            foreach (var types in _level.TypeMap)
            {
                if (index >= types.Item1.start && index <= types.Item1.start + types.Item1.len)
                    return types.type;
            }

            return TileType.None;
        }

        public (bool special, uint start, uint mid, uint end, TileType type) GetSpecialTile(uint index)
        {
            var special = _level.SpecialTiles.Where(x => x.start == index);
            if (special.Any())
            {
                var item = special.First();
                return (true, item.start, item.mid, item.end, item.type);
            }

            return (false, 0, 0, 0, TileType.None);
        }

        (int x, int y) _selectedPos;

        public bool DrawTileMap { get; private set; }
        public (int x, int y) MouseTilePos { get; private set; }
        public (int x, int y) ScreenTilePos => (MousePos.X / Constants.TileW, MousePos.Y / Constants.TileH);
        public (int x, int y) WorldTilePos()
        {
            var pos = ToTileIndex(_renderer().ToWorldCoord(new Vec2(MousePos.X, MousePos.Y)));
            return (pos.x, pos.y);
        }

        public bool IsPressing { get; private set; }
        public Vec2 StartDrag { get; private set; }
        public (int X, int Y) MousePos { get; private set; }

        public void SaveLevel(string name)
        {
            var levelSerialized = new SerializedLevelData();
            levelSerialized.VisibleTiles = _level.Map.VisibleTiles;
            levelSerialized.Tiles = _level.Map.Tiles;
            levelSerialized.Entities = _level.EntityDtos.Where(x => !(x is StairDataDTO) && !(x is LevelGateDTO)).ToList();
            levelSerialized.Stairs = _level.EntityDtos.Where(x => x is StairDataDTO).Select(x => x as StairDataDTO).ToList();
            levelSerialized.SpawnPoint = _level.SpawnPoint;

            levelSerialized.Gates = _level.EntityDtos.Where(x => x is LevelGateDTO).Select(x => x as LevelGateDTO).ToList();

            var serialized = JsonSerializer.Serialize(levelSerialized);

            File.WriteAllText($"./Levels/{name}", serialized);
        }

        public void LoadLevel(SerializedLevelData data)
        {
            var map = new TileMap();
            map.CreateLevel(data.VisibleTiles, data.Tiles);
            _level.Map.CreateLevel(map.VisibleTiles, map.Tiles);
            _level.SpawnPoint = new Vec2(data.SpawnPoint.X, data.SpawnPoint.Y) ?? _level.SpawnPoint;
            _level.ClearEntities();
            foreach (var entity in data?.Stairs)
            {
                _level.AddEntity((int)entity.Pos.X, (int)entity.Pos.Y, entity.Id, entity);
            }
            foreach (var gate in data.Gates)
            {
                _level.AddEntity((int)gate.Pos.X, (int)gate.Pos.Y, gate.Id, gate);
            }

            foreach (var entity in data.Entities)
            {
                _level.AddEntity((int)entity.Pos.X, (int)entity.Pos.Y, entity.Id, entity);
            }
        }

        private IEnumerable<(Vec2 pos, uint type)> GetTilesInLine(Vec2 start, Vec2 end, uint first, uint fill, uint last, TileType type)
        {
            var list = new List<(Vec2, uint type)>();
            var d = end - start;
            list.Add((new Vec2((int)start.X, (int)start.Y), first));
            list.Add((new Vec2((int)end.X, (int)end.Y), last));

            if ((int)start.X == (int)end.X && (int)start.Y == (int)end.Y)
                return new List<(Vec2, uint)>();

            if (start.X != end.X)
            {
                var dx = d.X > 0 ? 1 : -1;
                for (var x = start.X + dx; (int)x != (int)end.X; x += dx)
                {
                    list.Add((new Vec2(x, start.Y), fill));
                }
            }
            else
            {
                var dx = d.Y > 0 ? 1 : -1;
                for (var x = start.Y + dx; (int)x != (int)end.Y; x += dx)
                {
                    list.Add((new Vec2((int)start.X, (int)x), fill));
                }
            }

            return list;
        }

        private void DrawTileLine(Vec2 start, Vec2 end, uint first, uint fill, uint last, TileType type)
        {
            var tiles = GetTilesInLine(start, end, first, fill, last, type);
            var cmd = new DrawLineCommand(tiles.Select(x => x.pos).ToList(),
                _writer, () =>
                {
                    foreach (var tile in tiles)
                    {
                        _writer.SetTileAt((int)tile.pos.X, (int)tile.pos.Y, new TileData(tile.type, type));
                    }
                });

            _history.Add(cmd);
            cmd.Execute();
        }
 
        private void HandleLine()
        {
            if (_inputHandler.IsKeyDown(KeyName.Editor_DrawLine))
            {
                if (_inputHandler.IsKeyDown(KeyName.Editor_PlaceTile) && !IsPressing)
                {
                    IsPressing = true;
                    CurrentState = EditState.LinePreview;
                    var start = WorldTilePos();
                    StartDrag = new Vec2(start.x, start.y);
                }
                else if (IsPressing && !_inputHandler.IsKeyDown(KeyName.Editor_PlaceTile))
                {
                    IsPressing = false;
                    CurrentState = EditState.Draw;
                    var end = WorldTilePos();
                    var endVec = GetMajorDirEnd(StartDrag, new Vec2(end.x, end.y));

                    var special = GetSpecialTile(To1DIndex(new Vec2(_selectedPos.x, _selectedPos.y)));
                    var index = To1DIndex(new Vec2(_selectedPos.x, _selectedPos.y));
                    if (special.special)
                    {
                        _selectedtype = special.type;
                        DrawTileLine(StartDrag, endVec, special.start, special.mid, special.end, _selectedtype);
                    }
                    else
                        DrawTileLine(StartDrag, endVec, index, index, index, _selectedtype);
                }
            }
            else
                IsPressing = false;
        }

        private Vec2 GetMajorDirEnd(Vec2 start, Vec2 end)
        {
            var endVec = new Vec2(end.X, end.Y);
            var d = endVec - StartDrag;
            endVec.Y = Math.Abs(d.X) > Math.Abs(d.Y) ? StartDrag.Y : endVec.Y;
            endVec.X = Math.Abs(d.X) > Math.Abs(d.Y) ? endVec.X : StartDrag.X;
            return endVec;
        }


        public enum EditState
        {
            Draw,
            LinePreview,
            Fill,
            EditType,
            EndPreview
        }


        EditState CurrentState { get; set; }
        public (int x, int y) MouseLock { get; private set; }

        public void Update(float dt)
        {
            var ms = (X:_inputHandler.GetCursorX(),Y: _inputHandler.GetCursorY());
            MousePos = (ms.X, ms.Y);
            HandleLine();


            if (_inputHandler.IsKeyDown(KeyName.Editor_Fill))
            {
                CurrentState = EditState.Fill;
            }

            var mouse = WorldTilePos();
            MouseTilePos = WorldTilePos();

            if (_inputHandler.IsKeyPressed(KeyName.Editor_Undo))
            {
                UndoLastCommand();
            }

            if (_inputHandler.IsKeyPressed(KeyName.Editor_SwapHeading))
            {
                SwapCurrentHeading();
            }

            if (_inputHandler.MouseButtonState().right == BufferedMouseState.Down && !DrawTileMap)
            {
                var tile = _writer.GetTile(MouseTilePos.x, MouseTilePos.y);
                var cmd = new ClearTileCommand(MouseTilePos,
                    (tile.visual, tile.type), _writer);

                cmd.Execute();
                _history.Add(cmd);
            }

            bool isFinishStairCommand = _inputHandler.MouseButtonState().left == BufferedMouseState.Released && CurrentState == EditState.EndPreview;
            if (isFinishStairCommand)
            {

                WriteStairsToMap();
            }

            bool isAddingToMap = (_inputHandler.MouseButtonState().left == BufferedMouseState.Down || _inputHandler.IsKeyDown(KeyName.Select)) && !DrawTileMap;
            if (isAddingToMap)
            {
                if (CurrentState == EditState.Draw)
                {
                    var tile = _writer.GetTile((int)WorldTilePos().x, (int)WorldTilePos().y);
                    var type = _level.IndexToEntity(_visualIndex);

                    IEditCommand cmd = new ChangeTileCommand(new TileData(_visualIndex, _selectedtype), WorldTilePos(),
                        (tile.visual, tile.type), _writer);

                    if (type.id == EntityIds.Stair)
                    {
                        CurrentState = EditState.EndPreview;
                        MouseLock = MouseTilePos;
                        return;
                    }

                    if (type.found && type.id != EntityIds.None)
                    {
                        var pos = WorldTilePos();
                        cmd = CreateAddEntityCommand(type, pos);
                    }

                    if (tile.visual != _visualIndex || tile.type != _selectedtype)
                    {
                        _history.Add(cmd);
                        cmd.Execute();
                    }


                }
                else if (CurrentState == EditState.Fill)
                {
                    var fillCmd = new FillCommand(mouse, (_visualIndex, _selectedtype), _writer.GetTile(mouse.x, mouse.y), _writer);
                    _history.Add(fillCmd);
                    fillCmd.Execute();
                    CurrentState = EditState.Draw;
                }
            }

            var didDraw = DrawTileMap;
            if (_inputHandler.IsKeyPressed(KeyName.Editor_ShowTiles) || DrawTileMap)
            {
                DrawTileMap = true;
                _logger.Clear();
                var tilePos = ScreenTilePos;
                _logger.PrintLine(tilePos.ToString());
                _logger.PrintLine(To1DIndex(new Vec2(tilePos.x, tilePos.y)).ToString());
                MouseTilePos = tilePos;
                if (_inputHandler.MouseButtonState().left == BufferedMouseState.Released || _inputHandler.IsKeyPressed(KeyName.Editor_PlaceTile))
                {
                    _selectedPos.x = tilePos.x;
                    _selectedPos.y = tilePos.y;
                    _visualIndex = (uint)(_selectedPos.y * Constants.SpriteMapW + _selectedPos.x);
                    var idx = To1DIndex(new Vec2(_selectedPos.x, _selectedPos.y));
                    _selectedtype = this.IndexToType(_visualIndex);
                    DrawTileMap = false;
                }
            }

            if (didDraw == true && DrawTileMap && _inputHandler.IsKeyPressed(KeyName.Editor_ShowTiles))
            {
                DrawTileMap = false;
            }

        }

        private void WriteStairsToMap()
        {
            var tiles = GetDiagonalTiles(new Vec2(MouseTilePos.x, MouseTilePos.y), new Vec2(MouseLock.x, MouseLock.y));
            var heading = tiles.dir.X > 0 ? Heading.Right : Heading.Left;
            {
                //Begin
                {
                    var tile = _writer.GetTile(MouseTilePos.x, MouseTilePos.y);
                    var cmd = new CustomCommand(() => _writer.AddEntity(
                        MouseLock.x,MouseLock.y,
                        EntityIds.Stair,
                        new StairDataDTO(new Vec2(MouseLock.x,MouseLock.y),_heading,tiles.dir)),
                        () => { });

                    _history.Add(cmd);
                    cmd.Execute();
                }

                var arrtiles = tiles.tiles.ToArray();
                foreach (var pos in arrtiles[0..^1])
                {
                    var tile = _writer.GetTile((int)pos.X, (int)pos.Y);
                    //uint visualTile = tiles.dir.X > 0 ? 333 : 334;
                    uint visualTile = 333;
                    var cmd = new ChangeTileCommand(new TileData(visualTile, TileType.None), ((int)pos.X, (int)pos.Y), tile, _writer);
                    _history.Add(cmd);
                    cmd.Execute();
                }

                //End
                {
                    var tile = _writer.GetTile(MouseTilePos.x, MouseTilePos.y);
                    var end = tiles.tiles.Last();
                    //var cmd = new ChangeTileCommand(new StairData(333, TileType.Stair, heading, tiles.dir * -1), (x: (int)end.X, y: (int)end.Y), (tile.visual, tile.type), _writer);
                    var cmd = new CustomCommand(() => _writer.AddEntity(
                        (int)end.X,(int)end.Y,
                        EntityIds.Stair,
                        new StairDataDTO(new Vec2(end.X,end.Y),_heading,tiles.dir*-1)),
                        () => { });
                    _history.Add(cmd);
                    cmd.Execute();
                }
            }
            CurrentState = EditState.Draw;
        }

        private void SwapCurrentHeading()
        {
            _heading = _heading == Heading.Left ? Heading.Right : Heading.Left;
        }

        private void UndoLastCommand()
        {
            var lst = _history.Last();
            _history.Remove(lst);
            lst.Undo();
        }

        private IEditCommand CreateAddEntityCommand((bool found, EntityIds id) type, (int x, int y) pos)
        {
            IEditCommand cmd;
            if (type.id == EntityIds.LevelGate)
            {
                cmd = new CustomCommand(() =>
                {
                    var gate = _queryUser.GetGateData();
                    _writer.AddEntity(pos.x, pos.y, type.id, new LevelGateDTO(
                        Level: gate.level,
                        Pos: new Vec2(pos.x, pos.y),
                        Name: gate.name,
                        SpawnPoint: gate.spawn));
                },
                () =>
                {
                    _writer.RemoveEntityAt(pos.x, pos.y);
                });
            }
            else if (type.id == EntityIds.SpawnPoint)
            {
                cmd = new CustomCommand(() =>
                {
                    _writer.SetSpawnPoint(pos.x, pos.y);
                },
                () =>
                {
                    _writer.RemoveEntityAt(pos.x, pos.y);
                });
            }
            else
            {
                cmd = new CustomCommand(() =>
                {
                    //_writer.AddEntity(pos.x, pos.y, type.id, new WriteEntityData(_visualIndex, _selectedtype, _heading));
                    _writer.AddEntity(pos.x, pos.y, type.id, new EntityDataDTO(type.id,new Vec2(pos.x,pos.y),_heading));
                },
                () =>
                {
                    _writer.RemoveEntityAt(pos.x, pos.y);
                });

            }

            return cmd;
        }

        public Vec2 SelectedTileIndex()
        {
            return new Vec2(MouseTilePos.x, MouseTilePos.y);
        }


        TileType _selectedtype = TileType.Bridge;
        private uint _visualIndex;
        private Heading _heading;

        private (List<Vec2> tiles, Vec2 dir) GetDiagonalTiles(Vec2 a, Vec2 b)
        {
            var res = new List<Vec2>();
            var dx = a.X - b.X > 0 ? 1 : -1;
            var dy = a.Y - b.Y;

            for (int nextY = 0; nextY < Math.Abs(dy); nextY++)
            {
                var x = b.X + (dx * nextY);
                var y = b.Y + ((dy > 0 ? 1 : -1) * nextY);
                res.Add(new Vec2(x, y));
            }

            return (res, new Vec2(dx, dy));
        }

        public void Draw()
        {
            if (DrawTileMap)
            {
                _render.Draw("levelMap", new Vec2());
            }
            var pos = ToTileIndex(_renderer().ToWorldCoord(new Vec2(MousePos.X, MousePos.Y)));
            var wp = _renderer().ToScreenCoord(new Vec2(pos.x * Constants.TileW, pos.y * Constants.TileH));
            _render.Draw("tileMark", new Vec2(wp.x, wp.y));

            var tileSrc = _selectedtype switch
            {
                TileType.Wall => new Vec2(0, 0),
                TileType.Bridge => new Vec2(5, 3),
                TileType.Ladder => new Vec2(7, 9),
                _ => new Vec2(0, 0)
            };

            _logger.PrintLine("############");
            _logger.PrintLine("Editor data");
            _logger.PrintLine("############");
            _logger.PrintLine($"Faceing: {_heading}");
            _logger.PrintLine("");
            _logger.PrintLine("############");
            if (CurrentState == EditState.LinePreview)
            {
                var end = WorldTilePos();
                var le = GetMajorDirEnd(StartDrag, new Vec2(end.x, end.y));
                var tiles = GetTilesInLine(StartDrag, le, 0, 0, 0, TileType.Wall);
                foreach (var tile in tiles)
                {
                    var local = _renderer().Window().ToLocalCoords(tile.pos.X * Constants.TileW, tile.pos.Y * Constants.TileH);
                    _render.Draw("tileMark", new Vec2(local.x, local.y));
                }
            }

            if (CurrentState == EditState.EndPreview)
            {
                foreach (var tile in GetDiagonalTiles(new Vec2(MouseTilePos.x, MouseTilePos.y), new Vec2(MouseLock.x, MouseLock.y)).tiles)
                {
                    var local = _renderer().Window().ToLocalCoords(tile.X * Constants.TileW, tile.Y * Constants.TileH);
                    _render.Draw("tileMark", new Vec2(local.x, local.y));
                }
            }

            var tileX = 1;
            var tileY = 28;
            if (_selectedtype != TileType.None)
                _render.Draw("levelMap", new Vec2(tileX * Constants.TileW, tileY * Constants.TileH),(int)(tileSrc.X * Constants.TileW), ((int)tileSrc.Y * Constants.TileH), 16, 16);
        }
    }
}
