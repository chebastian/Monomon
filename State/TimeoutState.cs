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
            timeout = timeoutMs;
        }

        private float timeout;

        public override void Render(double param)
        {
            base.Render(param);
        }

        public override void Update(float time)
        {
            base.Update(time);
            timeout -= time*1000.0f;
            if (timeout <= 0)
                Completed = true;
        }
    }

    public class ConfirmState : SceneState
    {
        public ConfirmState(SceneView view,IINputHandler input) : base(view, input)
        {
        }

        public override void Render(double param)
        {
            base.Render(param);
        }

        public override void Update(float time)
        {
            if (_input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
                Completed = true;
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
            if(input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.A))
            {
                cancelation.Cancel();
                onCancel();
            }
            if (signalDone)
                Completed = true; 
        }

        public override void Render(double param)
        {
            base.Render(param);
        }
    }
}
