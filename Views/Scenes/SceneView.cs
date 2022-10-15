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
        protected ContentManager _content;
        protected GraphicsDevice _graphics;
        protected Effect? _effect;

        public SceneView(GraphicsDevice gd, ContentManager content)
        {
            _content = content;
            _graphics = gd;
            _effect = null;
        }

        internal void Draw(RenderParams param)
        {
            OnDraw(param.Batch);
        }

        protected abstract void OnDraw(SpriteBatch batch);
        public abstract void Update(double time);
        public abstract void LoadScene(ContentManager content);
    }
}
