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
    public class BattleReporter
    {
        private readonly SpriteBatch batch;
        private readonly StateStack<double> _stack;
        private Texture2D _sprites;
        private SpriteFont _font;
        private IINputHandler _input;
        private GraphicsDevice _gd;

        public List<string> Messages { get; set; }
        public BattleReporter(SpriteBatch batch, GraphicsDevice gd, State.StateStack<double> stack, IINputHandler input, SpriteFont font, Texture2D sprites)
        {
            _sprites = sprites;
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

        TimedState TimedMessage(string message)
        { 
            return new TimedState(new MessageScene(_gd,message,_font), 1000, _input);
        }

        ConfirmState ConfirmMessage(string message)
        { 
            return new ConfirmState(new MessageScene(_gd,message,_font), _input);
        }


        public void OnAttack(BattleMessage message, Mons.Mobmon _oponent, Action continueWith)
        {
            var attackInfoState = TimedMessage($"{message.attacker} choose {message.name}");
            var attackMessageState = TimedMessage($"{message.attacker} attacked {message.receiver} for {message.damage} points of damage!");

            var health = _oponent.Health;
            var hasFainted = false;
            var healthbarUpdateState = new TweenState((arg) => _oponent.Health = (float)(health - arg.lerp), () =>
            {
                _oponent.Health = health - message.damage;
                hasFainted = _oponent.Health <= 0;
            }, 0.0f, message.damage, 1.0f);


            _stack.Push(attackInfoState, () =>
            {
                //Remove ourself the attack info state
                _stack.Pop();
                _stack.Push(healthbarUpdateState, () =>
                {
                    //Remove ourself, the attack message state
                    _stack.Pop();
                    if (hasFainted)
                    {
                    }
                    else
                    {
                        _stack.Push(attackMessageState, () =>
                        {
                            _stack.Pop();
                            continueWith();
                        });
                    }
                });
            });
        }
    }
}