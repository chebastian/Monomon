namespace Monomon.Animation
{
    public class AttackFrame : Frame
    {
        public AttackFrame(int x, int y) : base(x, y)
        {
        }

        public AttackFrame((int x, int y) src, (int x, int y) origin, int w) : base(src, origin, w)
        {
        }
    }
}