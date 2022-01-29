using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Scenes
{
    public abstract class SceneView
    {
        protected GraphicsDevice _graphics;
        protected SpriteBatch _spriteBatch;
        protected Effect? _effect;

        public SceneView(GraphicsDevice gd)
        {
            _graphics = gd;
            _spriteBatch = new SpriteBatch(_graphics);
            _effect = null;
        }

        internal void Draw(double time)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null, _effect);
            OnDraw(_spriteBatch);
            _spriteBatch.End();
        }

        protected abstract void OnDraw(SpriteBatch batch);
        public abstract void Update(double time);
        public abstract void LoadScene(ContentManager content);
    }
}
