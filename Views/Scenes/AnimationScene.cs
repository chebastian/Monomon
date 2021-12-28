using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Animation;
using System;
using System.Collections.Generic;

namespace Monomon.Views.Scenes
{
    class AnimationScene : SceneView
    {
        private Vector2 _pos;
        private Texture2D _tex;
        private AnimationPlayer player;

        public AnimationScene(GraphicsDevice gd, Vector2 pos, Texture2D tex, List<Frame> anim) : base(gd)
        {
            _pos = pos;
            _tex = tex;
            player = new AnimationPlayer(1);
            player.ChangeAnimation(anim);
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            player.Update((float)time);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            var frame = player.CurrentFrame();
            batch.Draw(_tex, _pos,new Rectangle(frame.Source.x,frame.Source.y,24,24), Color.White);
        }
    }

    class TestScene : SceneView
    {
        private readonly Action<double> update;
        private readonly Action<SpriteBatch> render;

        public TestScene(GraphicsDevice gd,Action<double> update, Action<SpriteBatch> render) : base(gd)
        {
            this.update = update;
            this.render = render;
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            update(time);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            render(batch);
        }
    }
}
