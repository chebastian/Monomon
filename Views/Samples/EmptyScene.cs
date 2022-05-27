using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Samples
{
    class EmptyScene : SceneView
    {

        public EmptyScene(GraphicsDevice gd) : base(gd)
        {
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
        }

        protected override void OnDraw(SpriteBatch batch)
        {
        }
    }
}
