using Common.Game.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBase.Camera
{
    public static class CameraHelper
    {
        public static Vec2 ToWindowPosition(Vec2 pos, Vec2 window)
        {
            return new Vec2((int)((pos.X - window.X)), (int)((pos.Y - window.Y)));
        }
        public static void UpdateCamera(float dt, float maxX, Heading lookDir, Rect cameraRect, Vec2 targetPos, int offsetx, int offsety, int sizeW)
        {
            var xTarget = lookDir == Heading.Right ? -1 : 1;
            var xTargetOffset = (sizeW);
            var dd = new Vec2(cameraRect.X, cameraRect.Y).Quad(targetPos - new Vec2((cameraRect.Width * 0.5f) + (xTarget * xTargetOffset), cameraRect.Height * 0.5f), dt);

            cameraRect.X = dd.X + offsetx;
            cameraRect.Y = dd.Y + offsety;
            if (cameraRect.Right > maxX && maxX > 0)
                cameraRect.X = maxX - cameraRect.Width;
            else if (cameraRect.X <= 0)
                cameraRect.X = 0;
        }
    }
}
