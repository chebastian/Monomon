using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monomon.Input;
using Monomon.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monomon
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private BufferInputHandler _input;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private UIList<string> list;
        private UIList<string> fightList;
        private UIList<string> itemList;
        private UIList<string> _currentList;
        private string _selection;
        private Color _clearColor;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _input = new Monomon.Input.BufferInputHandler();
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("File");
            _clearColor = Color.SkyBlue;

            list = new UIList<string>(new List<string>() {
                "Fight",
                "Item",
                "Mon",
                "Run"
            }, x => { }, OnItemChanged);

            fightList = new UIList<string>(new List<string>() { 
                "Tackle",
                "Growl",
            }, x => { }, OnFightItemSelected);

            itemList = new UIList<string>(new List<string>() { 
                "potion",
                "mana-potion",
                "ball",
            }, x => { }, OnFightItemSelected);

            _currentList = list;

            list.SelectedItem = "Fourth...";

            base.Initialize();
        }

        private void OnFightItemSelected(string obj)
        {
        }

        public void DrawUIList<T>(UIList<T> list, Vector2 pos) where T : IEquatable<T>
        {
            var y = pos.Y;
            foreach(var item in list.Items.Select((x,i) => (x,i)))
            {
                var c = list.SelectedItem.Equals(item.x) ? Color.Red : Color.White;
                c = item.x.Equals(_selection) ? Color.Green : c;

                _spriteBatch.DrawString(font, item.x.ToString(), new Vector2(pos.X, y), c);
                y += 20;
            }
        }

        private void OnItemChanged(string obj)
        {
            _selection = obj;
            _clearColor = obj switch
            { 
                "Fight" => Color.HotPink,
                "Item" => Color.Blue,
                "Mon" => Color.Yellow,
                "Run" => Color.Black,
                _ => Color.SkyBlue
            };

            if (_selection == "Fight")
            {
                _currentList = fightList;
            }
            else if(_selection == "Item")
            {
                _currentList = itemList;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _input.Update(gameTime);
            var pad = GamePad.GetState(PlayerIndex.One);

            if (_input.IsKeyPressed(Keys.Down))
                _currentList.SelectNext();
            if (_input.IsKeyPressed(Keys.Up))
                _currentList.SelectPrevious();
            if(_input.IsKeyPressed(Keys.A))
            {
                _currentList = list;
            }

            if(_input.IsKeyPressed(Keys.Space))
            {
                _currentList.Select();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_clearColor);

            _spriteBatch.Begin();
            DrawUIList(_currentList, new Vector2(30, 10));
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
