using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.ViewModels;
using Monomon.Views.Battle;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Views.Samples
{
    public class TweenSamples : SceneView
    {
        private Action _reset;
        private BattleCardViewModel _card;
        private TweenState _first;
        private TweenState _second;
        private TweenState _third;
        private SpriteFont font;
        private Texture2D sprites;
        private float speed = 1.5f;
        private BattleCardViewModel _card2;
        private BattleCardViewModel _card3;

        public TweenSamples(GraphicsDevice gd) : base(gd)
        {

        }

        public override void LoadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("File");
            sprites = content.Load<Texture2D>("spritemap");
            _card = new BattleCardViewModel("", 5, 2, 2);
            _card.X = 250;
            _card.Y = 10;
            _card.PortraitSrc = new Rectangle(0, 90, 72, 112);
            _card.PortraitOffsetX = 240;
            _card.PortraitOffsetY = 32;

            _card2 = new BattleCardViewModel("", 5, 2, 2);
            _card2.X = 250;
            _card2.Y = 100;
            _card2.PortraitSrc = new Rectangle(0, 90, 72, 112);
            _card2.PortraitOffsetX = 240;
            _card2.PortraitOffsetY = 32;

            _card3 = new BattleCardViewModel("", 5, 2, 2);
            _card3.X = 250;
            _card3.Y = 200;
            _card3.PortraitSrc = new Rectangle(0, 90, 72, 112);
            _card3.PortraitOffsetX = 240;
            _card3.PortraitOffsetY = 32;

            _first = new TweenState(x => UpdateCard(_card,x.time,x.lerp), OnReset, 0.0f, 50.0f, speed, EasingFunc.Lerp);
            _second = new TweenState(x => UpdateCard(_card2,x.time,x.lerp), OnReset, 0.0f, 50.0f, speed, EasingFunc.EaseOutCube);
            _third = new TweenState(x => UpdateCard(_card3,x.time,x.lerp), OnReset, 0.0f, 50.0f, speed, EasingFunc.EaseOutBack);
        }
        private void OnReset()
        {
            _first = new TweenState(x => UpdateCard(_card,x.time,x.lerp), OnReset, 0.0f, 50.0f, speed, EasingFunc.EaseInBack);
            _second = new TweenState(x => UpdateCard(_card2,x.time,x.lerp), OnReset, 0.0f, 50.0f, speed, EasingFunc.EaseOutCube);
            _third = new TweenState(x => UpdateCard(_card3,x.time,x.lerp), OnReset, 0.0f, 50.0f, speed, EasingFunc.EaseOutBack);
        }

        private void UpdateCard(BattleCardViewModel vm, double time, double lerp)
        {
            vm.PortraitOffsetX = (int)(lerp);
        }

        private void OnTweenUpdate((double time, double lerp) obj)
        {
        }


        public override void Update(double time)
        {
            _first.Update((float)time);;
            _second.Update((float)time);;
            _third.Update((float)time);;
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            BattleCardView.Draw(batch, new Microsoft.Xna.Framework.Vector2(0, 0), font, sprites, _card);
            BattleCardView.Draw(batch, new Microsoft.Xna.Framework.Vector2(0, 0), font, sprites, _card2);
            BattleCardView.Draw(batch, new Microsoft.Xna.Framework.Vector2(0, 0), font, sprites, _card3);

            Gui.Panel.Draw(batch, sprites, Gui.Panel.BasePanel, new Rectangle(300+60, 10, 2, 500));
        }
    }
}
