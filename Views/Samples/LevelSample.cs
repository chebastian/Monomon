using Common.Game.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonogameBase;
using MonogameBase.Camera;
using MonoGameBase.Input;
using MonoGameBase.Level;
using Monomon.State;
using Monomon.Views.Scenes;
using System;
using System.IO;

namespace Monomon.Views.Samples;

using MonoGameBase.Animation;
using Monomon.Battle;
using Monomon.Data;
using Monomon.Effects;
using Monomon.Mons;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class LevelSample : SceneView
{
    private IINputHandler input;
    private SceneStack stack;
    private PaletteEffect _paletteEffect;
    private FadeEffect _fade;
    private Texture2D _tileSprites;
    private Texture2D _playerSprites;
    private Texture2D _spriteMap;
    private TileMap _map;
    private SpriteFont _font;
    private SerializedLevelData _levelData;
    Mons.Mobmon _playerMon;
    private AnimationPlayer _animPlayer;
    private List<Frame> _currentAnim;
    private Player _player;

    private Vec2 windowPos;

    public Vec2 ToGridTopLeft(Vec2 pos)
    {
        return new Vec2(((int)(pos.X / 16)) * 16, ((int)(pos.Y / 16)) * 16);
    }

    public LevelSample(GraphicsDevice gd, IINputHandler input, SceneStack stack, ContentManager content, PaletteEffect effect, FadeEffect fade) : base(gd, content)
    {
        this.input = input;
        this.stack = stack;
        _paletteEffect = effect;
        _content = content;
        _player = new Player();
        _player.Pos = new Vec2(128, 128);
        windowPos = new Vec2(0, 0);
        _spriteMap = content.Load<Texture2D>("spritemap");
        _map = new TileMap();
        _font = content.Load<SpriteFont>("File");
        _levelData = System.Text.Json.JsonSerializer.Deserialize<SerializedLevelData>(File.ReadAllText("./Levels/grass.json")) ?? throw new ArgumentNullException("level");
        _map.CreateLevel(_levelData.VisibleTiles, _levelData.Tiles);
        _fade = fade;

        _tileSprites = content.Load<Texture2D>("levelMap");
        _playerSprites = content.Load<Texture2D>("playerTopDown");
        _playerMon = new Mons.Mobmon("Player", 10, (new MonStatus(2, 7, 3)));

        _animPlayer = new AnimationPlayer(7.0f);
        _currentAnim = SourceForDir(new Vec2(0, 1));
        _animPlayer.ChangeAnimation(_currentAnim);
    }

    List<Frame> LeftAnim = new () { new(0,0), new(0,3) };
    List<Frame> RightAnim = new () { new(1,0), new(1,3) };
    List<Frame> UpAnim = new () { new(0,2), new(1,2) };
    List<Frame> DownAnim = new () { new(0,1), new(1,1) };

    public override void LoadScene(ContentManager content)
    {
    }

    List<Frame> SourceForDir(Vec2 inDir)
    {
        var dx = inDir.Normalize();

        List<Frame> frame = dx switch
        {
            var dir when dir.X > 0 => RightAnim,
            var dir when dir.X < 0 => LeftAnim,
            var dir when dir.Y > 0 => DownAnim,
            var dir when dir.Y < 0 => UpAnim,
            _ => new() { _currentAnim[0] }
        };

        return frame;
    }

    public override void Update(double time)
    {
        _dx = input.GetX();
        _dy = input.GetY();

        var winPos = _player.Pos + new Vec2(-(Window.Width * 0.5f), -(Window.Height * 0.5f));
        Window.X = winPos.X;
        Window.Y = winPos.Y;

        var currentFrame = SourceForDir(_player.Target - _player.OgPos);
        if (currentFrame != _currentAnim)
        {
            _currentAnim = currentFrame;
            _animPlayer.ChangeAnimation(_currentAnim);
        }

        var vel = new Vec2(_dx * 16, _dy * 16) * time;

        var playerRect = new Rect(_player.Pos.X, _player.Pos.Y, 16, 16);

        if (_dx != 0 || _dy != 0)
        {
            var bothPressed = (_dx != 00 && _dy != 0.0);
            _dy = bothPressed ? 0 : _dy;

            var targetOnGrid = ToGridTopLeft(_player.Center + new Vec2(_dx * TileValues.TileW, _dy * TileValues.TileH));
            var tileAtTarget = _map.GetTileAt((int)targetOnGrid.X / TileValues.TileW, (int)targetOnGrid.Y / TileValues.TileH);

            if (_player.Dist == 0.0f && tileAtTarget != TileType.Wall)
                _player.WalkInDirection(targetOnGrid, OnPlayerEnterTile);
        }

        if(_player.Dist > 0.0f)
        {
            _animPlayer.Update((float)time); 
        }
        else
        {
            _animPlayer.ChangeAnimation(_currentAnim);
        }

        _player.Update((float)time);
    }

    static readonly ReadOnlyCollection<uint> _grassTiles = new(new uint [] { 543,452,569 } );
    private void OnPlayerEnterTile()
    {
        var tileAtFeet = _map.GetTile((int)_player.Center.X / TileValues.TileW, (int)_player.Center.Y / TileValues.TileH);
        if(_grassTiles.Any(grass => tileAtFeet.visual == grass))
        {
            if (Random.Shared.NextDouble() < 0.25)
            {
                var battle = new BattleSample(_graphics,
                                              input,
                                              new() { _playerMon, new Mobmon("Another", 10, new MonStatus(2,12,2)), new Mobmon("Third", 13, new MonStatus(3,13,3)) },
                                              CreateRandomEnemy(),
                                              stack,
                                              _content,
                                              _paletteEffect,
                                              _fade); ;
                _fade.FadeOut(stack, () => { });
                stack.Push(new SceneState(battle, input), () => { }, () => { });
                battle.LoadScene(_content);
            }
        }
    }

    private Mobmon CreateRandomEnemy()
    {
        return new Mobmon("Enemy",
                          Random.Shared.Next(3, 10),
                          new MonStatus(5, 4, 5));
    }

    public Rect Window = new Rect(0, 0, 160, 144);
    private int _dx;
    private int _dy;

    protected override void OnDraw(SpriteBatch batch)
    {

        _graphics.Clear(Color.White);

        var renderpos = (x: 0, y: 0);

        foreach (var tile in _map.GetTilesInside(new Rect(Window.X, Window.Y, Window.Width, Window.Height)))
        {
            var tilepos = (x: tile.rect.X / (TileValues.TileW), y: tile.rect.Y / (TileValues.TileH));
            var visual = _map.GetVisibleTileAt((int)tilepos.x, (int)tilepos.y);
            var tileindex = MapHelper.To2DIndex(visual, TileValues.SpriteMapW);


            var tileSrc = (X: (int)tileindex.X * (TileValues.TileW), Y: (int)tileindex.Y * (TileValues.TileH));

            var renderPos = ToWindowPosition(new Vec2(tile.rect.X, tile.rect.Y));

            if (visual != (uint)TileType.None)
            {
                batch.Draw(_tileSprites,
                    renderPos,
                    new Rectangle(tileSrc.X, tileSrc.Y, TileValues.TileW, TileValues.TileH),
                    Color.White);
            }
        }

        var playerCamPos = ToWindowPosition(_player.Pos);
        var frame = _animPlayer.CurrentFrame();
        batch.Draw(_playerSprites,
            new Rectangle((int)playerCamPos.X, (int)playerCamPos.Y, 16, 16),
            new Rectangle((int)frame.Source.x * 16, (int)frame.Source.y*16,16,16),
            Color.White
            );

        MarkTile(ToGridTopLeft(_player.Center));
        MarkTile(ToGridTopLeft(_player.Center + new Vec2(_dx * TileValues.TileW, _dy * TileValues.TileH)));

        void MarkTile(Vec2 gridPos)
        {
            var pcenter = ToWindowPosition(gridPos);
            batch.Draw(_playerSprites,
                new Rectangle((int)pcenter.X, (int)pcenter.Y, 2, 2),
                new Rectangle(0, 0, 1, 1),
                Color.White
                );

            batch.Draw(_playerSprites,
                new Rectangle((int)pcenter.X + TileValues.TileW, (int)pcenter.Y, 2, 2),
                new Rectangle(0, 0, 1, 1),
                Color.White
                );

            batch.Draw(_playerSprites,
                new Rectangle((int)pcenter.X + TileValues.TileW, (int)pcenter.Y + TileValues.TileH, 2, 2),
                new Rectangle(0, 0, 1, 1),
                Color.White
                );

            batch.Draw(_playerSprites,
                new Rectangle((int)pcenter.X, (int)pcenter.Y + TileValues.TileH, 2, 2),
                new Rectangle(0, 0, 1, 1),
                Color.White
                );
        }



        Vector2 ToWindowPosition(Vec2 pos)
        {
            var wv = CameraHelper.ToWindowPosition(pos, new Vec2(Window.X, Window.Y));
            return new Vector2(wv.X + renderpos.x, wv.Y + renderpos.y);
        }

    }
}


