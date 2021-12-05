using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views
{
    abstract class SceneView
    {
        private GraphicsDevice _graphics;
        protected SpriteBatch _spriteBatch;

        public SceneView(GraphicsDevice gd)
        {
            _graphics = gd;
            _spriteBatch = new SpriteBatch(_graphics);
        }

        internal void Draw(GameTime time)
        {
            _spriteBatch.Begin();
            OnDraw(_spriteBatch);
            _spriteBatch.End();
        }

        protected abstract void OnDraw(SpriteBatch batch);
        public abstract void Update(GameTime time);
        public abstract void LoadScene(ContentManager content);
    }
}
