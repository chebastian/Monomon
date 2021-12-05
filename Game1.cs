using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monomon.Battle;
using Monomon.Input;
using Monomon.Mons;
using Monomon.UI;
using Monomon.ViewModels;
using Monomon.Views;
using Monomon.Views.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monomon
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private BattleManager _battleManager;
        private BufferInputHandler _input;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private UIList<string> _list;
        private UIList<string> fightList;
        private UIList<string> itemList;
        private UIList<string> _currentList;
        private Mobmon _mob;
        private Mobmon _player;
        private Random _rand;
        private BattleReporter _battleReporter;
        private string _selection;
        private Color _clearColor;
        private Texture2D _spriteMap;
        private BattleCardViewModel _currentEnemyCard;
        private BattleCardViewModel _playerCard;
        private SpriteCollection progressSprites;
        private SceneView _currentScene;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _input = new Monomon.Input.BufferInputHandler();
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("File");
            _clearColor = Color.SkyBlue;

            _spriteMap = Content.Load<Texture2D>("spritemap");

            _list = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Fight", x => { _currentList = fightList; }),
                new UIItem<string>("Item", x => { _currentList = itemList;}),
                new UIItem<string>("Mon"),
                new UIItem<string>("Run"),
            }, x => { }, OnItemChanged);

            fightList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Tackle", x => {
                    _battleManager.Attack(new AttackCommand(AttackType.Tackle, _player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Swipe", x => {
                    _battleManager.Attack(new AttackCommand(AttackType.Slash, _player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Wrap", x => {
                    _battleManager.Attack(new AttackCommand(AttackType.Wrap, _player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Growl", x => {}),
            }, x => { }, OnFightItemSelected);

            itemList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Potion", x => {}),
                new UIItem<string>("Mana Potion", x => {}),
                new UIItem<string>("Back", x => {_currentList = _list; }),
            }, x => { }, OnItemSelected);

            _currentList = _list;
            _player = new Mobmon("Player", 15, new MonStatus(4, 2, 3));
            _mob = new Mobmon("Mob", 25, new MonStatus(2, 2, 3));
            _rand = new Random();

            _battleReporter = new BattleReporter(_spriteBatch);
            _battleManager = new BattleManager(_player, _mob, _battleReporter, _input);
            _battleManager.Start();

            _currentEnemyCard = new BattleCardViewModel(_mob.Name, _mob.MaxHealth, _mob.Health, 2);
            _playerCard = new BattleCardViewModel("Player", _player.MaxHealth, _player.Health, 6);

            progressSprites = new SpriteCollection(
                new Rectangle(0, 0, 8, 8),
                new Rectangle(8, 0, 8, 8),
                new Rectangle(16, 0, 8, 8)
                );

            _currentScene = new BattleCardSample(GraphicsDevice);
            _currentScene.LoadScene(Content);

            base.Initialize();
        }

        private void OnItemSelected(string obj)
        {
        }

        private void OnFightItemSelected(string obj)
        {
        }

        public void DrawUIList<T>(UIList<T> list, Vector2 pos) where T : IEquatable<T>
        {
            var y = pos.Y;
            foreach (var item in list.Items.Select((x, i) => (x, i)))
            {
                var c = item.x.Selected ? Color.Red : Color.White;
                c = item.x.Item.Equals(_selection) ? Color.Green : c;

                _spriteBatch.DrawString(font, item.x.Item.ToString(), new Vector2(pos.X, y), c);
                y += 20;
            }
        }

        private void OnItemChanged(string obj)
        {
            _selection = obj;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _input.Update(gameTime);
            var pad = GamePad.GetState(PlayerIndex.One);

            if (_battleManager.IsPlayerTurn() && _battleManager.IsInteractive())
            {
                if (_input.IsKeyPressed(Keys.Down))
                    _currentList.SelectNext();
                if (_input.IsKeyPressed(Keys.Up))
                    _currentList.SelectPrevious();
                if (_input.IsKeyPressed(Keys.A))
                {
                    _currentList = _list;
                }

                if (_input.IsKeyPressed(Keys.Space))
                {
                    _currentList.Select();
                }
            }

            if (_battleManager.TurnIsDone())
            {
                _battleManager.NextTurn();
            }

            _currentScene.Update(gameTime);

            UpdateBattleCard(_mob, _currentEnemyCard, (float)gameTime.ElapsedGameTime.TotalSeconds);
            UpdateBattleCard(_player, _playerCard, (float)gameTime.ElapsedGameTime.TotalSeconds);


            base.Update(gameTime);
        }


        private void UpdateBattleCard(Mobmon mob, BattleCardViewModel card, float t)
        {
            card.CurrentHealth = mob.Health;
            card.Update(t);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _currentScene.Draw(gameTime);

            //if (!_battleManager.BattleOver())
            //{
            //    DrawBattle();
            //}
            //else
            //{
            //    DrawBattleResult(_battleManager.GetOutcome());
            //}

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawBattleResult(BattleOutcome battleOutcome)
        {
            var outcomeString = battleOutcome
                == BattleOutcome.Win ? "You win!" : "You lose";
            _spriteBatch.DrawString(font, outcomeString, new Vector2(10, 10), Color.White);
        }

        private void DrawBattleLog()
        {
            //var list = _battleReporter.Messages.Select(x => new UIItem<string>(x)).Reverse().ToList();
            //    DrawUIList<string>(new UIList<string>(list,x => { }, y => { }), new Vector2(200,100));
            if (_battleReporter.Messages.Any())
                _spriteBatch.DrawString(font, _battleReporter.Messages.Last(), new Vector2(200, 100), Color.White);
        }

        private void DrawBattle()
        {
            DrawUIList(_currentList, new Vector2(200, 200));
            DrawBattlecard(_currentEnemyCard, new Vector2(30, 20));
            DrawBattlecard(_playerCard, new Vector2(30, 200));


            DrawBattleLog();
        }

        private void DrawBattlecard(BattleCardViewModel card, Vector2 pos)
        {
            var color = card.IsLow() ? Color.Red : Color.White;
            float percentage = card.CurrentHealth > 0 ? (float)card.CurrentHealth / (float)card.MaxHealth : 0.01f;

            _spriteBatch.DrawString(font, $"{card.Name}", new Vector2(pos.X, pos.Y), Color.White);
            ProgressbarView.Draw(_spriteBatch, card.Percentage, 150, new Vector2(pos.X, pos.Y + 20), progressSprites, _spriteMap, color);
            _spriteBatch.DrawString(font, $"HP: {card.CurrentHealth}/{card.MaxHealth}", new Vector2(pos.X, pos.Y + 35), color);
            _spriteBatch.DrawString(font, $"Lv: {card.Level}", new Vector2(pos.X, pos.Y + 50), Color.White);
        }
    }
}
