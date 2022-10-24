using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Mons;
using Monomon.UI;
using Monomon.Views.Battle;
using Monomon.Views.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
            var h = list.Items.Sum(x => font.MeasureString(x.Item.ToString()).Y) + padding;
            var w = list.Items.Max(x => font.MeasureString(x.Item.ToString()).X) + padding+padding;
            if (PanelTexture != null)
            {
                Panel.Draw(batch, PanelTexture, Panel.BasePanel, new Rectangle((int)pos.X, (int)pos.Y, (int)Math.Max(w,UIValues.ListMinW), (int)h));
            }


            var y = pos.Y;
            if (!list.Items.Any())
                return;

            var itemHeight = font.MeasureString(list.Items.First().Item.ToString()).Y;
            foreach (var item in list.Items.Select((x, i) => (x, i)))
            {
                var c = item.x.Selected ? Color.Red : Color.White;

                var x = pos.X + padding + (item.x.Selected ? padding : 0);
                batch.DrawString(font, item.x.Item.ToString(), new Vector2(x, y+padding), c);
                if(item.x.Selected)
                    batch.DrawString(font,">",new Vector2(pos.X+padding,y+padding), c);

                y += itemHeight;
            }
        }
        public abstract class Drawable<T> : IMeasureable, IEquatable<Drawable<T>> where T: IEquatable<T>
        {
            public abstract void Draw(SpriteBatch batch, Vector2 pos);
            public abstract T Item { get; }


            public bool Equals(Drawable<T>? other)
            {
                if (other is null)
                    return false;

                return Item?.Equals(other.Item) ?? false;
            }

            public abstract Vector2 Meassure();
        }

        public class TestDrawable : Drawable<Mobmon>
        {
            public TestDrawable(Mobmon item, Texture2D spriteMap, SpriteFont font)
            {
                _font = font;
                _spriteMap = spriteMap;
                Item = item;
            }

            private SpriteFont _font;
            private Texture2D _spriteMap;

            public override Mobmon Item { get; }

            public override void Draw(SpriteBatch batch, Vector2 pos)
            {
                batch.DrawString(_font, $"{Item.Name} {Item.Health}", pos, Color.White);
                ProgressbarView.Draw(batch, 0.4f, 40, pos, BattleCardView.miniBarSprites, BattleCardView.miniEmptyBarSprites, _spriteMap, Color.White);
            }

            public override Vector2 Meassure()
            {
                return new (40,BattleCardView.miniBarSprites.first.Height + _font.MeasureString("a").Y);
            }
        }

        public interface IMeasureable
        {
            public Vector2 Meassure();
        }

        public static void DrawUIListX<T>(List< UIItem<Drawable<T>> > list, Vector2 pos, SpriteBatch batch, SpriteFont font) where T : IEquatable<T>
        {
            var padding = 10;
            var h = list.Sum(x => x.Item.Meassure().X) + padding;
            var w = list.Max(x => x.Item.Meassure().Y) + padding+padding;
            if (PanelTexture != null)
            {
                Panel.Draw(batch, PanelTexture, Panel.BasePanel, new Rectangle((int)pos.X, (int)pos.Y, (int)Math.Max(w,UIValues.ListMinW), (int)h));
            }

            var y = pos.Y;
            if (!list.Any())
                return;

            var itemHeight = list.First().Item.Meassure().Y;
            foreach (var item in list.Select((x, i) => (x, i)))
            {
                var c = item.x.Selected ? Color.Red : Color.White;

                var x = pos.X + padding + (item.x.Selected ? padding : 0);
                item.x.Item.Draw(batch, new Vector2(x,y));
                if (item.x.Selected)
                    item.x.Item.Draw(batch, new Vector2(x + 10, y));

                y += itemHeight;
            }
        }
    }
}
