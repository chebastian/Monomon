using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.State;
using Monomon.Views.Constants;
using Monomon.Views.Scenes;
using System;

namespace Monomon.Effects
{
    public class FadeEffect
    {
        private Effect _fadeEffect;
        private float _fadeTime;
        private Texture2D _fadeTexture;
        private Texture2D _flashTexture;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public FadeEffect(Effect effect, Texture2D fadeTexture, Texture2D flashTexture, Texture2D palette, float palettey)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _fadeEffect = effect;
            if (_fadeEffect == null)
                throw new ArgumentNullException();

            _fadeTime = 0.0f;
            //Init fade
            {
                //todo add to ctr
                //fadeTexture = content.Load<Texture2D>("fadeCircleOut");
                _fadeTexture = fadeTexture;
                _flashTexture = flashTexture;

                _fadeEffect.Parameters["flip"].SetValue(false);
                _fadeEffect.Parameters["fadeAmount"].SetValue(0.0f);
                _fadeEffect.Parameters["fadeTexture"].SetValue(fadeTexture);
                _fadeEffect.Parameters["paletteTexture"].SetValue(palette);
                //_fadeEffect.Parameters["paletteY"].SetValue(0.2f); 
                _fadeEffect?.Parameters["paletteY"].SetValue(palettey);
            }

        }

        private void SetTexture(Texture2D tex)
        {
            _fadeEffect.Parameters["fadeTexture"].SetValue(tex);
        }

        public void FadeIn(SceneStack stack, Action? onComplete = null)
        {
            SetTexture(_fadeTexture);
            var fadeIn = new TweenState(arg =>
            {
                _fadeEffect.Parameters["flip"].SetValue(true);
                UpdateFade((float)(1.0f - arg.lerp));
                if (arg.lerp >= 0.8f)
                {
                }
            }, () => { }, 0.0f, 1.0f, 1.4f, EasingFunc.Lerp);

            stack.Push(fadeIn, () => { stack.Pop(); onComplete?.Invoke(); });
        }

        public void FadeOut(SceneStack stack, Action? onComplete = null)
        {
            SetTexture(_fadeTexture);
            var fade = new TweenState(arg =>
            {
                _fadeEffect.Parameters["flip"].SetValue(false);
                UpdateFade((float)(1.0 - arg.lerp));
            }, () => { }, 0.0f, 1.0f, 1.4f, EasingFunc.Lerp);

            stack.Push(fade, () => { stack.Pop(); onComplete?.Invoke(); });
        }

        public void DoFade(SceneStack stack, Action onready, Action? onComplete = null)
        {
            SetTexture(_fadeTexture);
            var fade = new TweenState(arg =>
            {
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

            stack.Push(fade, () => { stack.Pop(); onComplete?.Invoke(); });
            stack.Push(fadeIn, () => { stack.Pop(); onready(); });
        }

        public void DoFade(float speed, SceneStack stack, Action onready, Action? onComplete = null)
        {
            SetTexture(_fadeTexture);
            var fade = new TweenState(arg =>
            {
                _fadeEffect.Parameters["flip"].SetValue(false);
                UpdateFade((float)(1.0 - arg.lerp));
                if (arg.lerp >= 0.8f)
                {
                }
            }, () => { }, 0.0f, 1.0f, speed, EasingFunc.Lerp);

            var fadeIn = new TweenState(arg =>
            {
                _fadeEffect.Parameters["flip"].SetValue(true);
                UpdateFade((float)(1.0f - arg.lerp));
                if (arg.lerp >= 0.8f)
                {
                }
            }, () => { }, 0.0f, 1.0f, speed, EasingFunc.Lerp);

            stack.Push(fade, () => { stack.Pop(); onComplete?.Invoke(); });
            stack.Push(fadeIn, () => { stack.Pop(); onready(); });
        }

        public void Flash(float speed, SceneStack stack, Action onready, Action? onComplete = null)
        {
            var flashSpeed = 0.1f;
            {
                SetTexture(_flashTexture);
                var fade = new TweenState(arg =>
                {
                    _fadeEffect.Parameters["flip"].SetValue(false);
                    UpdateFade((float)(1.0 - arg.lerp));
                }, () => { }, 0.0f, 1.0f, flashSpeed, EasingFunc.Lerp);

                var fadeIn = new TweenState(arg =>
                {
                    _fadeEffect.Parameters["flip"].SetValue(true);
                    UpdateFade((float)(1.0f - arg.lerp));
                }, () => { }, 0.0f, 1.0f, flashSpeed, EasingFunc.Lerp);

                var stop = new TweenState(arg =>
                {
                }, () => { }, 0.0f, 1.3f, 0.7f, EasingFunc.Lerp);


                stack.Push(stop, () => { stack.Pop(); onComplete?.Invoke(); });
                stack.Push(fade, () => { stack.Pop(); onready(); });
                stack.Push(fadeIn, () => { stack.Pop(); });
            }

            {
                SetTexture(_flashTexture);
                var fade = new TweenState(arg =>
                {
                    _fadeEffect.Parameters["flip"].SetValue(false);
                    UpdateFade((float)(1.0 - arg.lerp));
                }, () => { }, 0.0f, 1.0f, flashSpeed, EasingFunc.Lerp);

                var fadeIn = new TweenState(arg =>
                {
                    _fadeEffect.Parameters["flip"].SetValue(true);
                    UpdateFade((float)(1.0f - arg.lerp));
                }, () => { }, 0.0f, 1.0f, flashSpeed, EasingFunc.Lerp);

                stack.Push(fade, () => { stack.Pop(); });
                stack.Push(fadeIn, () => { stack.Pop(); });
            } 

        }



        public void Draw(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, _fadeEffect);
            batch.Draw(_fadeTexture, new Rectangle(0, 0, UIValues.WinW, UIValues.WinH), Color.White);
            batch.End();
        }

        void UpdateFade(float dt)
        {
            _fadeTime = dt;
            _fadeEffect.Parameters["fadeAmount"].SetValue(_fadeTime);
        }
    }
}
