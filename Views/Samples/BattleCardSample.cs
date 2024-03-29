﻿using Microsoft.Xna.Framework;
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
    class BattleCardSample : SceneView
    {
        private SpriteFont? font;
        private Texture2D? _spriteMap;
        private BattleCardViewModel _card;
        private BattleCardViewModel _lowHealth;
        private BattleCardViewModel _moving;
        private SpriteCollection progressSprites;
        private Task _task;

        public BattleCardSample(GraphicsDevice gd) : base(gd)
        {
            _card = new BattleCardViewModel("Full health", 50, 50, 5);
            _lowHealth = new BattleCardViewModel("Low health", 50, 2, 5);
            _moving = new BattleCardViewModel("Moving", 50, 2, 5);

            progressSprites = new SpriteCollection(
                new Rectangle(0, 0, 8, 8),
                new Rectangle(8, 0, 8, 8),
                new Rectangle(16, 0, 8, 8)
                );

            _task = Task.Run( async () => { 
                while(true)
                {
                    _moving.SetHealth(50);
                    await Task.Delay(2000);
                    _moving.SetHealth( 25);
                    await Task.Delay(2000);
                    _moving.SetHealth(10);
                    await Task.Delay(2000);
                }
            });
        }

        public override void LoadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("File");
            _spriteMap = content.Load<Texture2D>("spritemap"); 
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            BattleCardView.Draw(batch, new Vector2(100, 100), font, _spriteMap, _card);
            BattleCardView.Draw(batch, new Vector2(100, 200), font, _spriteMap, _lowHealth);
            BattleCardView.Draw(batch, new Vector2(100, 300), font, _spriteMap, _moving); 
        }

        public override void Update(double time)
        {
            _card.Update((float)time);
            _lowHealth.Update((float)time);
            _moving.Update((float)time);
        }
    }
}
