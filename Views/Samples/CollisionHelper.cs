using Common.Game.Math;
using Microsoft.Xna.Framework;
using MonogameBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameBase.Collision
{
    public static class CollisionHelper
    {
        public static CollisionResult HandleCollision(TileMap map,Rect colliderA, Vec2 velA, List<Rect> rects)
        {
            var normalizedDir = velA.Normalize();
            var collisionsResult = Rect.ResolveCollisions(colliderA, rects, velA, (normalizedDir.X, normalizedDir.Y));
            var vel = new Vec2(velA.X, velA.Y);
            foreach (var item in collisionsResult.collision)
            {
                if (item.n.X == 0 && item.n.Y == 0)
                {
                    var tileIdx = map.ToTileIndex((int)item.r.X, (int)item.r.Y);
                    var normals = map.TileOpenSides(tileIdx.x, tileIdx.y);
                    var dots = normals.Select(x => (n: x, dot: Vector2.Dot(new Vector2(x.X, x.Y), new Vector2(normalizedDir.X, normalizedDir.Y)))).ToList();

                    if (dots.Any())
                    {
                        var minDot = dots.Min(x => x.dot);
                        var minItem = dots.First(x => x.dot == minDot);
                        if (minDot < 0)
                        {
                            item.n.X = minItem.n.X;
                            item.n.Y = minItem.n.Y;
                        }
                    }
                }

                vel += item.n * new Vec2(Math.Abs(vel.X), Math.Abs(vel.Y)) * (1.0f - item.t);
                collisionsResult.resultingVelocity = vel;
            }

            return new CollisionResult(collisionsResult.resultingVelocity, collisionsResult.collision);
        }
    }
}
