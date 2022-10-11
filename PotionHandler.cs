using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBase.Input;
using Monomon.Battle;
using Monomon.Views.Scenes;
using System;

namespace Monomon
{
    public class PotionHandler : BattleReporter
    {
        public PotionHandler(GraphicsDevice gd, SceneStack stack, IINputHandler input, SpriteFont font, Texture2D sprites, Action<Sounds> soundCallback, ContentManager mgr) : base(gd, stack, input, font, sprites, soundCallback, mgr)
        {

        }

        public void Execute(PotionMessage potion, Mons.Mobmon user, Action continueWith)
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
}