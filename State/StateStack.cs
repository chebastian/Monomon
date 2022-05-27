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

    //public record StateTransition<R>(State<R> state, Action onEnter, Action onExit);
    public class StateTransition<R>
    {
        public readonly State<R> state;
        public readonly Action onEnter;
        public readonly Action onExit;
        public bool initialized = false;

        public StateTransition(State<R> state, Action onEnter, Action onExit)
        {
            this.state = state;
            this.onEnter = onEnter;
            this.onExit = onExit;
        }
    }

    public class StateStack<RenderArgs>
    {
        private Stack<StateTransition<RenderArgs>> _states;
        private List<StateTransition<RenderArgs>> _sequence;

        public StateStack()
        {
            _states = new Stack<Monomon.State.StateTransition<RenderArgs>>();
            _sequence = new List<StateTransition<RenderArgs>>();
        }

        public void Push(State<RenderArgs> s,Action onCompleted, Action? onEnter = null)
        {
            _states.Push(new StateTransition<RenderArgs>(s, onEnter ?? (() => { }), onCompleted));
        }
        
        public void BeginStateSequence()
        {
            _sequence = new List<StateTransition<RenderArgs>>();
        }

        public void EndStateSecence(Action end)
        {
            _sequence.Reverse();
            var lastState = _sequence.First();
            Push(lastState.state, () => {
                lastState.onExit();
                Pop();
                end();
            },_sequence.First().onEnter);

            foreach (var state in _sequence.Skip(1))
                Push(state.state, () => { state.onExit(); Pop(); }, state.onEnter);

            _sequence.Clear();
        }

        public void AddState(State<RenderArgs> state, Action? onExit = null, Action? onEnter = null)
        {
            _sequence.Add(new StateTransition<RenderArgs>(state, onEnter ?? new Action(() => { }), onExit ?? new Action(() => { })));
        }

        public void Update(float time)
        {
            var top = _states.Peek();
            if (!top.initialized)
            {
                top.onEnter();
                top.initialized = true;
            }

            top.state.Update(time);
            if(top.state.Completed)
            {
                top.onExit();
            }
        }

        public void Pop()
        {
            _states.Pop();
            //_states.Peek().onEnter();
        }
        
        public void Render(RenderArgs param)
        {
            try
            {
                foreach (var state in _states.Reverse())
                {
                    state.state.Render(param); 
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
