using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Views.Constants;

namespace Monomon.Views.Scenes
{
    class MessageScene : SceneView
    {
        private readonly string message;
        private SpriteFont _font;

        public MessageScene(GraphicsDevice gd, string message, SpriteFont font) : base(gd)
        {
            this.message = message;
            _font = font;
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            batch.DrawString(_font,message,new Vector2(100,UIValues.BattleMessageY), Color.White);
        }
    }
}
