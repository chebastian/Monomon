﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monomon.Battle;
using Monomon.Input;
using Monomon.Mons;
using Monomon.UI;
using Monomon.ViewModels;
using Monomon.Views;
using Monomon.Views.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monomon
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private BufferInputHandler _input;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private UIList<string> _sceneList;
        private string _selection;
        private SceneView _currentScene;

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

            _sceneList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Battle test",x => {
                    SwapScene(new BattleSample(GraphicsDevice));
                }),
                new UIItem<string>("BattleCard sample",x => {
                    SwapScene(new BattleCardSample(GraphicsDevice));
                }),
                new UIItem<string>("Empty",x => {
                    SwapScene(new EmptyScene(GraphicsDevice));
                }),
            }, x => { }, x => { });

            //_currentScene = new BattleCardSample(GraphicsDevice);
            _currentScene = new EmptyScene(GraphicsDevice);

            base.Initialize();
        }

        void SwapScene(SceneView scene)
        {
            _currentScene = scene;
            _currentScene.LoadScene(Content);
        }

        public void DrawUIList<T>(UIList<T> list, Vector2 pos) where T : IEquatable<T>
        {
            var y = pos.Y;
            foreach (var item in list.Items.Select((x, i) => (x, i)))
            {
                var c = item.x.Selected ? Color.Red : Color.White;
                c = item.x.Item.Equals(_selection) ? Color.Green : c;

                _spriteBatch.DrawString(font, item.x.Item.ToString(), new Vector2(pos.X, y), c);
                y += 20;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _currentScene.LoadScene(Content);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _input.Update(gameTime);

            if (_input.IsKeyPressed(Keys.Down))
                _sceneList.SelectNext();
            if (_input.IsKeyPressed(Keys.Up))
                _sceneList.SelectPrevious();
            if (_input.IsKeyPressed(Keys.Right))
                _sceneList.Select();

            _currentScene.Update(gameTime);


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            DrawUIList(_sceneList, new Vector2(0, 0));
            _currentScene.Draw(gameTime);


            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }
}
