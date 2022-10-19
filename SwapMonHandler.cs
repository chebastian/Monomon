using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBase.Input;
using Monomon.Battle;
using Monomon.Mons;
using Monomon.ViewModels;
using Monomon.Views.Scenes;
using System;

namespace Monomon
{
    public class SwapMonHandler : BattleReporter
    {
        public SwapMonHandler(GraphicsDevice gd,
                              SceneStack stack,
                              IINputHandler input,
                              SpriteFont font,
                              Texture2D sprites,
                              Action<Sounds> soundCallback,
                              ContentManager mgr) : base(gd, stack, input, font, sprites, soundCallback, mgr)
        {
        }

        public void Execute(Mons.Mobmon swapper, Mons.Mobmon swapTo, Action doSwap, Action continueWith, BattleCardViewModel swapperCard, BattleCardViewModel swapToCard)
        {
            // IMPORTANT  battle manager does not know that we swapped mon
            var originalX = swapperCard.PortraitOffsetX;
            var message = this.TimedMessage($"Swapping mon to {swapTo.Name}");

            var tweenOut = new TweenState(x => {
                swapperCard.PortraitOffsetX = (int)x.lerp;
            },() => { 
            },swapperCard.PortraitOffsetX,-swapperCard.PortraitSrc.Width,.8f, EasingFunc.EaseOutBack);

            var tweenIn = new TweenState(x => {
                swapperCard.PortraitOffsetX = (int)x.lerp;
            },() => {
                swapperCard.Swap(swapTo.Name, swapTo.MaxHealth, swapTo.Health, 3);
                doSwap();
            },-swapperCard.PortraitSrc.Width,0,.6f, EasingFunc.EaseOutBack);

            _stack.AddState(message);
            _stack.AddState(tweenOut);
            _stack.AddState(tweenIn);
            _stack.AddState(TimedMessage($"{swapTo.Name} is ready to figh!"));
            _stack.EndStateSecence(() => { continueWith(); });
        }
    }
}