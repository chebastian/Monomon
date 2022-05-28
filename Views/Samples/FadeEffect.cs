using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.State;
using Monomon.Views.Scenes;
using System;
#nullable disable

namespace Monomon.Views.Samples
{
    class FadeEffect
    {
        private Effect _fadeEffect;
        private float _fadeTime;
        private Texture2D _fadeTexture;

        public FadeEffect(Effect effect, Texture2D fadeTexture, Texture2D palette, float palettey)
        {
            _fadeEffect = effect;
            //Init fade
            {
                //todo add to ctr
                //fadeTexture = content.Load<Texture2D>("fadeCircleOut");
                _fadeTexture = fadeTexture;

                _fadeEffect.Parameters["flip"].SetValue(false);
                _fadeEffect.Parameters["fadeAmount"].SetValue(0.0f);
                _fadeEffect.Parameters["fadeTexture"].SetValue(fadeTexture);
                _fadeEffect.Parameters["paletteTexture"].SetValue(palette);
                //_fadeEffect.Parameters["paletteY"].SetValue(0.2f); 
                _fadeEffect?.Parameters["paletteY"].SetValue(palettey);
            }

        }

        public void DoFade(StateStack<double> stack, Action onready)
        {
            var fade = new TweenState(arg =>
            {
                onready();
                _fadeEffect.Parameters["flip"].SetValue(false);
                UpdateFade((float)(1.0 - arg.lerp));
                if (arg.lerp >= 0.8f)
                {
                }
            }, () => { }, 0.0f, 1.0f, 1.4f, EasingFunc.Lerp);

            var fadeIn = new TweenState(arg =>
            {
                _fadeEffect.Parameters["flip"].SetValue(true);
                UpdateFade((float)(1.0f - arg.lerp));
                if (arg.lerp >= 0.8f)
                {
                }
            }, () => { }, 0.0f, 1.0f, 1.4f, EasingFunc.Lerp);

            stack.Push(fade, () => stack.Pop());
            stack.Push(fadeIn, () => stack.Pop());
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null, _fadeEffect);
            batch.Draw(_fadeTexture, new Rectangle(0, 0, 800, 600), Color.White);
            batch.End();
        }

        void UpdateFade(float dt)
        {
            _fadeTime = dt;
            _fadeEffect.Parameters["fadeAmount"].SetValue(_fadeTime);
        }
    }
}
