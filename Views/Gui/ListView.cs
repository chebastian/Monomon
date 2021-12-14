using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Gui
{
    public class ListView
    {
        public static void DrawUIList<T>(UIList<T> list, Vector2 pos, SpriteBatch batch, SpriteFont font) where T : IEquatable<T>
        {
            var y = pos.Y;
            foreach (var item in list.Items.Select((x, i) => (x, i)))
            {
                var c = item.x.Selected ? Color.Red : Color.White;

                batch.DrawString(font, item.x.Item.ToString(), new Vector2(pos.X, y), c);
                y += 20;
            }
        }
    }
}
