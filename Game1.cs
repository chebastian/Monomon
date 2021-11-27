using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monomon.Input;
using Monomon.Mons;
using Monomon.UI;
using Monomon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monomon
{
    public class Turn
    {
        private readonly Action t;

        public Turn(Action t)
        {
            this.t = t;
            Complete = false;
        }

        public Task Execute()
        {
            return Task.Run(() => t());
        }

        public bool Complete { get; set; }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
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
        private Turn _enemyTurn;
        private string _selection;
        private Color _clearColor;
        private Turn _currentTurn;
        private BattleCardViewModel _currentEnemyCard;
        private BattleCardViewModel _playerCard;

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
            _currentTurn = new Turn(() => { });

            _list = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Fight", x => { _currentList = fightList; }),
                new UIItem<string>("Item", x => { _currentList = itemList;}),
                new UIItem<string>("Mon"),
                new UIItem<string>("Run"),
            }, x => { }, OnItemChanged);

            fightList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Tackle", x => {
                    _currentTurn = new Turn(() => { 
                        _mob.Health -= 1;
                    });
                    _currentTurn.Complete = true;
                    _currentList = _list;
                }),
                new UIItem<string>("Growl", x => {}),
            }, x => { }, OnFightItemSelected);

            itemList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Potion", x => {}),
                new UIItem<string>("Mana Potion", x => {}),
                new UIItem<string>("Back", x => {_currentList = _list; }),
            }, x => { }, OnItemSelected);

            //itemList = new UIList<string>(new List<string>() { 
            //    "potion",
            //    "mana-potion",
            //    "ball",
            //}, x => { }, OnFightItemSelected);

            _currentList = _list;
            _mob = new Mobmon("First Mob", 5);
            _player = new Mobmon("Player", 25);

            _rand = new Random();
            _enemyTurn = new Turn(() => {
                Task.Run(async () => {
                    await Task.Delay(500);
                    _player.Health -= _rand.Next(5);
                    _enemyTurn.Complete = true;
                });
            });
            _enemyTurn.Complete = true;

            _currentEnemyCard = new BattleCardViewModel(_mob.Name, _mob.MaxHealth, _mob.Health, 2);
            _playerCard = new BattleCardViewModel("Player", 25, 25, 6);

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

            if (_currentTurn.Complete)
            {
                _currentTurn.Execute();
                _currentTurn = NextTurn();
            }

            // TODO: Add your update logic here

            UpdateBattleCard(_mob, _currentEnemyCard);
            UpdateBattleCard(_player, _playerCard);
            base.Update(gameTime);
        }

        int turnNum = 0;
        private Turn NextTurn()
        {
            turnNum++;
            if (turnNum % 2 == 0)
                return new Turn(() => { });
            else
                return _enemyTurn;
        }

        private void UpdateBattleCard(Mobmon mob, BattleCardViewModel card)
        {
            card.CurrentHealth = mob.Health;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            DrawUIList(_currentList, new Vector2(200, 200));
            DrawBattlecard(_currentEnemyCard, new Vector2(30,20));
            DrawBattlecard(_playerCard, new Vector2(30,200));
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawBattlecard(BattleCardViewModel card, Vector2 pos)
        {
            var color = card.IsLow() ? Color.Red : Color.White;
            float percentage = (float)card.CurrentHealth / (float)card.MaxHealth;
            float healthbarW = 16;

            _spriteBatch.DrawString(font, $"{card.Name}", new Vector2(pos.X, pos.Y), Color.White);
            _spriteBatch.DrawString(font, $"{string.Join(null,Enumerable.Repeat("#",(int)(percentage*healthbarW)))}", new Vector2(pos.X, pos.Y + 20), color);
            _spriteBatch.DrawString(font, $"HP: {card.CurrentHealth}/{card.MaxHealth}", new Vector2(pos.X, pos.Y + 35), color);
            _spriteBatch.DrawString(font, $"Lv: {card.Level}", new Vector2(pos.X,pos.Y + 50), Color.White);
        }
    }
}
