using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Monomon.Views.Scenes
{
    class MessageScene : SceneView
    {
        private readonly string message;
        private SpriteFont font;

        public MessageScene(GraphicsDevice gd, string message) : base(gd)
        {
            this.message = message;
        }

        public override void LoadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("File");
        }

        public override void Update(double time)
        {
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            batch.DrawString(font,message,new Vector2(100,100), Color.White);
        }
    }
}
