namespace MonoGameBase.Entity
{
    using Common.Game.Math;
    using MonogameBase;
    using MonoGameBase.Animation;
    using MonoGameBase.Collision;
    using System.Collections.Generic;

    public interface ICollisionHandler
    {
        public void OnCollidesWithWorld(CollisionResult col);
    }

    public abstract class Entity
    {
        public bool Solid { get; set; } = false;
        public EntityIds Identifier { get;  set; }
        public virtual Vec2 Pos { get; set; }
        public Vec2 TargetPos { get;  set; }
        public Vec2 Velocity { get; set; }
        public virtual Rect Collider { get; set; }
        public bool Fall { get; set; } = true;
        public string SpriteId { get; set; }

        public Rect SourceRect { get; set; }
        public bool OnGround { get; set; }
        public virtual Heading Faceing { get => Velocity.X < 0 ? Heading.Left : Heading.Right; }
        public bool OnStair { get; set; }

        public abstract void OnIntersect(Entity ent);
        public abstract void Update(float dt);
    }

    public class Particle : Entity
    {
        public Particle(Vec2 pos, float life, List<Frame> animation)
        {
            Pos = pos;
            LifeTime = life;
            TotalTime = 0.0f;
            Collider = new Rect(pos.X, pos.Y, 16, 16);
            _animPlayer = new AnimationPlayer(12.0f, () => { TotalTime = LifeTime; });
            _animPlayer.ChangeAnimation(animation);
            SourceRect = new Rect(0, 0, 16, 16);
        }

        public float LifeTime { get; private set; }
        public float TotalTime { get; private set; }

        protected AnimationPlayer _animPlayer;

        public bool Alive => TotalTime <= LifeTime;

        public override void OnIntersect(Entity ent)
        {
        }

        public override void Update(float dt)
        {
            TotalTime += dt;
            if (Alive)
            {
                SourceRect.X = _animPlayer.CurrentFrame().Source.x;
                SourceRect.Y = _animPlayer.CurrentFrame().Source.y;

                _animPlayer.Update(dt);
            }

        }
    }
}