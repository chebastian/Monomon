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
            try
            {
                foreach (var state in _states)
                    state.state.Render(param); 
            }
            catch when (true)
            {
                throw;
            }
        }
    }
}
