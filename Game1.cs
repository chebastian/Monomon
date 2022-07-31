using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monomon.Effects;
using Monomon.Input;
using Monomon.State;
using Monomon.UI;
using Monomon.Views.Gui;
using Monomon.Views.Samples;
using Monomon.Views.Scenes;
using System.Collections.Generic;

namespace Monomon
{
#nullable disable

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private BufferInputHandler _input;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private UIList<string> _sceneList;
        private SceneView _currentScene;
        private StateStack<double> _stateStack;
        private PaletteEffect _paletteEffect;
        private FadeEffect _fadeImpl;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _stateStack = new StateStack<double>();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _input = new Monomon.Input.BufferInputHandler();
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("File");
            _paletteEffect = new PaletteEffect(Content,Content.Load<Texture2D>("paletteMini"));
            _fadeImpl = new FadeEffect(Content.Load<Effect>("Fade"),
                                       Content.Load<Texture2D>("pixelCircle"),
                                       Content.Load<Texture2D>("paletteMini"),
                                       _paletteEffect.CurrentPalette);

            _sceneList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Battle test",x => {
                }),
                new UIItem<string>("BattleCard sample",x => {
                    SwapScene(new BattleCardSample(GraphicsDevice,Content));
                }),
                new UIItem<string>("Empty",x => {
                    SwapScene(new EmptyScene(GraphicsDevice,Content));
                }),
            }, x => { }, x => { });

            //_currentScene = new BattleCardSample(GraphicsDevice);
            _currentScene = new SampleScene(GraphicsDevice, _stateStack, _input, Content,_paletteEffect,_fadeImpl);

            _stateStack.Push(new SceneState(_currentScene, _input),
                () => { });
            base.Initialize();
        }

        void SwapScene(SceneView scene)
        {
            _currentScene = scene;
            _currentScene.LoadScene(Content);
            _stateStack.Push(new SceneState(scene, _input), () =>
            {
                _stateStack.Push(new SceneState(new EmptyScene(GraphicsDevice, Content), _input), () => { });
            });
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ListView.PanelTexture = Content.Load<Texture2D>("spritemap");

            _currentScene.LoadScene(Content);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            _input.Update();
            //if(_input.IsKeyPressed(MonoGameBase.Input.KeyName.))
            //{
            //    _graphics.ToggleFullScreen();
            //}
            //if (_input.IsKeyPressed(Keys.Down))
            //    _sceneList.SelectNext();
            //if (_input.IsKeyPressed(Keys.Up))
            //    _sceneList.SelectPrevious();
            //if (_input.IsKeyPressed(Keys.Right))
            //    _sceneList.Select();

            //_currentScene.Update(gameTime.ElapsedGameTime.TotalSeconds);

            _stateStack.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _paletteEffect.EffectBegin(_spriteBatch);

            _stateStack.Render(gameTime.ElapsedGameTime.TotalSeconds);
            _spriteBatch.End();
            _fadeImpl.Draw(_spriteBatch);
            //batch.End();
            //batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null, _effect);
            //_paletteEffect.EffectBegin(_spriteBatch); //since we stop to draw the fade reenable palette effect


            //_spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }
}
