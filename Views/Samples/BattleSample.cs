using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBase.Input;
using Monomon.Battle;
using Monomon.Effects;
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
        private PaletteEffect _paletteEffect;
        private UIList<string> _list;
        private UIList<string> fightList;
        private UIList<string> itemList;
        private UIList<string> _currentList;
        private BattleReporter _battleReporter;
        private BattleCardViewModel _currentEnemyCard;
        private BattleCardViewModel _playerCard;
        private SpriteFont font;
        private Texture2D _spriteMap;
        private SoundEffect _menuMoveEffect;
        private SoundEffect _menuSelectEffect;
        private SoundEffect _battleTackleEffect;
        private SoundEffect _battleHurtEffect;
        private SoundEffect _battleXpUpEffect;
        private bool ready;
        private FadeEffect _fadeImpl;

        public BattleSample(GraphicsDevice gd, IINputHandler input, Mobmon player, Mobmon enemy, StateStack<double> stack, ContentManager content, PaletteEffect palette, FadeEffect fade) : base(gd, content)
        {
            _input = input;
            _player = player;
            _mob = enemy;
            _stack = stack;
            _paletteEffect = palette;
            _fadeImpl = fade;

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
                    _battleManager.Execute(new AttackCommand(AttackType.Tackle, _player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Slash", x => {
                    _battleManager.Execute(new Slash(_player.Stats));
                    _currentList = _list;
                }),
                new UIItem<string>("Swipe", x => {
                    _battleManager.Execute(new Swipe( _player.Stats));
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
                new UIItem<string>("Potion", x => {
                    _battleReporter.OnItem(new PotionMessage("Player","potion",5), _player,() => {_battleManager.NextTurn(); },_playerCard,_currentEnemyCard,true);
                }),
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

            _battleReporter = new BattleReporter(_graphics, _stack, _input, font, _spriteMap, OnPlaySound, _content);

            _battleManager = new BattleManager(_player, _mob, _battleReporter, _input, _playerCard, _currentEnemyCard);

            _battleManager.Start();

            var offset = _currentEnemyCard.PortraitOffsetX;
            _currentEnemyCard.PortraitOffsetX = 800;
            var dx = _currentEnemyCard.X;

            var slideIn = new TweenState((arg) =>
            {
                _currentEnemyCard.PortraitOffsetX = (int)arg.lerp;
            }, () => { }, 800, offset, 1.2f, EasingFunc.EaseOutBack);


            _stack.Push(slideIn, () => _stack.Pop());

            _fadeImpl.DoFade(_stack, () => ready = true);
        }

        public override void LoadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("File");
            _spriteMap = content.Load<Texture2D>("spritemap");

            _menuMoveEffect = content.Load<SoundEffect>("menuSelectSimple");
            _menuSelectEffect = content.Load<SoundEffect>("menuMoveChirpy");

            _battleTackleEffect = content.Load<SoundEffect>("tackle");
            _battleHurtEffect = content.Load<SoundEffect>("hurtChirpy");
            _battleXpUpEffect = content.Load<SoundEffect>("XpUp");

            InitBattle();

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
            if (_input.IsKeyPressed(KeyName.Option))
            {
                var paletteH = 16;
                var choice = new List<Choice>();
                var delta = 1.0f / (float)paletteH;
                for (var i = 6; i < paletteH; i++)
                {
                    var y = delta * (float)i;
                    choice.Add(new Choice($"#{i}", () =>
                    {
                        _paletteEffect.CurrentPalette = y;
                    }));
                }

                SelectChoice("Select palette", choice.ToArray());
            }

            if (_battleManager.BattleOver())
            {
                var result = _battleManager.GetOutcome();

                if (result == BattleOutcome.Win)
                {
                    _fadeImpl.FadeIn(_stack,
                        () =>
                        {
                            _stack.Pop(); // pop the battle state 
                        });
                }
                else
                {
                    var yesChoice = new Choice("Yes", () =>
                    {
                        _player = new Mobmon("Player", 3, new MonStatus(4, 2, 3));
                        InitBattle();
                    });
                    var no = new Choice("No", () =>
                    {
                    });
                    SelectChoice("You lost, continue with another mon?", yesChoice, no);
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
                    new MessageScene(_graphics, message, font, _spriteMap, _content),
                    1000,
                    _input),
                () =>
                {
                    _stack.Push(
                        new ConfirmState(
                            new ChoiceScene(_graphics, choices.Select(x => x.name).ToList(), font, _spriteMap, selection =>
                            {
                                _stack.Pop(); // pop the timed state
                                _stack.Pop(); // pop the confirm state
                                var choosen = choices.Where(item => item.name == selection).FirstOrDefault();
                                if (choosen != null)
                                    choosen.action();
                            }, _content),
                            _input),
                        () =>
                        {
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
            batch.End(); // end global effect, which is null
            _paletteEffect.EffectBegin(batch); //start drawing again usingpalette... ?
            DrawBattle(batch);
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

        private void DrawBattle(SpriteBatch batch)
        {
            if (ready)
            {
                ListView.DrawUIList(_currentList, new Vector2(UIValues.PlayerHudX, UIValues.PlayerHudY + 100), batch, font);
                BattleCardView.Draw(batch, new Vector2(_currentEnemyCard.X, _currentEnemyCard.Y), font, _spriteMap, _currentEnemyCard);
                BattleCardView.Draw(batch, new Vector2(_playerCard.X, _playerCard.Y), font, _spriteMap, _playerCard);
            }
            _currentEnemyCard.SetHealth(_mob.Health);
            _playerCard.SetHealth(_player.Health);
            _playerCard.SetXp(_player.Xp);



            //batch.End();
            //batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null, _effect);
            //_paletteEffect.EffectBegin(batch); //since we stop to draw the fade reenable palette effect
        }
    }
}
