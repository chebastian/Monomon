using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Mons;
using Monomon.Views.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Gui
{
    public class MonSummaryView
    {
        public static void Draw(SpriteBatch batch, SpriteFont font, Texture2D spriteMap, List<Mobmon> mons)
        {
            Panel.Draw(batch, spriteMap, Panel.BasePanel, new Rectangle(0, 0, 120, 40));

            var drawPosX = 10;
            Panel.Stack(mons, (item, y) =>
            {
                var healthStr = $"{item.Health}/{item.MaxHealth}".PadRight(8);
                var strW = 28;
                batch.DrawString(font, healthStr, new Vector2(drawPosX, y), Color.White);
                ProgressbarView.Draw(batch, item.HealthPercentage, 40, new Vector2(drawPosX + strW, y), BattleCardView.miniBarSprites, BattleCardView.miniEmptyBarSprites, spriteMap, Color.White);
            }, 14, 8);
        }

    }
}
