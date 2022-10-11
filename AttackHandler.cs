using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBase.Input;
using Monomon.Battle;
using Monomon.State;
using Monomon.ViewModels;
using Monomon.Views.Scenes;
using System;

namespace Monomon
{
    public class AttackHandler : BattleReporter
    {
        public AttackHandler(GraphicsDevice gd, SceneStack stack, IINputHandler input, SpriteFont font, Texture2D sprites, Action<Sounds> soundCallback, ContentManager mgr) : base(gd, stack, input, font, sprites, soundCallback, mgr)
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
}