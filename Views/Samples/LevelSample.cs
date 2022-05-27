using Common.Game.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonogameBase;
using MonogameBase.Camera;
using MonoGameBase.Collision;
using MonoGameBase.Input;
using MonoGameBase.Level;
using Monomon.State;
using Monomon.Views.Scenes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Data
{
    public static class Constants
    {
        public const int TileH = 16;
        public const int TileW = 16;
        public const int SpriteMapW = 27;
    }
}

namespace Monomon.Views.Samples
{
    using Monomon.Data;

    public class Player
    {
        public Player()
        {
            Pos = new Vec2();
            Vel = new Vec2();
            Target = new Vec2();
            Dist = 0.0f;
        }
        public Vec2 Pos { get; set; }
        public Vec2 Vel { get; set; }
        public Vec2 Target { get; set; }
        public float Dist { get; set; }
        public Vec2 OgPos { get; private set; }

        public Vec2 Center
        {
            get
            {
                return Pos + new Vec2(Constants.TileW/2,Constants.TileH/2);
            }
        }

        public void SetTarget(Vec2 target)
        {
            var playerCenter = Pos + new Vec2(8, 8);
            var tileBegin = ((int)(playerCenter.X / 16)*16,((int)playerCenter.Y / 16)*16); 

            if (Dist <= 0.0f && (target.X != 0 || target.Y != 0))
            {
                Debug.WriteLine("hit");
                Target = target + new Vec2(tileBegin.Item1,tileBegin.Item2);
                Dist = 0.0f;
            }
        }

        public void Advance(float d, float t)
        {
            if (Target.X == 0 && Target.Y == 0)
                return;

            Dist += d;
            Dist = System.MathF.Min(Dist, 1.0f);
            if (Dist >= 1)
            {
                Pos = Pos.Quad(Target, Dist);
                Target = new Vec2(0, 0);
                Dist = 0.0f;
            }

            if (Target.X == 0 && Target.Y == 0)
                return;

            if (Dist <= 1)
                Pos = Pos.Lerp(Target, Dist);
        }

        internal void WalkInDirection(Vec2 target)
        {
            Target = target;//new Vec2(Center.X + dx * Constants.TileW, Center.Y + dy* Constants.TileH);
            Dist = 1.0f;
            OgPos = Pos;
        }

        internal void Update(float dt)
        {
            if(Dist > 0.0)
            {
                Dist -= dt * 4.0f;
                Dist = MathF.Max(0.0f, Dist);
                Pos = OgPos.Lerp(Target, 1.0f - Dist); 
            }
        }
    }

    public class LevelSample : SceneView
    {
        private IINputHandler input;
        private StateStack<double> stack;
        private Texture2D _tileSprites;
        private Texture2D _playerSprites;
        private Effect paletteEffect;
        private RenderTarget2D _renderTarget;
        private TileMap _map;
        private SerializedLevelData? _levelData;

        private Player _player;

        private Vec2 windowPos;

        public LevelSample(GraphicsDevice gd) : base(gd)
        {
        }

        public Vec2 ToPositionOnGrid(Vec2 pos)
        {
            return new Vec2(((int)(pos.X / 16)) * 16,((int)( pos.Y / 16))*16);
        }

        public LevelSample(GraphicsDevice gd, IINputHandler input, StateStack<double> stack) : this(gd)
        {
            this.input = input;
            this.stack = stack;
            _player = new Player();
            _player.Pos = new Vec2(128, 128);
            windowPos = new Vec2(0, 0);
        }

        public override void LoadScene(ContentManager content)
        {
            _renderTarget = new RenderTarget2D(_graphics, 160, 144);
            _map = new TileMap();
            _levelData = System.Text.Json.JsonSerializer.Deserialize<SerializedLevelData>(File.ReadAllText("./Levels/grass.json"));
            _map.CreateLevel(_levelData.VisibleTiles, _levelData.Tiles);


            _tileSprites = content.Load<Texture2D>("levelMap");
            _playerSprites = content.Load<Texture2D>("player");

            paletteEffect = content.Load<Effect>("Indexed");

            //Init effect
            {
                _palette = content.Load<Texture2D>("paletteMini");
                paletteEffect.Parameters["time"].SetValue(0.0f);
                paletteEffect.Parameters["swap"].SetValue(1.0f);
                paletteEffect.Parameters["palette"].SetValue(_palette);
            }
        }

