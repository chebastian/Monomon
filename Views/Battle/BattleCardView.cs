using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.ViewModels;
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
        public static void Draw(SpriteBatch batch,Vector2 pos, SpriteFont font,Texture2D spriteMap, BattleCardViewModel card)
        { 
            var color = card.IsLow() ? Color.Red : Color.Green;

            batch.DrawString(font, $"{card.Name}", new Vector2(pos.X, pos.Y), Color.White);
            ProgressbarView.Draw(batch, card.Percentage, 150, new Vector2(pos.X, pos.Y + 40), barSprites,emptyBarSprites, spriteMap, Color.White);
            batch.DrawString(font, $"HP: {(int)(card.CurrentHealth)}/{card.MaxHealth}", new Vector2(pos.X, pos.Y + 20), color);
            batch.DrawString(font, $"Lv: {card.Level}", new Vector2(pos.X, pos.Y + 60), Color.White);

            ProgressbarView.Draw(batch, card.XpPercentage, 150, new Vector2(pos.X, pos.Y + 80), barSprites, emptyBarSprites,spriteMap, Color.Black);

            batch.Draw(spriteMap, new Rectangle(card.X + card.PortraitOffsetX, card.PoirtrateAnimDelta+ card.Y + card.PortraitOffsetY, card.PortraitSrc.Width, card.PortraitSrc.Height), card.PortraitSrc, Color.White);
            if(card.Dying)
            {
                batch.Draw(spriteMap, 
                    new Rectangle(card.X + card.PortraitOffsetX, card.Y + card.PortraitOffsetY + card.PortraitSrc.Height, card.PortraitSrc.Width, card.PortraitSrc.Height), 
                    new Rectangle(8,0,1,1),
                    Color.Black);
            }
        }
    }
}
