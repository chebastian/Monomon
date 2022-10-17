using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Views.Constants;
using Monomon.Views.Gui;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Scenes
{
    class MessageScene : SceneView
    {
        private readonly bool _confirm;
        private readonly string _message;
        private readonly float _panelH;
        private SpriteFont _font;
        private Texture2D _sprites;
        private int _charCount;
        private float _totalTime;
        private int _padding;

        public MessageScene(GraphicsDevice gd, string message, SpriteFont font, Texture2D sprites, ContentManager mgr, bool confirm = false) : base(gd, mgr)
        {
            _confirm = confirm;
            _message = message;
            if (confirm)
                _message += " >>";
            _font = font;
            _sprites = sprites;

            _charCount = 0;
            _totalTime = 0.0f;

            _padding = 8;
            _message = FitString(_message, message.Length, panelw - _padding * 2, _font);
            _panelH = Math.Max(_font.MeasureString(_message).Y,30);
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            _totalTime += (float)time;
            _charCount = (int)(Math.Min(1.0f, _totalTime) * _message.Length);
        }

        public bool IsCompleted => _totalTime >= 1.0f;

        static string FitString(string msg, int charCount, int maxWidth, SpriteFont font)
        {
            var words = msg.Substring(0, Math.Min(charCount, msg.Length)).Split(' ').ToArray();
            var builder = new StringBuilder();
            var wordsAdded = 0;
            var completeString = "";
            while (words.Length > wordsAdded)
            {
                var nextString = $"{completeString} {words[wordsAdded]}".TrimStart();
                if (font.MeasureString(nextString).X > maxWidth)
                {
                    completeString = $"{completeString} \n {words[wordsAdded]}";
                }
                else
                    completeString = nextString;

                wordsAdded++;
            }

            return completeString;
        }
        int panelw = UIValues.PortraitW;

        protected override void OnDraw(SpriteBatch batch)
        {
            if (_confirm && _totalTime <= 0) // Do not render untill we are updated... i think
                return;

            var meassured = false;
            if (meassured)
            {
                var sz = _font.MeasureString(_message);
                Panel.Draw(batch, _sprites, Panel.BasePanel, new Rectangle(100, UIValues.BattleMessageY, (int)(sz.X + 40), (int)(sz.Y + 20)));
                batch.DrawString(_font, _message, new Vector2(120, UIValues.BattleMessageY + 10), Color.White);
            }
            else
            {
                int panelx = 0;
                var renderString = _message.Substring(0, _charCount);
                Panel.Draw(batch, _sprites, Panel.BasePanel, new Rectangle(panelx, UIValues.BattleMessageY, panelw, (int)(_panelH + _padding)));


                batch.DrawString(_font, renderString, new Vector2(panelx + _padding, UIValues.BattleMessageY + _padding), Color.White);
            }
        }
    }
}
