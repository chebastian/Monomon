using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views
{
    public record SpriteCollection(Rectangle first, Rectangle mid, Rectangle end);
    public class ProgressbarView
    {
        public static void Draw(SpriteBatch batch,float percentage, int fullW, Vector2 pos, SpriteCollection sprites, Texture2D tex,Color color)
        {
            var sz = fullW * percentage;
            var first = new Rectangle((int)pos.X, (int)pos.Y, sprites.first.Width, sprites.first.Height);
            var last = new Rectangle((int)((int)pos.X + sz), (int)pos.Y, sprites.first.Width, sprites.first.Height);
            batch.Draw(tex, first, sprites.first, color);

            for (var i = sprites.first.Width; i < sz; i++)
            { 
                batch.Draw(tex, new Rectangle((int)pos.X + i,(int)pos.Y,1,sprites.mid.Height), sprites.mid, color);
            }

            batch.Draw(tex, last, sprites.end, color);
        }
    }
}
