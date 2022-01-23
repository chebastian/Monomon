using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Battle;
using Monomon.Input;
using Monomon.State;
using Monomon.ViewModels;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monomon
{
    public enum Sounds
    {
        Attack_Tackle,
        TakeDamage,
        XpUP,
    }

    public class BattleReporter
    {
        private readonly SpriteBatch batch;
        private readonly StateStack<double> _stack;
        private Action<Sounds> _soundCallback;
        private Texture2D _sprites;
        private SpriteFont _font;
        private IINputHandler _input;
        private GraphicsDevice _gd;
        private List<StateTransition<double>> _states;

        public List<string> Messages { get; set; }
        public BattleReporter(SpriteBatch batch, GraphicsDevice gd, State.StateStack<double> stack, IINputHandler input, SpriteFont font, Texture2D sprites, Action<Sounds> soundCallback)
        {
            _soundCallback = soundCallback;
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
            return new TimedState(new MessageScene(_gd, message, _font, _sprites), 2500, _input);
        }

        ConfirmState ConfirmMessage(string message)
        {
            return new ConfirmState(new MessageScene(_gd, message, _font, _sprites, true), _input);
        }

        public void OnAttack(BattleMessage message, Mons.Mobmon attacker, Mons.Mobmon _oponent, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard)
        {
            //oldAttack(message,attacker,_oponent,continueWith,attackerCard,oponentCard);

            var attackInfoState = TimedMessage($"{message.attacker} used {message.name}");

            BeginStateSequence();
            AddState(attackInfoState);

            var ey = oponentCard.PortraitOffsetY;
            var hitAnim = new TweenState((arg) =>
            {
                oponentCard.PortraitOffsetY = (int)(ey + Math.Sin(3.14 * arg.lerp) * 20);
            }, () =>
            {
            }, 0, 1, .3f);

            var y = attackerCard.PortraitOffsetY;
            var attackAnimation = new TweenState((arg) =>
            {
                attackerCard.PortraitOffsetY = (int)(y + Math.Sin(3.14 * arg.lerp) * -40);
            }, () =>
            {
                _soundCallback(Sounds.Attack_Tackle);
            }, 0, 1, .3f);

            AddState(attackAnimation);
            AddState(hitAnim, () => { });

            var health = _oponent.Health;
            var hasFainted = false;
            var healthbarUpdateState = new TweenState((arg) => _oponent.Health = (float)(health - arg.lerp), () =>
            {
                _oponent.Health = health - message.damage;
                hasFainted = _oponent.Health <= 0;
            }, 0.0f, Math.Min(message.damage, health), 1.0f, EasingFunc.EaseOutCube);

            AddState(healthbarUpdateState, () => { 
                _oponent.Health = health - message.damage;
            });

            if(health - message.damage <= 0)
            {
                var offset = 0;
                var dropPoirtrait = new TweenState((arg) => { oponentCard.PoirtrateAnimDelta = ((int)(offset + arg.lerp)); oponentCard.Dying = true; }, () =>
               {
               }, 0.0f, oponentCard.PortraitSrc.Height, 0.5f, EasingFunc.EaseInBack);

                AddState(dropPoirtrait,null, () => _soundCallback(Sounds.TakeDamage));
                AddState(ConfirmMessage($"{_oponent.Name} has fainted"));
                AddState(ConfirmMessage("XP Gained"));
                var xp = attacker.Xp;
                var xpUpdate = new TweenState((arg) => attacker.Xp = (float)(xp + arg.lerp), () =>
                {
                    attacker.Xp = xp + 20;
                }, 0.0f, 20, 1.0f, EasingFunc.EaseOutCube);

                AddState(xpUpdate,null,() => _soundCallback(Sounds.XpUP));
            }

            EndStateSecence(() => {
                if(!hasFainted) //continue will swap to next round, if we dont we will prompt for which turn to go next
                    continueWith();
            });
        }

        private void BeginStateSequence()
        {
            _states = new List<StateTransition<double>>();
        }

        private void EndStateSecence(Action end)
        {
            _states.Reverse();
            _stack.Push(_states.First().state, () => {
                _states.First().onExit();
                _stack.Pop();
                end();
            },_states.First().onEnter);
            foreach (var state in _states.Skip(1))
                _stack.Push(state.state, () => { state.onExit(); _stack.Pop(); }, state.onEnter);
        }

        private void AddState(State<double> state, Action? onExit = null, Action? onEnter = null)
        {
            _states.Add(new StateTransition<double>(state, onEnter ?? new Action(() => { }), onExit ?? new Action(() => { })));
        }

        public class ConditionalState : State<double>
        {
            private bool? _condition;

            public ConditionalState(State<double> whenTrue, State<double> whenFalse, Func<bool> condition)
            {
                TrueState = whenTrue;
                FalseState = whenFalse;
                Condition = condition;
                _condition = null;
            }

            public State<double> TrueState { get; }
            public State<double> FalseState { get; }
            public Func<bool> Condition { get; }

            private bool IsTrue()
            {
                if(_condition == null)
                {
                    _condition = Condition();
                }

                return (bool)_condition;
            }

            public override void Render(double param)
            {
                (IsTrue() ? TrueState : FalseState).Render(param);
            }

            public override void Update(float time)
            {
                (IsTrue() ? TrueState : FalseState).Update(time);
            }
        }

        private class StateSequence
        {
            List<State<double>> _states;
            public StateSequence Setup(State<double> state)
            {
                _states.Add(state);
                return this;
            }

            public StateSequence Then(State<double> state)
            {
                _states.Add(state);
                return this;
            } 
        }

        public void oldAttack(BattleMessage message, Mons.Mobmon attacker, Mons.Mobmon _oponent, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard)
        {
            var attackInfoState = TimedMessage($"{message.attacker} choose {message.name}");
            var attackMessageState = TimedMessage($"{message.attacker} attacked {message.receiver} for {message.damage} points of damage!");

            var health = _oponent.Health;
            var hasFainted = false;
            var healthbarUpdateState = new TweenState((arg) => _oponent.Health = (float)(health - arg.lerp), () =>
            {
                _oponent.Health = health - message.damage;
                hasFainted = _oponent.Health <= 0;
            }, 0.0f, Math.Min(message.damage, health), 1.0f, EasingFunc.EaseOutCube);

            var xp = attacker.Xp;
            var xpUpdate = new TweenState((arg) => attacker.Xp = (float)(xp + arg.lerp), () =>
            {
                attacker.Xp = xp + 20;
            }, 0.0f, 20, 1.0f, EasingFunc.EaseOutCube);

            var offset = 0;
            var dropPoirtrait = new TweenState((arg) => { oponentCard.PoirtrateAnimDelta = ((int)(offset + arg.lerp)); oponentCard.Dying = true; }, () =>
           {
           }, 0.0f, oponentCard.PortraitSrc.Height, 0.5f, EasingFunc.EaseInBack);

            //_soundCallback(Sounds.Attack_Tackle);
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
                        _soundCallback(Sounds.TakeDamage);


                        _stack.Push(ConfirmMessage($"{_oponent.Name} has fainted"),
                            () =>
                            {
                                _stack.Pop();// pop this message
                                _stack.Push(ConfirmMessage($"XP Gained"), () =>
                                {
                                    _stack.Pop();
                                    _soundCallback(Sounds.XpUP);
                                    _stack.Push(xpUpdate,
                                    () =>
                                    {
                                        _stack.Pop();// pop xpupdate animation
                                    });
                                });
                            });

                        _stack.Push(dropPoirtrait, () => { _stack.Pop(); });
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

                var ey = oponentCard.PortraitOffsetY;
                var hitAnim = new TweenState((arg) =>
                {
                    oponentCard.PortraitOffsetY = (int)(ey + Math.Sin(3.14 * arg.lerp) * 20);
                }, () =>
                {
                }, 0, 1, .3f);

                _stack.Push(hitAnim, () =>
                {
                    _stack.Pop();
                });
                var y = attackerCard.PortraitOffsetY;
                var attackAnimation = new TweenState((arg) =>
                {
                    attackerCard.PortraitOffsetY = (int)(y + Math.Sin(3.14 * arg.lerp) * -40);
                }, () =>
                {
                    _soundCallback(Sounds.Attack_Tackle);
                }, 0, 1, .3f);
                _stack.Push(attackAnimation, () =>
                {
                    _stack.Pop();
                });
            });
        }
    }
}