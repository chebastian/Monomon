namespace MonoGameBase.Camera
{
    using Common.Game.Math;
    using System;

    public record RenderMode(int Scale, Func<Vec2> DrawPos, Func<Rect> Window)
    {
        public (float x, float y) ToScreenCoord(Vec2 world)
        {
            return (
                x: world.X - Window().X - DrawPos().X,
                y: world.Y - Window().Y - DrawPos().Y);
        }

        public (float x, float y) ToWorldCoord(Vec2 screen)
        {
            return (
                x: screen.X / Scale + Window().X,
                y: screen.Y / Scale + Window().Y);
        }
    }
}