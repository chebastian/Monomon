using System.Collections.Generic;

namespace MonoGameBase.Collision
{
    using Common.Game.Math;

    public class CollisionResult
    {
        public CollisionResult(Vec2 velocity, List<(Rect, Vec2, float)> cols)
        {
            ResultingVelocity = velocity;
            Collisions = cols;
        }


        public Vec2 ResultingVelocity { get; }
        public List<(Rect r, Vec2 n, float t)> Collisions { get; }
    }
}