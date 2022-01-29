using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monomon.Battle;
using Monomon.Input;
using Monomon.Mons;
using Monomon.State;
using Monomon.UI;
using Monomon.ViewModels;
using Monomon.Views.Battle;
using Monomon.Views.Constants;
using Monomon.Views.Gui;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace Monomon.Views.Samples
{
    class BattleSample : SceneView
    {
        private BattleManager _battleManager;
        private IINputHandler _input;
        private Mobmon _player;
        private Mobmon _mob;
        private StateStack<double> _stack;
        private UIList<string> _list;
        private UIList<string> fightList;
        private UIList<string> itemList;
        private UIList<string> _currentList;
        private BattleReporter _battleReporter;
        private BattleCardViewModel _currentEnemyCard;
        private BattleCardViewModel _playerCard;
        private SpriteFont? font;
        private Texture2D? _spriteMap;
        private SoundEffect _menuMoveEffect;
        private SoundEffect _menuSelectEffect;
        private SoundEffect _battleTackleEffect;
        private SoundEffect _battleHurtEffect;
        private SoundEffect _battleXpUpEffect;

        public BattleSample(GraphicsDevice gd, IINputHandler input, StateStack<double> stack) : base(gd)
        {
            _input = input;
            _player = new Mobmon("Player", 20, new MonStatus(4, 2, 3));
            _mob = new Mobmon("Mob", 9, new MonStatus(2, 2, 3));
            _stack = stack;

            _list = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Fight", x => { _currentList = fightList; }),
                new UIItem<string>("Item", x => { _currentList = itemList;}),
                new UIItem<string>("Mon"),
                new UIItem<string>("Run"),
            }, x =>
            {

                OnMenuMove();
            }, x =>
            {
                //TODO can we remove this callback?
                OnMenuSelect();
            });

            fightList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Tackle", x => {
                    _battleManager.Attack(new AttackCommand(AttackType.Tackle, _player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Slash", x => {
                    _battleManager.Attack(new Slash(_player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Swipe", x => {
                    _battleManager.Attack(new Swipe( _player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Growl", x => {}),
            }, x =>
            {
                OnMenuMove();
            }, x =>
            {
                OnMenuSelect();
            });

            itemList = new UIList<string>(new List<UIItem<string>>() {
                new UIItem<string>("Potion", x => {}),
                new UIItem<string>("Mana Potion", x => {}),
                new UIItem<string>("Back", x => {_currentList = _list; }),
            }, x =>
            {
                OnMenuMove();
            },
        x =>
        {
            OnMenuSelect();
        });

            _currentList = _list;

        }
        private void InitBattle()
        {
            _currentEnemyCard = new BattleCardViewModel(_mob.Name, _mob.MaxHealth, _mob.Health, 2);
            _currentEnemyCard.X = UIValues.OponentHudX;
            _currentEnemyCard.Y = 10;
            _currentEnemyCard.PortraitSrc = new Rectangle(0, 90, 72, 112);
            _currentEnemyCard.PortraitOffsetX = 240;
            _currentEnemyCard.PortraitOffsetY = 32;

            _playerCard = new BattleCardViewModel("Player", _player.MaxHealth, _player.Health, 6);
            _playerCard.X = UIValues.PlayerHudX;
            _playerCard.Y = UIValues.PlayerHudY;
            _playerCard.PortraitSrc = new Rectangle(72, 90, 96, 128);
            _playerCard.PortraitOffsetX = -112;
            _playerCard.PortraitOffsetY = -32;

            _battleReporter = new BattleReporter(_spriteBatch, _graphics, _stack, _input, font, _spriteMap, OnPlaySound);

            _battleManager = new BattleManager(_player, _mob, _battleReporter, _input, _playerCard, _currentEnemyCard);

            _battleManager.Start();
        }

        public override void LoadScene(ContentManager content)
        {

            font = content.Load<SpriteFont>("File");
            _spriteMap = content.Load<Texture2D>("spritemap");

            InitBattle();

            _menuMoveEffect = content.Load<SoundEffect>("menuSelectSimple");
            _menuSelectEffect = content.Load<SoundEffect>("menuMoveChirpy");

            _battleTackleEffect = content.Load<SoundEffect>("tackle");
            _battleHurtEffect = content.Load<SoundEffect>("hurtChirpy");
            _battleXpUpEffect = content.Load<SoundEffect>("XpUp");
        }

        private void OnPlaySound(Sounds sound)
        {
            var instance = sound switch
            {
                Sounds.Attack_Tackle => _battleTackleEffect.CreateInstance(),
                Sounds.TakeDamage => _battleHurtEffect.CreateInstance(),
                Sounds.XpUP => _battleXpUpEffect.CreateInstance(),
                _ => _battleTackleEffect.CreateInstance(),
            };

            instance.Play();
        }

        public void OnMenuMove()
        {
            _menuMoveEffect?.CreateInstance().Play();
        }
        public void OnMenuSelect()
        {
            _menuSelectEffect?.CreateInstance().Play();
        }

        public override void Update(double time)
        {
            if (_battleManager.IsPlayerTurn() && _battleManager.IsInteractive())
            {
                if (_input.IsKeyPressed(KeyName.Down))
                    _currentList.SelectNext();
                if (_input.IsKeyPressed(KeyName.Up))
                    _currentList.SelectPrevious();
                if (_input.IsKeyPressed(KeyName.Back))
                {
                    _currentList = _list;
                }

                if (_input.IsKeyPressed(KeyName.Select))
                {
                    _currentList.Select();
                }
            }

            if (_battleManager.BattleOver())
            {
                var result = _battleManager.GetOutcome();

                if (result == BattleOutcome.Win)
                {
                    var yesChoice = new Choice("Yes", () =>
                    {
                        _mob = new Mobmon("Mon2", 8, new MonStatus(5, 5, 5));
                        _currentEnemyCard = new BattleCardViewModel(_mob.Name, _mob.MaxHealth, _mob.Health, 5);
                        InitBattle();
                        _stack.Pop();
                    });
                    var no = new Choice("No", () =>
                    {
                        _stack.Pop();
                        _stack.Pop();
                    });
                    SelectChoice("Do you want to continue?", yesChoice, no);
                }
            }

            UpdateBattleCard(_mob, _currentEnemyCard, (float)time);
            UpdateBattleCard(_player, _playerCard, (float)time);
        }

        public record Choice(string name, Action action);
        private void SelectChoice(string message, params Choice[] choices)
        {
            if (choices.Select(x => x.name).Distinct().ToList().Count != choices.Length)
                throw new ArgumentException("Choices must be unique");

            _stack.Push(
                new TimedState(
                    new MessageScene(_graphics, message, font, _spriteMap),
                    1000,
                    _input),
                () =>
                {
                    _stack.Push(
                        new ConfirmState(
                            new ChoiceScene(_graphics, choices.Select(x => x.name).ToList(), font, _spriteMap, selection =>
                            {
                                var choosen = choices.Where(item => item.name == selection).FirstOrDefault();
                                if (choosen != null)
                                    choosen.action();
                            }),
                            _input),
                        () =>
                        {
                            _stack.Pop(); // pop the timed state
                        });
                });

        }

        private void SelectChoice(string message, List<string> choices, Action<string> onSelected)
        {
            _stack.Push(
                new TimedState(
                    new MessageScene(_graphics, message, font, _spriteMap),
                    1000,
                    _input),
                () =>
                {
                    _stack.Push(
                        new ConfirmState(
                            new ChoiceScene(_graphics, choices, font, _spriteMap, onSelected),
                            _input),
                        () =>
                        {
                            _stack.Pop(); // stack the timed state
                        });
                });

        }

        private void UpdateBattleCard(Mobmon mob, BattleCardViewModel card, float t)
        {
            card.CurrentHealth = mob.Health;
            card.Update(t);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            DrawBattle();
        }

        private void DrawBattleResult(BattleOutcome battleOutcome)
        {
            var outcomeString = battleOutcome
                == BattleOutcome.Win ? "You win!" : "You lose";
            _spriteBatch.DrawString(font, outcomeString, new Vector2(10, 10), Color.White);
        }

        private void DrawBattleLog()
        {
            if (_battleReporter.Messages.Any())
                _spriteBatch.DrawString(font, _battleReporter.Messages.Last(), new Vector2(200, 100), Color.White);
        }

        private void DrawBattle()
        {
            _currentEnemyCard.SetHealth(_mob.Health);
            _playerCard.SetHealth(_player.Health);
            _playerCard.SetXp(_player.Xp);


            ListView.DrawUIList(_currentList, new Vector2(UIValues.PlayerHudX, UIValues.PlayerHudY + 100), _spriteBatch, font);
            BattleCardView.Draw(_spriteBatch, new Vector2(_currentEnemyCard.X, _currentEnemyCard.Y), font, _spriteMap, _currentEnemyCard);
            BattleCardView.Draw(_spriteBatch, new Vector2(_playerCard.X, _playerCard.Y), font, _spriteMap, _playerCard);



            //DrawBattleLog();
        }

    }
}
