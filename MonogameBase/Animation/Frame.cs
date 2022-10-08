namespace MonoGameBase.Animation
{
    using System;

    public class Frame : IEquatable<Frame>
    {
        public (int x, int y) Source;
        public int w;
        public (float x, float y) _origin;
        public (float x, float y) Origin
        {
            get
            {
                if (w > 1 && flipped)
                    return (Math.Abs(_origin.x - (w - 1)), _origin.y);

                return _origin;
            }
        }

        public bool flipped { get; set; }

        public Frame(int x, int y)
        {
            Source = (x, y);
            _origin = (0, 0);
            w = 1;
            _onEnter = () => { };
        }

        public Frame(int x, int y, Action onEnter)
        {
            Source = (x, y);
            _origin = (0, 0);
            w = 1;
            _onEnter = onEnter;
        }

        public Frame((int x, int y) src, (float x, float y) origin, int w)
        {
            Source = src;
            _origin = origin;
            this.w = w;
            _onEnter = () => { };
        }

        public Frame((int x, int y) src, (float x, float y) origin, int w, Action onEnter)
        {
            Source = src;
            _origin = origin;
            this.w = w;
            _onEnter = onEnter;
        }

        public Action OnEnter => _onEnter;
        protected Action _onEnter;

        public bool Equals(Frame? other)
        {
            if(other != null)
            {
                return other.Source == Source;
            }

            return false;
        }
    }

    public class StopFrame : Frame
    {
        private bool _called;

        public StopFrame(int x, int y) : base(x, y)
        {
            _onEnter = () => { };
        }

        public StopFrame(int x, int y, Action onEnter) : base(x, y)
        {
            _called = false;
            _onEnter = () =>
            {
                if (!_called)
                    onEnter();

                _called = true;
            };
        }

    }
}