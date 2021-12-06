using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Samples
{
    class EmptyScene : SceneView
    {
        private SpriteFont font;

        public EmptyScene(GraphicsDevice gd) : base(gd)
        {
        }

        public override void LoadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("File");
        }

        public override void Update(GameTime time)
        {
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            batch.DrawString(font,"Empty",new Vector2(100,100), Color.White);
        }
    }
}
