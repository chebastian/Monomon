﻿using Monomon.Input;
using Monomon.Views.Scenes;

namespace Monomon.State
{
    public class SceneState : State<double>
    {
        private SceneView _scene;
        private IINputHandler _input;

        public SceneState(SceneView view, IINputHandler input)
        {
            _scene = view;
            _input = input;
        }

        public override void Render(double param)
        {
            _scene.Draw(param);
        }

        public override void Update(float time)
        {
            _scene.Update((double)time);
            if(_input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.A))
            {
                Completed = true;
            }
        }
    }
}