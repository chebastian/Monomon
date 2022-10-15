using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Views.Constants;
using Monomon.Views.Gui;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Scenes
{
    class MessageScene : SceneView
    {
        private readonly bool _confirm;
        private readonly string message;
        private SpriteFont _font;
        private Texture2D _sprites;
        private int _charCount;
        private float _totalTime;

        public MessageScene(GraphicsDevice gd, string message, SpriteFont font, Texture2D sprites,ContentManager mgr, bool confirm = false) : base(gd,mgr)
        {
            _confirm = confirm;
            this.message = message;
            if (confirm)
                this.message += " >>";
            _font = font;
            _sprites = sprites;

            _charCount = 0;
            _totalTime = 0.0f;
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            _totalTime += (float)time;
            _charCount = (int)(Math.Min(1.0f, _totalTime) * message.Length);
        }

        public bool IsCompleted => _totalTime >= 1.0f;

        protected override void OnDraw(SpriteBatch batch)
        {
            if (_confirm && _totalTime <= 0) // Do not render untill we are updated... i think
                return;

            var meassured = false;
            if (meassured)
            {
                var sz =_font.MeasureString(message);
                Panel.Draw(batch, _sprites, Panel.BasePanel, new Rectangle(100, UIValues.BattleMessageY, (int)(sz.X+40), (int)(sz.Y+20))); 
                batch.DrawString(_font,message,new Vector2(120,UIValues.BattleMessageY+10), Color.White);
            }
            else
            {
                int panelw = UIValues.PortraitW;
                int panelx = 0;
                int padding = 8;
                var renderString = FitString(message, panelw - (padding * 2));
                var height = Math.Min(_font.MeasureString(renderString).Y, 100);
                Panel.Draw(batch, _sprites, Panel.BasePanel, new Rectangle(panelx, UIValues.BattleMessageY, panelw, (int)(height + padding)));

                string FitString(string msg, int w)
                {
                    var words = msg.Substring(0,Math.Min(_charCount,msg.Length)).Split(' ').ToArray();
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
                //if (_confirm)
                //    batch.DrawString(_font, "X", new Vector2(panelx + panelw - padding - padding, UIValues.BattleMessageY + height - padding), Color.Red);
            }
        }
    }
}
