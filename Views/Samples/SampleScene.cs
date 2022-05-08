using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameBase.Input;
using Monomon.Input;
using Monomon.State;
using Monomon.UI;
using Monomon.Views.Gui;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Samples
{
    public class SampleScene : SceneView
    {
        private SpriteFont font;
        private UIList<string> _sceneList;
        private ContentManager _content;
        private IINputHandler _input;
        private StateStack<double> _stack;

        public GraphicsDevice GraphicsDevice { get; }

        public SampleScene(GraphicsDevice gd, StateStack<double> stack, IINputHandler input)
            : base(gd)

        {
            _input = input;
            _stack = stack;
            GraphicsDevice = gd;
        }

        public override void LoadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("File");
            var sprites = content.Load<Texture2D>("spritemap");
            _sceneList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Level test",x => {
                    SwapScene(new LevelSample(GraphicsDevice,_input,_stack));
                }),
                new UIItem<string>("Battle test",x => {
                    SwapScene(new BattleSample(GraphicsDevice,_input,_stack));
                }),
                new UIItem<string>("Tween",x => {
                    SwapScene(new TweenSamples(GraphicsDevice));
                }),
                new UIItem<string>("Message",x => {
                    SwapScene(new MessageScene(GraphicsDevice,"First message",font,sprites));
                }),
                new UIItem<string>("Long Message",x => {
                    SwapScene(new MessageScene(GraphicsDevice,"First message aaa aa a aaa a aa aaaa aaa aaaa a aa aaa a aaaaaaa aaaaaaaaaaaaa aaa aaaaaaaaaaaaaaaaaaaaa aaa",font,sprites));
                }),
                new UIItem<string>("Long single worded message",x => {
                    SwapScene(new MessageScene(GraphicsDevice,"first message aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",font,sprites));
                }),
                new UIItem<string>("Timed message 2",x => {

                    var scene =new MessageScene(GraphicsDevice,"Completes in for 2s...",font,sprites);
                    scene.LoadScene(content);
                    _stack.Push(new TimeoutState(scene,2000,_input,
                    onCancel: () => {
                        _stack.Pop();
                    }),
                    onCompleted: () => 
                    {
                        _stack.Pop();
                        SwapScene(new MessageScene(GraphicsDevice,"Completed!",font,sprites)); }
                    );
            ;
                }),
                new UIItem<string>("BattleCard sample",x => {
                    SwapScene(new BattleCardSample(GraphicsDevice));
                }),
                new UIItem<string>("Empty",x => {
                    SwapScene(new EmptyScene(GraphicsDevice));
                }),
            }, x => { }, x => { });

            _content = content;
        }

        private void SwapScene(SceneView newScene)
        {
            _stack.Push(new SceneState(newScene, _input), () =>
            {
                _stack.Pop();
            },() => { });
            newScene.LoadScene(_content);
        }

        public override void Update(double time)
        {
            if (_input.IsKeyPressed(KeyName.Down))
                _sceneList.SelectNext();
            if (_input.IsKeyPressed(KeyName.Up))
                _sceneList.SelectPrevious();
            if (_input.IsKeyPressed(KeyName.Select))
                _sceneList.Select();
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            ListView.DrawUIList(_sceneList, new Vector2(10, 10),batch,font);
        }
    }
}
