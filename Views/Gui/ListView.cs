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
        public static Texture2D? PanelTexture { get; set; }

        public static void DrawUIList<T>(UIList<T> list, Vector2 pos, SpriteBatch batch, SpriteFont font) where T : IEquatable<T>
        {
            var padding = 10;
            var h = list.Items.Sum(x => font.MeasureString(x.Item.ToString()).Y) + padding+padding;
            var w = list.Items.Max(x => font.MeasureString(x.Item.ToString()).X) + padding+padding;
            if (PanelTexture != null)
            {
                Panel.Draw(batch, PanelTexture, Panel.BasePanel, new Rectangle((int)pos.X, (int)pos.Y, (int)w, (int)h));
            }


            var y = pos.Y;
            foreach (var item in list.Items.Select((x, i) => (x, i)))
            {
                var c = item.x.Selected ? Color.Red : Color.White;

                batch.DrawString(font, item.x.Item.ToString(), new Vector2(pos.X+padding, y+padding), c);
                y += 20;
            }
        }
    }
}
