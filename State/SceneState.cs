using MonoGameBase.Input;
using Monomon.Input;
using Monomon.Views.Scenes;

namespace Monomon.State
{
    public class SceneState : State<RenderParams>
    {
        protected SceneView _scene;
        protected IINputHandler _input;

        public SceneState(SceneView view, IINputHandler input)
        {
            _scene = view;
            _input = input;
        }

        public override void Render(RenderParams param)
        {
            _scene.Draw(param);
        }

        public override void Update(float time)
        {
            _scene.Update((double)time);
            if(_input.IsKeyPressed(KeyName.Quit))
            {
                Completed = true;
            }
        }
    }
}
