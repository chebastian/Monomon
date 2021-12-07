using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Input;
using Monomon.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monomon.State
{
    public abstract class State<RenderArgs>
    {
        public abstract void Update(float time);
        public abstract void Render(RenderArgs param);
        public bool Completed { get; protected set; }
    }

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

    public class TimeoutState : SceneState
    {
        private readonly IINputHandler input;
        private readonly Action onCancel;
        private CancellationTokenSource cancelation;
        private CancellationToken token;

        public TimeoutState(SceneView view,int timeoutMs, IINputHandler input, Action onCancel) : base(view, input)
        {
            cancelation = new CancellationTokenSource();
            token = cancelation.Token;
            Task.Run(async () =>
            {
                await Task.Delay(timeoutMs);
                if(!token.IsCancellationRequested)
                    Completed = true;

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
        }

        public override void Render(double param)
        {
            base.Render(param);
        }
    }

    public class StateStack<RenderArgs>
    {
        private Stack<(State<RenderArgs> state, Action onExit)> _states;

        public StateStack()
        {
            _states = new Stack<(State<RenderArgs>, Action)>();
        }

        public void Push(State<RenderArgs> s,Action onCompleted)
        {
            _states.Push((s,onCompleted));
        }

        public void Update(float time)
        {
            var top = _states.Peek();
            top.state.Update(time);
            if(top.state.Completed)
            {
                top.onExit();
            }
        }

        public void Pop()
        {
            _states.Pop();
        }
        
        public void Render(RenderArgs param)
        {
            foreach (var state in _states)
                state.state.Render(param); 
        }
    }
}
