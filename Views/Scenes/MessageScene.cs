using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Views.Constants;
using Monomon.Views.Gui;
using System;
using System.Text;

namespace Monomon.Views.Scenes
{
    class MessageScene : SceneView
    {
        private readonly bool _confirm;
        private readonly string message;
        private SpriteFont _font;
        private Texture2D _sprites;

        public MessageScene(GraphicsDevice gd, string message, SpriteFont font,Texture2D sprites, bool confirm = false) : base(gd)
        {
            _confirm = confirm;
            this.message = message;
            _font = font;
            _sprites = sprites;
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            var meassured = false;
            if (meassured)
            {
                var sz =_font.MeasureString(message);
                Panel.Draw(batch, _sprites, Panel.BasePanel, new Rectangle(100, UIValues.BattleMessageY, (int)(sz.X+40), (int)(sz.Y+20))); 
                batch.DrawString(_font,message,new Vector2(120,UIValues.BattleMessageY+10), Color.White);
            }
            else
            {
                int panelw = 350;
                int panelx = 100;
                int padding = 8;
                var renderString = FitString(message, panelw - (padding * 2));
                var height = Math.Max(_font.MeasureString(renderString).Y, 100);
                Panel.Draw(batch, _sprites, Panel.AltPanel, new Rectangle(panelx, UIValues.BattleMessageY, panelw, (int)(height + padding)));

                string FitString(string msg, int w)
                {
                    var words = msg.Split(' ');
                    var builder = new StringBuilder();
                    var wordsAdded = 0;
                    var completeString = "";
                    while(words.Length > wordsAdded)
                    {
                        var nextString = $"{completeString} {words[wordsAdded]}";
                        if (_font.MeasureString(nextString).X > w)
                        {
                            completeString = $"{completeString} \n {words[wordsAdded]}";
                        }
                        else 
                            completeString = nextString;

                        wordsAdded++;
                    }

                    return completeString;
                }

                batch.DrawString(_font,renderString,new Vector2(panelx+padding,UIValues.BattleMessageY+padding), Color.White);
                if(_confirm)
                    batch.DrawString(_font,"X",new Vector2(panelx+panelw-padding-padding,UIValues.BattleMessageY+height-padding), Color.Red);
            }
        }
    }
}
