using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            _sceneList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Battle test",x => {
                    SwapScene(new BattleSample(GraphicsDevice));
                }),
                new UIItem<string>("Message",x => {
                    SwapScene(new MessageScene(GraphicsDevice,"First message"));
                }),
                new UIItem<string>("Timed message 2",x => {

                    var scene =new MessageScene(GraphicsDevice,"Completes in for 2s...");
                    scene.LoadScene(content);
                    _stack.Push(new TimeoutState(scene,2000,_input,
                    onCancel: () => {
                        _stack.Pop();
                    }),
                    onCompleted: () => 
                    {
                        _stack.Pop();
                        SwapScene(new MessageScene(GraphicsDevice,"Completed!")); }
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
            newScene.LoadScene(_content);
            _stack.Push(new SceneState(newScene, _input), () =>
            {
                _stack.Pop();
            });
        }

        public override void Update(double time)
        {
            if (_input.IsKeyPressed(Keys.Down))
                _sceneList.SelectNext();
            if (_input.IsKeyPressed(Keys.Up))
                _sceneList.SelectPrevious();
            if (_input.IsKeyPressed(Keys.Space))
                _sceneList.Select();
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            ListView.DrawUIList(_sceneList, new Vector2(10, 10),batch,font);
        }
    }
}
