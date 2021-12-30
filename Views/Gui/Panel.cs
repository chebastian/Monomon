using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Gui
{
    public class Panel
    {
        public static NineSquare BasePanel = new NineSquare(
            //Top
            SpriteCollection.FromRow(0,40,8,8),
            //Mid
            SpriteCollection.FromRow(0,48,8,8),
            //Bottom
            SpriteCollection.FromRow(0,56,8,8)
            );

        public static NineSquare AltPanel = new NineSquare(
            //Top
            SpriteCollection.FromRow(0,64,8,8),
            //Mid
            SpriteCollection.FromRow(0,72,8,8),
            //Bottom
            SpriteCollection.FromRow(0,80,8,8)
            );
        public static void Draw(SpriteBatch batch, Texture2D texture, NineSquare sprites, Rectangle rect)
        {

            batch.Draw(texture, new Rectangle(rect.Left+sprites.top.first.Width,rect.Top+sprites.top.first.Height,rect.Width-sprites.top.end.Width,rect.Height-sprites.top.end.Height),
                      sprites.mid.mid, Color.White);

            batch.Draw(texture, new Rectangle(rect.Left+sprites.top.first.Width,rect.Top,rect.Width-sprites.top.end.Width,sprites.top.mid.Height),
                      sprites.top.mid, Color.White);

            batch.Draw(texture, new Rectangle(rect.Left+sprites.bottom.first.Width,rect.Bottom,rect.Width-sprites.bottom.end.Width,sprites.bottom.mid.Height),
                      sprites.bottom.mid, Color.White);

            batch.Draw(texture, new Rectangle(rect.Left,rect.Top+sprites.top.first.Height,sprites.mid.first.Width,rect.Height-sprites.bottom.first.Height),
                      sprites.mid.first, Color.White);

            batch.Draw(texture, new Rectangle(rect.Right,rect.Top+sprites.top.end.Height,sprites.mid.end.Width,rect.Height-sprites.bottom.end.Height),
                      sprites.mid.end, Color.White);


            batch.Draw(texture, new Rectangle(rect.Left, rect.Top, sprites.top.first.Width, sprites.top.first.Height), sprites.top.first, Color.White);
            batch.Draw(texture, new Rectangle(rect.Right, rect.Top, sprites.top.end.Width, sprites.top.end.Height), sprites.top.end, Color.White);

            batch.Draw(texture, new Rectangle(rect.Left, rect.Bottom, sprites.bottom.first.Width, sprites.bottom.first.Height), sprites.bottom.first, Color.White);
            batch.Draw(texture, new Rectangle(rect.Right, rect.Bottom, sprites.bottom.end.Width, sprites.bottom.end.Height), sprites.bottom.end, Color.White);

            //batch.Draw(texture, rect
            //          sprites.top.mid, Color.White);

            //batch.Draw(texture, new Rectangle(rect.Left + sprites.top.first.Width,
            //                                  rect.Top + sprites.top.first.Height,
            //                                  rect.Width-sprites.top.end.Width,
            //                                  sprites.bottom.end.Height),
            //          sprites.top.mid, Color.White);
        }
    }
}
