using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBase.Input;
using Monomon.Battle;
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

    public class PotionHandler : BattleReporter
    {
        public PotionHandler(GraphicsDevice gd, State.StateStack<double> stack, IINputHandler input, SpriteFont font, Texture2D sprites, Action<Sounds> soundCallback, ContentManager mgr) : base(gd, stack, input, font, sprites, soundCallback, mgr)
        {

        }

        public void Execute(PotionMessage potion, Mons.Mobmon user, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard, bool isPlayer)
        {
            var health = user.Health;
            var healthbarUpdateState = new TweenState((arg) => user.Health = Math.Min(user.MaxHealth, (float)(health + arg.lerp)), () =>
            {
                user.Health = Math.Min(health + potion.hpRestored, user.MaxHealth);
            }, 0.0f, Math.Min(potion.hpRestored, health), 1.0f, EasingFunc.Lerp);

            _stack.AddState(healthbarUpdateState, () =>
            {
                potion.Use(user);
            });

            _stack.EndStateSecence(() => { continueWith(); });
        } 
    }

    public class AttackHandler : BattleReporter
    {
        public AttackHandler(GraphicsDevice gd, StateStack<double> stack, IINputHandler input, SpriteFont font, Texture2D sprites, Action<Sounds> soundCallback, ContentManager mgr) : base(gd, stack, input, font, sprites, soundCallback, mgr)
        {
        }

        public void Execute(BattleMessage message, Mons.Mobmon attacker, Mons.Mobmon _oponent, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard, bool isPlayer)
        {
            var attackInfoState = TimedMessage($"{message.attacker} used {message.name}");

            _stack.BeginStateSequence();
            _stack.AddState(attackInfoState);

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

            _stack.AddState(attackAnimation);
            _stack.AddState(hitAnim, () => { });

            var health = _oponent.Health;
            var hasFainted = false;
            var healthbarUpdateState = new TweenState((arg) => _oponent.Health = (float)(health - arg.lerp), () =>
            {
                _oponent.Health = health - message.damage;
                hasFainted = _oponent.Health <= 0;
            }, 0.0f, Math.Min(message.damage, health), 1.0f, EasingFunc.EaseOutCube);

            _stack.AddState(healthbarUpdateState, () =>
            {
                _oponent.Health = health - message.damage;
            });

            if (health - message.damage <= 0)
            {
                var offset = 0;
                var dropPoirtrait = new TweenState((arg) => { oponentCard.PoirtrateAnimDelta = ((int)(offset + arg.lerp)); oponentCard.Dying = true; }, () =>
               {
               }, 0.0f, oponentCard.PortraitSrc.Height, 0.5f, EasingFunc.EaseInBack);

                _stack.AddState(dropPoirtrait, null, () => _soundCallback(Sounds.TakeDamage));
                _stack.AddState(ConfirmMessage($"{_oponent.Name} has fainted"));
                if (isPlayer)
                {
                    _stack.AddState(ConfirmMessage("XP Gained"));
                    var xp = attacker.Xp;
                    var xpUpdate = new TweenState((arg) => attacker.Xp = (float)(xp + arg.lerp), () =>
                    {
                        attacker.Xp = xp + 20;
                    }, 0.0f, 20, 1.0f, EasingFunc.EaseOutCube);

                    _stack.AddState(xpUpdate, null, () => _soundCallback(Sounds.XpUP));
                }
            }

            _stack.EndStateSecence(() =>
            {
                if (!hasFainted) //continue will swap to next round, if we dont we will prompt for which turn to go next
                    continueWith();
            });
        }
    }

    public class BattleReporter
    {
        protected readonly StateStack<double> _stack;
        protected ContentManager _content;
        protected Action<Sounds> _soundCallback;
        protected Texture2D _sprites;
        protected SpriteFont _font;
        protected IINputHandler _input;
        protected GraphicsDevice _gd;

        public List<string> Messages { get; set; }
        public BattleReporter(GraphicsDevice gd, State.StateStack<double> stack, IINputHandler input, SpriteFont font, Texture2D sprites, Action<Sounds> soundCallback, ContentManager mgr)
        {
            _content = mgr;
            _soundCallback = soundCallback;
            _sprites = sprites;
            _font = font;
            _input = input;
            _gd = gd;

            _stack = stack;
            Messages = new List<string>();
        }

        protected TimedState TimedMessage(string message)
        {
            return new TimedState(new MessageScene(_gd, message, _font, _sprites, _content), 2500, _input);
        }

        protected ConfirmState ConfirmMessage(string message)
        {
            return new ConfirmState(new MessageScene(_gd, message, _font, _sprites, _content, true), _input);
        }

        public void OnItem(ItemMessage message, Mons.Mobmon user, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard, bool isPlayer)
        {
            var attackInfoState = TimedMessage($"{message.user} used {message.name}");
            _stack.BeginStateSequence();
            _stack.AddState(attackInfoState);

            if (message is PotionMessage potion)
            {
                var health = user.Health;
                var healthbarUpdateState = new TweenState((arg) => user.Health = Math.Min(user.MaxHealth, (float)(health + arg.lerp)), () =>
                {
                    user.Health = Math.Min(health + potion.hpRestored, user.MaxHealth);
                }, 0.0f, Math.Min(potion.hpRestored, health), 1.0f, EasingFunc.Lerp);

                _stack.AddState(healthbarUpdateState, () =>
                {
                    message.Use(user);
                });

                _stack.EndStateSecence(() => { continueWith(); });

            }
        }

        public void OnAttack(BattleMessage message, Mons.Mobmon attacker, Mons.Mobmon _oponent, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard, bool isPlayer)
        {
            var handler = new AttackHandler( _gd, _stack, _input, _font, _sprites, _soundCallback, _content);
            handler.Execute(message,attacker,_oponent,continueWith,attackerCard,oponentCard,isPlayer);
        }

    }
}