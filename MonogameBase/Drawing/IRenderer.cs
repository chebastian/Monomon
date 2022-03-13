using Common.Game.Math;

namespace MonoGameBase.Drawing
{
    public interface IRenderer
    {
        void Draw(string v, Vec2 vec2);
        void Draw(string v1, Vec2 vec2, int x, int y, int w, int h);
    }
}
