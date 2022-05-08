namespace Common.Game.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Vec2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2()
        {
            X = 0;
            Y = 0;
        }

        public Vec2 Lerp(Vec2 b, float d)
        {
            var dir = b - this;
            return this + (dir * Math.Min(d,1.0f));
        }

        public Vec2 Quad(Vec2 b, float d)
        {
            var dir = b - this;
            var expo = 1.0f - Math.Pow(2, -10 * d);
            return this + (dir * Math.Min((float)expo,1.0f));
        }

        public float SingleBounce(float a,float b, float d)
        {
            var dir = b - a;
            var expo = Math.Abs(MathF.Sin(d * ((float)Math.PI)));
            return a + (dir * expo);
        }

        public float Bounce(float a,float b, float d)
        {
            var dir = b - a;
            var expo = Math.Abs(MathF.Sin(d * ((float)Math.PI*1.5f)));
            return a + (dir * expo);
        }


        public static Vec2 operator *(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X * b.X, a.Y * b.Y);
        }

        public static Vec2 operator *(Vec2 a, float scalar)
        {
            return new Vec2(a.X * scalar, a.Y * scalar);
        }

        public static Vec2 operator *(Vec2 a, double scalar)
        {
            return new Vec2((float)(a.X * scalar), (float)(a.Y * scalar));
        }


        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2 operator /(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X / b.X, a.Y / b.Y);
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }

        public Vec2 Normalize()
        {
            var len = Length();
            var nx = X != 0.0f ? X / len : 0.0f;
            var ny = Y != 0.0f ? Y / len : 0.0f;
            return new Vec2(nx, ny);
        }

        public float LengthSquared()
        {
            return (X * X + Y * Y);
        }

        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec2 other)
                return X == other.X && Y == other.Y;

            return false;
        }

        public override string ToString()
        {
            return $"{X} {Y}";
        }


    }

    public class Rect
    {
        public Rect(float x, float y, float w, float h)
        {
            X = x; Y = y; Width = w; Height = h;
        }

        public static bool Intersects(Rect a, Rect b)
        {
            return false;
        }

        public (float x, float y) Center()
        {
            return (X + Width * 0.5f, Y + Height * 0.5f);
        }

        public Rect MinkowskiSum(Rect t)
        {
            return new Rect(X - t.Width * 0.5f, Y - t.Height * 0.5f, Width + t.Width, Height + t.Height);
        }

        public static Rect FromBounds(float left, float top, float right, float bottom)
        {
            return new Rect(left, top, (int)(right - left), (int)(bottom - top));
        }

        public (float x, float y) ToLocalCoords(float x, float y)
        {
            return (x - X, y - Y);
        }

        public (float x, float y) LocalToWorld(float x, float y)
        {
            return (x + X, y + Y);
        }

        public Rect EnclosingRect(Rect r)
        {
            return Rect.FromBounds(
                Math.Min(r.X, X),
                Math.Min(r.Y, Y),
                Math.Max(r.Right, Right),
                Math.Max(r.Bottom, Bottom));
        }

        public static (Vec2 resultingVelocity,List<(Rect r,Vec2 n,float t)> collision) ResolveCollisions(Rect r, List<Rect> colliders, Vec2 velocity, (float x, float y) dir)
        {
            r.X = (float)Math.Round(r.X, 2);
            r.Y = (float)Math.Round(r.Y, 2);
            var minkowskiSums = colliders.Select((x,i) => (idx:i,sum:x.MinkowskiSum(r)));
            var centerVec = new Vec2(r.Center().x, r.Center().y);
            var target = centerVec + velocity;

            var intersections = minkowskiSums.Select(x =>
            (intersect: x.sum.RayIntersect((centerVec.X, centerVec.Y), (target.X, target.Y), (dir.x, dir.y)),
            rect: x)).Where(x => x.intersect.t).ToList();

            var intersectionsInOrder = intersections.OrderBy(x => x.intersect.time).ToList();
            var resultingVelocity = velocity;

            var resolvedIntersections = new List<(Rect, Vec2 n, float t)>();

            //Debug.WriteLineIf(intersectionsInOrder.Any(x => x.intersect.norm == (0, 1)), "Collides");
            foreach (var intersection in intersectionsInOrder)
            {
                if (intersection.intersect.time <= 1.0f)
                {
                    if (intersection.intersect.time < 0.0f)
                    {
                        continue;
                    }


                    var secondPassIntersection = intersection.rect.sum.RayIntersect(centerVec, centerVec + resultingVelocity, resultingVelocity.Normalize());
                    if (secondPassIntersection.t && secondPassIntersection.time <= 1.0f)
                    {
                        var n = new Vec2(secondPassIntersection.norm.x, secondPassIntersection.norm.y);
                        if(n.X == 1.0 && n.Y == 1.0)
                        {

                        }
                        resultingVelocity += (n * new Vec2(Math.Abs(resultingVelocity.X), Math.Abs(resultingVelocity.Y))) * (1.0f - secondPassIntersection.time);
                        resolvedIntersections.Add((colliders[intersection.rect.idx], n, secondPassIntersection.time));
                    }
                }
            }

            return (resultingVelocity, resolvedIntersections);
        }

        public (bool t, float time, (float x, float y) norm) RayIntersect((float x, float y) p, (float x, float y) target, (float x, float y) dir)
        {
            return RayIntersect(new Vec2(p.x, p.y), new Vec2(target.x, target.y), new Vec2(dir.x, dir.y));
        }

        public (bool t, float time, (float x, float y) norm) RayIntersect(Vec2 p, Vec2 p2, Vec2 dir)
        {
            var d = p2 - p;

            if (dir.X == 0.0f && dir.Y == 0.0f)
                return (false, 0.0f, (0, 0));

            var invDir = (X: 1.0f / d.X, Y: 1.0f / d.Y);

            (float x, float y) near = ((X - p.X) * invDir.X, (Y - p.Y) * invDir.Y);
            (float x, float y) far = ((Right - p.X) * invDir.X, (Bottom - p.Y) * invDir.Y);



            if (float.IsNaN(near.x) || float.IsNaN(near.y) || float.IsNaN(far.x) || float.IsNaN(far.y))
            {
                return (false, 0.0f, (0, 0));
            }

            if (near.x > far.x)
            {
                var tmp = near.x;
                near.x = far.x;
                far.x = tmp;
            }
            if (near.y > far.y)
            {
                var tmp = near.y;
                near.y = far.y;
                far.y = tmp;
            }

            var farHit = Math.Min(far.x, far.y);
            if (farHit < 0)
                return (false, 0.0f, (0.0f, 0.0f));

            if (near.x > far.y || near.y > far.x)
                return (false, 0.0f, (0.0f, 0.0f));

            var hitNear = Math.Max(near.x, near.y);
            var hitFar = Math.Min(far.x, far.y);

            (float x, float y) norm = (0f, 0f);
            if (near.x > near.y)
            {
                norm.x = d.X > 0 ? -1.0f : 1.0f;
            }
            //favor collision resolutions in the Y axis, causes sliding in the Y axis to be stopped when jumping straight into a wall
            else if (near.x < near.y)
            {
                norm.y = d.Y > 0 ? -1.0f : 1.0f;
            } 
            else
            {
                //Causes some more problems, with tiles gettings stuck, but this must be fixed
                //norm.x = -invDir.X;
                //norm.y = -invDir.Y;
            }

            if (hitNear < 0.0f || hitNear > 1.0f)
            {
                return (false, 0, (0, 0));
            }

            var neither = (near.x > far.y) || (near.y > far.x);
            return (!neither && !float.IsInfinity(hitNear) && hitNear >= 0.0f, hitNear, norm);
        }

        public bool Intersects(Rect t)
        {
            var r = this.MinkowskiSum(t);
            var center = t.Center();
            return (center.x > r.X && center.x < r.Right) && (center.y > r.Y && center.y < r.Bottom);
        }

        public Rect Translate(float x, float y)
        {
            return new Rect(X + x, Y + y, Width, Height);
        }

        public float Right { get => X + Width; }
        public float Bottom { get => Y + Height; }


        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public override string ToString()
        {
            return $"{X} {Y} {Width} {Height}";
        }
    }
}