        public override void Update(double time)
        {
            _dx = input.GetX();
            _dy = input.GetY();

            var winPos = _player.Pos + new Vec2(-(Window.Width * 0.5f), -(Window.Height * 0.5f));
            Window.X = winPos.X;
            Window.Y = winPos.Y;


            var vel = new Vec2(_dx * 16, _dy * 16) * time;
            var speed = 16.0;

            var playerRect = new Rect(_player.Pos.X, _player.Pos.Y, 16, 16);

            if (_dx != 0 || _dy != 0)
            {
                var bothPressed = (_dx != 00 && _dy != 0.0);
                _dy = bothPressed ? 0 : _dy;

                if(_player.Dist == 0.0f)
                    _player.WalkInDirection(ToPositionOnGrid(_player.Center + new Vec2(_dx * Constants.TileW, _dy * Constants.TileH)));
            }
            //if (dx == 0 && dy == 0)
            //{
            //    var oldVel = _player.Vel.Normalize();
            //    _player.Advance((float)((time * speed / 16.0f)),(float)time);
            //    _player.SetTarget(new Vec2(System.MathF.Sign(oldVel.X) * 16, System.MathF.Sign(oldVel.Y) * 16));
            //}

            _player.Update((float)time);

            var tiles = _map.GetTilesInside(playerRect.MinkowskiSum(new Rect(0, 0, 16, 16))).Where(x => x.type == TileType.Wall).Select(x => x.rect).ToList();
            var info = CollisionHelper.HandleCollision(_map, playerRect, vel, tiles);

            if (info.Collisions.Any())
                _player.Pos += info.ResultingVelocity;
            else
                _player.Pos += vel;

            _player.Vel = vel;
        }

        public Rect Window = new Rect(0, 0, 160, 144);
        private Texture2D _palette;
        private int _dx;
        private int _dy;

        protected override void OnDraw(SpriteBatch batch)
        {
            batch.End();

            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(Color.White);
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, paletteEffect);

            var renderpos = (x: 0, y: 0);
            static (Rectangle src, int x, int y) GetUvCoords(Rect rect, Rect src, Rect win)
            {
                return (new Rectangle((int)src.X, (int)src.Y, (int)src.Width, (int)src.Height), (int)rect.X, (int)rect.Y);

            }

            foreach (var tile in _map.GetTilesInside(new Rect(Window.X, Window.Y, Window.Width, Window.Height)))
            {
                //_spriteBatch.Draw(_floorTexture, new Vector2(winPos.x  + (tile.rect.X - .X), winPos.y + (tile.rect.Y - window.Y)), new Rectangle((int)(tileindex.X * TileW), (int)tileindex.Y * TileH, Constants.TileW, Constants.TileH), Color.White);
                var tilepos = (x: tile.rect.X / (Constants.TileW), y: tile.rect.Y / (Constants.TileH));
                var visual = _map.GetVisibleTileAt((int)tilepos.x, (int)tilepos.y);
                var tileindex = MapHelper.To2DIndex(visual, Constants.SpriteMapW);


                var tileSrc = (X: (int)tileindex.X * (Constants.TileW), Y: (int)tileindex.Y * (Constants.TileH));

                var renderPos = ToWindowPosition(new Vec2(tile.rect.X, tile.rect.Y));

                if (visual != (uint)TileType.None)
                {
                    _spriteBatch.Draw(_tileSprites,
                        renderPos,
                        new Rectangle(tileSrc.X, tileSrc.Y, Constants.TileW, Constants.TileH),
                        Color.White);
                }
            }

            var playerCamPos = ToWindowPosition(_player.Pos);
            _spriteBatch.Draw(_playerSprites,
                new Rectangle((int)playerCamPos.X, (int)playerCamPos.Y, 16, 16),
                SourceForDir(_player.Target - _player.OgPos),
                Color.White
                );

            MarkTile(ToPositionOnGrid( _player.Center));
            MarkTile(ToPositionOnGrid( _player.Center + new Vec2(_dx*Constants.TileW,_dy*Constants.TileH)));

            void MarkTile(Vec2 gridPos)
            {
                var pcenter = ToWindowPosition(gridPos);
                _spriteBatch.Draw(_playerSprites,
                    new Rectangle((int)pcenter.X, (int)pcenter.Y, 2, 2),
                    new Rectangle(0,0,1,1),
                    Color.Black 
                    );

                _spriteBatch.Draw(_playerSprites,
                    new Rectangle((int)pcenter.X + Constants.TileW, (int)pcenter.Y, 2, 2),
                    new Rectangle(0,0,1,1),
                    Color.Black 
                    );

                _spriteBatch.Draw(_playerSprites,
                    new Rectangle((int)pcenter.X + Constants.TileW, (int)pcenter.Y + Constants.TileH, 2, 2),
                    new Rectangle(0,0,1,1),
                    Color.Black 
                    );

                _spriteBatch.Draw(_playerSprites,
                    new Rectangle((int)pcenter.X, (int)pcenter.Y + Constants.TileH, 2, 2),
                    new Rectangle(0,0,1,1),
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

            static bool IsVisible(Rect pos, Rect win)
            {
                return win.Intersects(pos);
            }

            var zoom = 2;
            batch.End();
            batch.Begin(samplerState: SamplerState.PointWrap);
            _graphics.SetRenderTarget(null);
            batch.Draw(_renderTarget, new Rectangle(0, 0, _renderTarget.Width * zoom, _renderTarget.Height * zoom), Color.White);
            batch.End();
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, paletteEffect);

        }
    }


}
