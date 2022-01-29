namespace Monomon.Animation
{
    using System;
    using System.Collections.Generic;
#nullable disable

    public class AnimationPlayer
    {
        public AnimationPlayer(float speed)
        {
            _speed = speed;
            _timer = 0.0f;
            _finished = () => { };
            _frame = 0;
            animation = new List<Frame>() {NullFrame};
            _playing = true;
        }
        public AnimationPlayer(float speed, Action onFinish)
        {
            _speed = speed;
            _timer = 0.0f;
            _finished = onFinish;
            _frame = 0;
            animation = new List<Frame> { NullFrame };
            _playing = true;
        }

        public AnimationPlayer(float speed, float offset, List<Frame> start)
        {
            _finished = () => { };
            _speed = speed;
            ChangeAnimation(start);
            _timer = offset % start.Count;
            _frame = 0;
            _playing = true;
        }

        public void Stop()
        {
            _playing = false;
        }

        public void Resume()
        {
            _playing = true;
        }

        private int _frame;
        private float _speed;
        private float _timer;
        private Action _finished;
        List<Frame> animation;
        private float _playback;

        public void ChangeAnimation(List<Frame> anim, float playback = 1.0f)
        {
            if (anim == null)
                throw new ArgumentNullException($"not allowed to set a anim to null");

            animation = anim;
            _playback = playback;
            _frame = 0;
            _timer = 0.0f;
            if(anim.Count > 0)
            {
                anim[0].OnEnter();
            }
        }

        Frame NullFrame = new Frame(0, 0);
        private bool _playing;

        public Frame CurrentFrame()
        {
            return animation[Frame()];
        }

        public int Frame()
        {
            return _frame;
        }

        public void Update(float dt)
        {
            if (!_playing)
                return;

            if (CurrentFrame() is StopFrame stop)
            {
                stop.OnEnter();
                return; 
            }
            var lastFrame = _frame;

            _timer += dt * (_speed * _playback);
            var last = _frame;
            _frame = (int)_timer % animation.Count;

            if (lastFrame != _frame)
                CurrentFrame().OnEnter();

            if(last > _frame )
            {
                _finished();
            }
        }
    }
}