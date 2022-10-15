using MonoGameBase.Input;
using Monomon.Input;
using Monomon.Views.Scenes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monomon.State
{
    public class TimedState : SceneState
    {
        public TimedState(SceneView view,int timeoutMs, IINputHandler input) : base(view, input)
        {
            _skipped = false;
            timeout = timeoutMs;
        }

        private bool _skipped;
        private float timeout;

        public override void Render(RenderParams param)
        {
            base.Render(param);
        }

        public override void Update(float time)
        {
            base.Update(time);
            timeout -= time*1000.0f;
            if (timeout <= 0)
                Completed = true;

            if(_input.IsKeyPressed(KeyName.Select) && !_skipped)
            {
                if(_scene is MessageScene msg)
                {
                    msg.Update(1.0f);
                    timeout -= timeout * 0.7f;
                }
            }
        }
    }

    public class ConfirmState : SceneState
    {
        private bool _skipped;

        public ConfirmState(SceneView view,IINputHandler input) : base(view, input)
        {
            _skipped = false;
        }

        public override void Render(RenderParams param)
        {
            base.Render(param);
        }

        public override void Update(float time)
        {
            if (_input.IsKeyPressed(KeyName.Select))
            {
                if (_scene is MessageScene msg)
                {
                    if (_skipped || msg.IsCompleted)
                        Completed = true;

                    msg.Update(1.0f);
                    _skipped = true;
                }
                else if (_scene is ChoiceScene choice)
                {
                    choice.List.Select();
                    Completed = true;
                }
                else
                { 
                    Completed = true; 
                }
            }

            if(_scene is ChoiceScene choice2)
            {
                if (_input.IsKeyPressed(KeyName.Down))
                    choice2.List.SelectNext();
                else if (_input.IsKeyPressed(KeyName.Up))
                    choice2.List.SelectPrevious();
            }

            _scene.Update((float)time);
        }
    }

    public class TimeoutState : SceneState
    {
        private readonly IINputHandler input;
        private readonly Action onCancel;
        private CancellationTokenSource cancelation;
        private CancellationToken token;
        private bool signalDone = false;

        public TimeoutState(SceneView view,int timeoutMs, IINputHandler input, Action onCancel) : base(view, input)
        {
            cancelation = new CancellationTokenSource();
            token = cancelation.Token;
            Task.Run(async () =>
            {
                await Task.Delay(timeoutMs);
                if (!token.IsCancellationRequested)
                    signalDone = true;

            }, token);
            this.input = input;
            this.onCancel = onCancel;
        }

        public override void Update(float time)
        { 
            if(input.IsKeyPressed(KeyName.Back))
            {
                cancelation.Cancel();
                onCancel();
            }
            if (signalDone)
                Completed = true; 
        }

        public override void Render(RenderParams param)
        {
            base.Render(param);
        }
    }
}
