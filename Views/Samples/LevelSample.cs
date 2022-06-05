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

namespace Monomon.Views.Samples
{
    using Monomon.Battle;
    using Monomon.Data;
    using Monomon.Effects;
    using Monomon.Mons;
    using System.Collections.Generic;

    public class LevelSample : SceneView
    {
        private IINputHandler input;
        private StateStack<double> stack;
        private PaletteEffect _paletteEffect;
        private FadeEffect _fade;
        private Texture2D _tileSprites;
        private Texture2D _playerSprites;
        private RenderTarget2D _renderTarget;
        private Texture2D _spriteMap;
        private TileMap _map;
        private SpriteFont _font;
        private SerializedLevelData _levelData;
        Mons.Mobmon _playerMon;

        private Player _player;

        private Vec2 windowPos;

        public Vec2 ToGridTopLeft(Vec2 pos)
        {
            return new Vec2(((int)(pos.X / 16)) * 16, ((int)(pos.Y / 16)) * 16);
        }

        public LevelSample(GraphicsDevice gd, IINputHandler input, StateStack<double> stack, ContentManager content, PaletteEffect effect, FadeEffect fade) : base(gd, content)
        {
            this.input = input;
            this.stack = stack;
            _paletteEffect = effect;
            _content = content;
            _player = new Player();
            _player.Pos = new Vec2(128, 128);
            windowPos = new Vec2(0, 0);
            _renderTarget = new RenderTarget2D(_graphics, 160, 144);
            _spriteMap = content.Load<Texture2D>("spritemap");
            _map = new TileMap();
            _font = content.Load<SpriteFont>("File");
            _levelData = System.Text.Json.JsonSerializer.Deserialize<SerializedLevelData>(File.ReadAllText("./Levels/grass.json")) ?? throw new ArgumentNullException("level");
            _map.CreateLevel(_levelData.VisibleTiles, _levelData.Tiles);
            _fade = fade;

            _tileSprites = content.Load<Texture2D>("levelMap");
            _playerSprites = content.Load<Texture2D>("player");
            _playerMon = new Mons.Mobmon("Player", 10, (new MonStatus(2, 7, 3)));
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            _dx = input.GetX();
            _dy = input.GetY();

            var winPos = _player.Pos + new Vec2(-(Window.Width * 0.5f), -(Window.Height * 0.5f));
            Window.X = winPos.X;
            Window.Y = winPos.Y;


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

            _player.Update((float)time);
        }

        private void OnPlayerEnterTile()
        {
            var tileAtFeet = _map.GetTile((int)_player.Center.X / TileValues.TileW, (int)_player.Center.Y / TileValues.TileH);
            if (new List<uint>() { 543, 452, 569 }.Contains(tileAtFeet.visual))
            {
                if (Random.Shared.NextDouble() < 0.25)
                {
                    var battle = new BattleSample(_graphics,
                                                  input,
                                                  _playerMon,
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
                              new MonStatus(2, 4, 5));
        }

        public Rect Window = new Rect(0, 0, 160, 144);
        private int _dx;
        private int _dy;

        protected override void OnDraw(SpriteBatch batch)
        {
            batch.End();

            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(Color.White);
            _paletteEffect.EffectBegin(batch);

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
            batch.Draw(_playerSprites,
                new Rectangle((int)playerCamPos.X, (int)playerCamPos.Y, 16, 16),
                SourceForDir(_player.Target - _player.OgPos),
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
                    Color.Black
                    );

                batch.Draw(_playerSprites,
                    new Rectangle((int)pcenter.X + TileValues.TileW, (int)pcenter.Y, 2, 2),
                    new Rectangle(0, 0, 1, 1),
                    Color.Black
                    );

                batch.Draw(_playerSprites,
                    new Rectangle((int)pcenter.X + TileValues.TileW, (int)pcenter.Y + TileValues.TileH, 2, 2),
                    new Rectangle(0, 0, 1, 1),
                    Color.Black
                    );

                batch.Draw(_playerSprites,
                    new Rectangle((int)pcenter.X, (int)pcenter.Y + TileValues.TileH, 2, 2),
                    new Rectangle(0, 0, 1, 1),
                    Color.Black
                    );
            }

            static Rectangle SourceForDir(Vec2 inDir)
            {
                var dx = inDir.Normalize();

                int frameX = dx switch
                {
                    var dir when dir.X > 0 => 2,
                    var dir when dir.X < 0 => 3,
                    var dir when dir.Y > 0 => 4,
                    var dir when dir.Y < 0 => 1,
                    _ => 0
                };

                return new Rectangle(frameX * 16, 0, 16, 16);
            }


            Vector2 ToWindowPosition(Vec2 pos)
            {
                var wv = CameraHelper.ToWindowPosition(pos, new Vec2(Window.X, Window.Y));
                return new Vector2(wv.X + renderpos.x, wv.Y + renderpos.y);
            }

            var zoom = 2;
            batch.End();
            batch.Begin(samplerState: SamplerState.PointWrap);
            _graphics.SetRenderTarget(null);
            batch.Draw(_renderTarget, new Rectangle(0, 0, _renderTarget.Width * zoom, _renderTarget.Height * zoom), Color.White);
        }
    }


}
