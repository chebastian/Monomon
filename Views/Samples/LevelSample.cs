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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Samples
{
    public class LevelSample : SceneView
    {
        private IINputHandler input;
        private StateStack<double> stack;
        private Texture2D _tileSprites;
        private TileMap _map;
        private SerializedLevelData? _levelData;

        private Vec2 windowPos;

        public LevelSample(GraphicsDevice gd) : base(gd)
        {
        }

        public LevelSample(GraphicsDevice gd, IINputHandler input, StateStack<double> stack) : this(gd)
        {
            this.input = input;
            this.stack = stack;
            windowPos = new Vec2(0, 0);
        }

        public override void LoadScene(ContentManager content)
        {
            _map = new TileMap();
            _levelData = System.Text.Json.JsonSerializer.Deserialize<SerializedLevelData>(File.ReadAllText("./Levels/initLevel.json"));
            _map.CreateLevel(_levelData.VisibleTiles, _levelData.Tiles);


            _tileSprites = content.Load<Texture2D>("levelMap");

        }

        public override void Update(double time)
        {
            var dx = 0.0;
            dx = input.IsKeyDown(KeyName.Left) ? -1.0 : dx;
            dx = input.IsKeyDown(KeyName.Right) ? 1.0 : dx;

            var dy = 0.0;
            dy = input.IsKeyDown(KeyName.Up) ? -1.0 : dy;
            dy = input.IsKeyDown(KeyName.Down) ? 1.0 : dy;

            windowPos += (new Vec2((float)dx, (float)dy) * 200.0) * time;
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            var renderpos = (x: 200,y: 200);
            var window = (X: windowPos.X, Y: windowPos.Y, Width: 300, Height: 200);
            var Constants = (TileW: 16, TileH: 16, SpriteMapW: 27);
            static (Rectangle src, int x, int y) GetUvCoords(Rect rect, Rect src, Rect win)
            {
                return (new Rectangle((int)src.X, (int)src.Y, (int)src.Width, (int)src.Height), (int)rect.X, (int)rect.Y);

            }


            foreach (var tile in _map.GetTilesInside(new Rect(window.X,window.Y,window.Width,window.Height)))
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


            Vector2 ToWindowPosition(Vec2 pos)
            {
                var wv = CameraHelper.ToWindowPosition(pos, new Vec2(window.X, window.Y));
                return new Vector2(wv.X + renderpos.x, wv.Y + renderpos.y);
            }

            static bool IsVisible(Rect pos, Rect win)
            {
                return win.Intersects(pos);
            }

        }
    }
}
