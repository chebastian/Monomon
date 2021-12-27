using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Battle;
using Monomon.Input;
using Monomon.State;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;

namespace Monomon
{
    internal class BattleReporter : IBattleReporter
    {
        private readonly SpriteBatch batch;
        private readonly StateStack<double> _stack;
        private SpriteFont _font;
        private IINputHandler _input;
        private GraphicsDevice _gd;

        public List<string> Messages { get; set; }
        public BattleReporter(SpriteBatch batch, GraphicsDevice gd, State.StateStack<double> stack,IINputHandler input, SpriteFont font)
        {
            _font = font; 
            _input = input;
            _gd = gd;
            if (batch is null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            this.batch = batch;
            _stack = stack;
            Messages = new List<string>();
        }

        public void OnAttack(BattleMessage message)
        {
            var str = $"{message.attacker} attacked {message.receiver} for {message.damage} points of damage!";
            var view = new MessageScene(_gd, str, _font);
            _stack.Push(new TimeoutState(view,1000,_input, () => { _stack.Pop(); }),() => { _stack.Pop(); });
        }
    }
}