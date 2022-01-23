using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monomon.State
{
    public abstract class State<RenderArgs>
    {
        public abstract void Update(float time);
        public abstract void Render(RenderArgs param);
        public bool Completed { get; protected set; }
    }

    public record StateTransition<R>(State<R> state, Action onEnter, Action onExit);
    public class StateStack<RenderArgs>
    {
        private Stack<StateTransition<RenderArgs>> _states;

        public StateStack()
        {
            _states = new Stack<Monomon.State.StateTransition<RenderArgs>>();
        }

        public void Push(State<RenderArgs> s,Action onCompleted, Action? onEnter = null)
        {
            _states.Push(new StateTransition<RenderArgs>(s, onEnter ?? (() => { }), onCompleted));
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
            _states.Peek().onEnter();
        }
        
        public void Render(RenderArgs param)
        {
            try
            {
                foreach (var state in _states.Reverse())
                    state.state.Render(param); 
            }
            catch when (true)
            {
                throw;
            }
        }
    }
}
