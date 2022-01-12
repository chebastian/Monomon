using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.UI;
using Monomon.Views.Constants;
using Monomon.Views.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monomon.Views.Scenes
{
    public class ChoiceScene : SceneView
    {
        private SpriteFont _font;
        private Texture2D _sprites;
        private int _charCount;
        private float _totalTime;
        public UIList<string> List;

        public ChoiceScene(GraphicsDevice gd, List<string> choices, SpriteFont font, Texture2D sprites, Action<string> onSelection) : base(gd)
        {
            _font = font;
            _sprites = sprites;

            _charCount = 0;
            _totalTime = 0.0f;
            List = new UIList<string>(choices.Select(item => new UIItem<string>(item)).ToList(), x => { }, onSelection);
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            _totalTime += (float)time;
            //_charCount = (int)(Math.Min(1.0f, _totalTime) * message.Length);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            ListView.DrawUIList(List, new Vector2(UIValues.PlayerHudX,UIValues.PlayerHudY), _spriteBatch, _font);
            //int panelw = 350;
            //int panelx = 100;
            //int padding = 8;
            //var renderString = FitString(message, panelw - (padding * 2));
            //var height = Math.Max(_font.MeasureString(renderString).Y, 100);
            //Panel.Draw(batch, _sprites, Panel.AltPanel, new Rectangle(panelx, UIValues.BattleMessageY, panelw, (int)(height + padding)));

            //string FitString(string msg, int w)
            //{
            //    var words = msg.Substring(0, Math.Min(_charCount, msg.Length)).Split(' ').ToArray();
            //    var builder = new StringBuilder();
            //    var wordsAdded = 0;
            //    var completeString = "";
            //    while (words.Length > wordsAdded)
            //    {
            //        var nextString = $"{completeString} {words[wordsAdded]}";
            //        if (_font.MeasureString(nextString).X > w)
            //        {
            //            completeString = $"{completeString} \n {words[wordsAdded]}";
            //        }
            //        else
            //            completeString = nextString;

            //        wordsAdded++;
            //    }

            //    return completeString;
            //}

            //batch.DrawString(_font, renderString, new Vector2(panelx + padding, UIValues.BattleMessageY + padding), Color.White);
        }
    }
}
