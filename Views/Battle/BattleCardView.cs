using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.ViewModels;
using Monomon.Views.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Battle
{
    public class BattleCardView
    {
        private static SpriteCollection barSprites = new SpriteCollection(
                new Rectangle(0, 0, 8, 8),
                new Rectangle(8, 0, 8, 8),
                new Rectangle(16, 0, 8, 8)
                );

        private static SpriteCollection emptyBarSprites = new SpriteCollection(
                new Rectangle(0, 8, 8, 8),
                new Rectangle(8, 8, 8, 8),
                new Rectangle(16, 8, 8, 8)
                );

        private static int w = 4;
        private static int h = 5;
        private static SpriteCollection smallBarSprites = new SpriteCollection(
                new Rectangle(32, 0, w, h),
                new Rectangle(32 + w, 0, w, h),
                new Rectangle(48 - w, 0, w, h)
                );

        private static SpriteCollection smallEmptyBarSprites = new SpriteCollection(
                new Rectangle(48, 0, w, h),
                new Rectangle(48 + w, 0, w, h),
                new Rectangle(64 - w, 0, w, h)
                );

        public static void Stack(List<Action<int>> drawCalls,int offset, int spaceing)
        {
            var y = offset;
            foreach (var call in drawCalls)
            {
                call(y);
                y += spaceing;
            }
        }

        public static void DrawTopCard(SpriteBatch batch, Vector2 pos, SpriteFont font, Texture2D spriteMap, BattleCardViewModel card)
        {
            var textHeight = font.MeasureString("H");
            var color = card.IsLow() ? Color.Red : Color.Green;

            int progressWidth = UIValues.TileSz * 4;

            batch.Draw(spriteMap,
                       new Rectangle(card.X + card.PortraitOffsetX, card.PoirtrateAnimDelta + card.Y + card.PortraitOffsetY, card.PortraitSrc.Width, card.PortraitSrc.Height),
                       card.PortraitSrc,
                       Color.White);

            if (card.Dying)
            {
                batch.Draw(spriteMap,
                    new Rectangle(card.X + card.PortraitOffsetX,
                                  card.Y + card.PortraitOffsetY + card.PortraitSrc.Height,
                                  card.PortraitSrc.Width,
                                  card.PortraitSrc.Height),
                    new Rectangle(0, 16, 1, 1), // Black pixel
                    Color.White);
            }

            Stack(new List<Action<int>>() {
                py => batch.DrawString(font, $"{card.Name}", new Vector2(pos.X, py + pos.Y), Color.White),
                py => ProgressbarView.Draw(batch, card.Percentage, progressWidth, new Vector2(pos.X, pos.Y + py), smallBarSprites, smallEmptyBarSprites, spriteMap, Color.White),
                py => batch.DrawString(font, $"{(int)(card.CurrentHealth)}/{card.MaxHealth}", new Vector2(pos.X, pos.Y + py), Color.White)

            },0,12);

        }
        public static void Draw(SpriteBatch batch, Vector2 pos, SpriteFont font, Texture2D spriteMap, BattleCardViewModel card)
        {
            var textHeight = font.MeasureString("H");
            var color = card.IsLow() ? Color.Red : Color.Green;

            int progressWidth = UIValues.TileSz * 4;

            batch.Draw(spriteMap,
                       new Rectangle(card.X + card.PortraitOffsetX, card.PoirtrateAnimDelta + card.Y + card.PortraitOffsetY, card.PortraitSrc.Width, card.PortraitSrc.Height),
                       card.PortraitSrc,
                       Color.White);

            Stack(new List<Action<int>>() { 
                //py => batch.DrawString(font, $"{card.Name}", new Vector2(pos.X, py + pos.Y), Color.White),
                py => ProgressbarView.Draw(batch, card.Percentage, progressWidth, new Vector2(pos.X, pos.Y + py), smallBarSprites, smallEmptyBarSprites, spriteMap, Color.White),
                py => batch.DrawString(font, $"{(int)(card.CurrentHealth)}/{card.MaxHealth}", new Vector2(pos.X, pos.Y + py), Color.White),
                py => batch.DrawString(font, $"Lv: {card.Level}", new Vector2(pos.X, pos.Y + py), Color.White),
                py => ProgressbarView.Draw(batch, card.XpPercentage, progressWidth, new Vector2(pos.X, pos.Y + py), smallBarSprites, smallEmptyBarSprites, spriteMap, Color.Black)

            },0,10);

            if (card.Dying)
            {
                batch.Draw(spriteMap,
                    new Rectangle(card.X + card.PortraitOffsetX,
                                  card.Y + card.PortraitOffsetY + card.PortraitSrc.Height,
                                  card.PortraitSrc.Width,
                                  card.PortraitSrc.Height),
                    new Rectangle(0, 16, 1, 1),
                    Color.White);
            }
        }
    }
}
