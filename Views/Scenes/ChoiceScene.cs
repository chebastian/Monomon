﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.UI;
using Monomon.Views.Constants;
using Monomon.Views.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monomon.Views.Scenes
{
    public class ChoiceScene : SceneView
    {
        private SpriteFont _font;
        private Texture2D _sprites;
        private float _totalTime;
        public UIList<string> List;

        public ChoiceScene(GraphicsDevice gd, List<string> choices, SpriteFont font, Texture2D sprites, Action<string> onSelection,ContentManager mgr) : base(gd,mgr)
        {
            _font = font;
            _sprites = sprites;

            _totalTime = 0.0f;
            List = new UIList<string>(choices.Select(item => new UIItem<string>(item)).ToList(), x => { }, onSelection);
        }

        public override void LoadScene(ContentManager content)
        {
        }

        public override void Update(double time)
        {
            _totalTime += (float)time;
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            ListView.DrawUIList(List, new Vector2(UIValues.PlayerHudX-50,UIValues.PlayerHudY+100), batch, _font);
        }
    }
}
