using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Battle;
using Monomon.Input;
using Monomon.State;
using Monomon.ViewModels;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;

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
            return new TimedState(new MessageScene(_gd,message,_font,_sprites), 2500, _input);
        }

        ConfirmState ConfirmMessage(string message)
        { 
            return new ConfirmState(new MessageScene(_gd,message,_font,_sprites,true), _input);
        }


        public void OnAttack(BattleMessage message,Mons.Mobmon attacker, Mons.Mobmon _oponent, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard)
        {
            var attackInfoState = TimedMessage($"{message.attacker} choose {message.name}");
            var attackMessageState = TimedMessage($"{message.attacker} attacked {message.receiver} for {message.damage} points of damage!");

            var health = _oponent.Health;
            var hasFainted = false;
            var healthbarUpdateState = new TweenState((arg) => _oponent.Health = (float)(health - arg.lerp), () =>
            {
                _oponent.Health = health - message.damage;
                hasFainted = _oponent.Health <= 0;
            }, 0.0f, Math.Min(message.damage,health) , 1.0f,EasingFunc.EaseOutCube);

            var xp = attacker.Xp;
            var xpUpdate = new TweenState((arg) => attacker.Xp = (float)(xp + arg.lerp), () =>
            {
                attacker.Xp = xp + 20;
            }, 0.0f, 20, 1.0f,EasingFunc.EaseOutCube);

            var offset = 0;
            var dropPoirtrait = new TweenState((arg) => { oponentCard.PoirtrateAnimDelta =  ((int)(offset + arg.lerp)); oponentCard.Dying = true; }, () =>
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
                                _stack.Push(ConfirmMessage($"XP Gained"), () => { 
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
                var hitAnim = new TweenState((arg) => {
                    oponentCard.PortraitOffsetY = (int)(ey + Math.Sin(3.14 * arg.lerp)*20);
                }, () => {
                }, 0, 1, .3f);

                _stack.Push(hitAnim, () =>
                {
                    _stack.Pop();
                });
                var y = attackerCard.PortraitOffsetY;
                var attackAnimation = new TweenState((arg) => {
                    attackerCard.PortraitOffsetY = (int)(y + Math.Sin(3.14 * arg.lerp)*-40);
                }, () => {
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