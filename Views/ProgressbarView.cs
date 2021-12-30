using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views
{
    public record SpriteCollection(Rectangle first, Rectangle mid, Rectangle end)
    {
        public static SpriteCollection FromRow(int x,int y, int xoffset, int h) => new SpriteCollection(
            new Rectangle(x,y,xoffset,h), new Rectangle(x+xoffset,y,xoffset,h), new Rectangle(x+xoffset+xoffset,y,xoffset,h)
            );
    }
    public record NineSquare(SpriteCollection top, SpriteCollection mid, SpriteCollection bottom);
    public class ProgressbarView
    {
        public static void Draw(SpriteBatch batch, float percentage, int fullW, Vector2 pos, SpriteCollection sprites, Texture2D tex, Color color)
        {
            var sz = Math.Max(sprites.first.Width, fullW * percentage);

            var first = new Rectangle((int)pos.X, (int)pos.Y, sprites.first.Width, sprites.first.Height + 2);
            var last = new Rectangle((int)((int)pos.X + fullW), (int)pos.Y, sprites.first.Width, sprites.first.Height+2);

            batch.Draw(tex, first, sprites.first, Color.White);

            for (var i = sprites.first.Width; i < fullW; i++)
            { 
                batch.Draw(tex, new Rectangle((int)pos.X + i,(int)pos.Y,1,sprites.mid.Height+2), sprites.mid, Color.White);

                if (i < fullW * percentage)
                    batch.Draw(tex, new Rectangle((int)pos.X + i, (int)pos.Y + 2, 1, sprites.mid.Height - 2), sprites.mid, color);
            }

            batch.Draw(tex, last, sprites.end, Color.White);
        }
    }
}
