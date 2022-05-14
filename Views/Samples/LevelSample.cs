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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Samples
{
    public class Player
    {
        public Player()
        {
            Pos = new Vec2();
            Vel = new Vec2();
        }
        public Vec2 Pos { get; set; }
        public Vec2 Vel { get; set; }
    }

    public class LevelSample : SceneView
    {
        private IINputHandler input;
        private StateStack<double> stack;
        private Texture2D _tileSprites;
        private Texture2D _playerSprites;
        private TileMap _map;
        private SerializedLevelData? _levelData;

        private Player _player;

        private Vec2 windowPos;

        public LevelSample(GraphicsDevice gd) : base(gd)
        {
        }

        public LevelSample(GraphicsDevice gd, IINputHandler input, StateStack<double> stack) : this(gd)
        {
            this.input = input;
            this.stack = stack;
            _player = new Player();
            _player.Pos = new Vec2(128, 100);
            windowPos = new Vec2(0, 0);
        }

        public override void LoadScene(ContentManager content)
        {
            _map = new TileMap();
            _levelData = System.Text.Json.JsonSerializer.Deserialize<SerializedLevelData>(File.ReadAllText("./Levels/grass.json"));
            _map.CreateLevel(_levelData.VisibleTiles, _levelData.Tiles);


            _tileSprites = content.Load<Texture2D>("levelMap");
            _playerSprites = content.Load<Texture2D>("player");

        }

        public override void Update(double time)
        {
            var dx = 0.0;
            dx = input.IsKeyDown(KeyName.Left) ? -1.0 : dx;
            dx = input.IsKeyDown(KeyName.Right) ? 1.0 : dx;

            var dy = 0.0;
            dy = input.IsKeyDown(KeyName.Up) ? -1.0 : dy;
            dy = input.IsKeyDown(KeyName.Down) ? 1.0 : dy;

            var winPos = _player.Pos + new Vec2(-(Window.Width * 0.5f), -(Window.Height * 0.5f));
            Window.X = winPos.X;
            Window.Y = winPos.Y;

            var vel = new Vec2((float)((dx * 200.0) * time), (float)((dy * 200.0) * time));
            var playerRect = new Rect(_player.Pos.X, _player.Pos.Y, 16, 16);

            var tiles = _map.GetTilesInside(playerRect.MinkowskiSum(new Rect(0,0,16,16))).Where(x => x.type == TileType.Wall).Select(x => x.rect).ToList();
            var info = CollisionHelper.HandleCollision(_map,playerRect, vel, tiles);
            if (info.Collisions.Any())
                _player.Pos += info.ResultingVelocity;
            else
                _player.Pos += vel;

            _player.Vel = vel;
        }

        public Rect Window = new Rect(0, 0, 400, 200);
        protected override void OnDraw(SpriteBatch batch)
        {
            var renderpos = (x: 200, y: 0);
            var Constants = (TileW: 16, TileH: 16, SpriteMapW: 27);
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
                SourceForDir(_player.Vel),
                Color.White
                );

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

                return new Rectangle(frameX*16, 0, 16, 16);
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

        }
    }


}
