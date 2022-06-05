using Common.Game.Math;
using System;
using System.Diagnostics;

namespace Monomon.Views.Samples
{
    using Monomon.Data;

    public class Player
    {
        public Player()
        {
            Pos = new Vec2();
            Vel = new Vec2();
            Target = new Vec2();
            Dist = 0.0f;
            OgPos = new Vec2();
            _onCompleted = () => { };
        }
        public Vec2 Pos { get; set; }
        public Vec2 Vel { get; set; }
        public Vec2 Target { get; set; }

        private Action _onCompleted;

        public float Dist { get; set; }
        public Vec2 OgPos { get; private set; }

        public Vec2 Center
        {
            get
            {
                return Pos + new Vec2(TileValues.TileW / 2, TileValues.TileH / 2);
            }
        }

        public void SetTarget(Vec2 target)
        {
            var playerCenter = Pos + new Vec2(8, 8);
            var tileBegin = ((int)(playerCenter.X / 16) * 16, ((int)playerCenter.Y / 16) * 16);

            if (Dist <= 0.0f && (target.X != 0 || target.Y != 0))
            {
                Debug.WriteLine("hit");
                Target = target + new Vec2(tileBegin.Item1, tileBegin.Item2);
                Dist = 0.0f;
            }
        }

        public void Advance(float d, float t)
        {
            if (Target.X == 0 && Target.Y == 0)
                return;

            Dist += d;
            Dist = System.MathF.Min(Dist, 1.0f);
            if (Dist >= 1)
            {
                Pos = Pos.Quad(Target, Dist);
                Target = new Vec2(0, 0);
                Dist = 0.0f;
            }

            if (Target.X == 0 && Target.Y == 0)
                return;

            if (Dist <= 1)
                Pos = Pos.Lerp(Target, Dist);
        }

        internal void WalkInDirection(Vec2 target, Action onCompleted)
        {
            Target = target;//new Vec2(Center.X + dx * Constants.TileW, Center.Y + dy* Constants.TileH);
            _onCompleted = onCompleted;
            Dist = 1.0f;
            OgPos = Pos;
        }

        internal void Update(float dt)
        {
            if (Dist > 0.0)
            {
                Dist -= dt * 4.0f;
                Dist = MathF.Max(0.0f, Dist);
                Pos = OgPos.Lerp(Target, 1.0f - Dist);

                if(Dist == 0.0f)
                {
                    _onCompleted();
                }
            }
        }
    }


}
