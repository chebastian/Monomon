using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Animation;
using System;
using System.Collections.Generic;

namespace Monomon.Views.Scenes
{
    class AnimationScene : SceneView
    {
        private Vector2 _pos;
        private Texture2D _tex;
        private AnimationPlayer player;

        public AnimationScene(GraphicsDevice gd, Vector2 pos, Texture2D tex, List<Frame> anim,ContentManager content) : base(gd,content)
        {
            _pos = pos;
            _tex = tex;
            player = new AnimationPlayer(1);
            player.ChangeAnimation(anim);
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            player.Update((float)time);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            var frame = player.CurrentFrame();
            batch.Draw(_tex, _pos,new Rectangle(frame.Source.x,frame.Source.y,24,24), Color.White);
        }
    }

    class TestScene : SceneView
    {
        private readonly Action<double> update;
        private readonly Action<SpriteBatch> render;

        public TestScene(GraphicsDevice gd,Action<double> update, Action<SpriteBatch> render,ContentManager mgr) : base(gd,mgr)
        {
            this.update = update;
            this.render = render;
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            update(time);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            render(batch);
        }
    }

    public enum EasingFunc
    {
        Lerp,
        EaseInBack,
        EaseOutBack,
        EaseOutCube, 
    }

    public class TweenState : State.State<double>
    {
        private readonly Action<(double time, double lerp)> _update;
        private readonly Action completed;
        private readonly float start;
        private readonly float target;
        private readonly float inTime;
        private float totalTime;
        private Func<float, float> _ease;

        public TweenState(Action<(double time, double lerp)> update, Action completed, float start, float target, float inTime, EasingFunc easingFunc = EasingFunc.Lerp)
        {
            _update = update;
            this.completed = completed;
            this.start = start;
            this.target = target;
            this.inTime = inTime;
            totalTime = 0.0f;

            _ease = easingFunc switch
            {
                EasingFunc.Lerp => Lerp,
                EasingFunc.EaseInBack => EaseInBack,
                EasingFunc.EaseOutCube => EaseOutCube,
                EasingFunc.EaseOutBack => EaseOutBack,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void Render(double param)
            {
        }
        float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1.0f - by) + secondFloat * by;
        }

        float Lerp(float x)
        {
            return x;
        }

        float EaseInBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * x * x * x - c1 * x * x;
        }

        float EaseOutCube(float x)
        {
            return (float) 1f - (float)Math.Pow(1-x, 3);
        }
        
        float EaseOutBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return (float)(1.0 + c3 * Math.Pow(x - 1.0, 3.0) + c1 * Math.Pow(x - 1.0, 2.0));
        }

        public override void Update(float time)
        {
            totalTime += time;
            _update((time,Lerp(start,target,_ease(totalTime/inTime))));
            if (totalTime >= inTime)
            {
                completed();
                Completed = true;
            }
        }
    }
}
