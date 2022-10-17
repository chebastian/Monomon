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

namespace Monomon.Views.Samples;

class BattleSample : SceneView
{
    private BattleManager _battleManager;
    private IINputHandler _input;
    private List<Mobmon> _playerMons;
    private Mobmon _player;
    private Mobmon _mob;
    private SceneStack _stack;
    private PaletteEffect _paletteEffect;
    private UIList<string> _rootMenu;
    private UIList<string> fightList;
    private UIList<string> itemList;
    private UIList<string> _currentRoot;
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
    private SoundEffect _enterBattleEffect;
    private bool ready;
    private FadeEffect _fadeImpl;

    public BattleSample(GraphicsDevice gd, IINputHandler input, List<Mobmon> playerMons, Mobmon enemy, SceneStack stack, ContentManager content, PaletteEffect palette, FadeEffect fade) : base(gd, content)
    {
        _input = input;
        _playerMons = playerMons;
        _player = playerMons.First();
        _mob = enemy;
        _stack = stack;
        _paletteEffect = palette;
        _fadeImpl = fade;

        _rootMenu = new UIList<string>(new List<UIItem<string>>() {
            new UIItem<string>("Fight", x => { _currentRoot = fightList; }),
            new UIItem<string>("Item", x => { _currentRoot = itemList;}),
            new UIItem<string>("Mon", x => SwapMon()),
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
                _currentRoot = _rootMenu;
            }),
            new UIItem<string>("Slash", x => {
                _battleManager.Execute(new Slash(_player.Stats));
                _currentRoot = _rootMenu;
            }),
            new UIItem<string>("Swipe", x => {
                _battleManager.Execute(new Swipe( _player.Stats));
                _currentRoot = _rootMenu;
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
            new UIItem<string>("Potion +5", x => {
                _battleManager.Execute(new PotionCommand(5));
                _currentRoot = _rootMenu;
            }),
            new UIItem<string>("Potion +1", x => {
                _battleManager.Execute(new PotionCommand(1));
                _currentRoot = _rootMenu;
            }),
            new UIItem<string>("Back", x => {_currentRoot = _rootMenu; }),
        }, x =>
        {
            OnMenuMove();
        },
    x =>
    {
        OnMenuSelect();
    });

        _currentRoot = _rootMenu;

    }

    private void SwapMon()
    {
        var chooseMon = _playerMons.Select(x => new Choice(x.Name, () =>
        {
            _battleManager.Execute(new PotionCommand(0));
            _player = x;
            _playerCard.Swap(x.Name, x.MaxHealth, x.Health, 6);
            _currentRoot = _rootMenu;
        }));
        var yesChoice = new Choice("Yes", () =>
        {
            SelectChoice("Select your mon", chooseMon.ToArray());
        });
        var no = new Choice("No", () =>
        {
            _currentRoot = _rootMenu;
        });

        SelectChoice("Do you want to swap your mon?", yesChoice, no);
    }

    private void InitBattle()
    {
        _currentEnemyCard = new BattleCardViewModel(_mob.Name, _mob.MaxHealth, _mob.Health, 2);
        _currentEnemyCard.X = UIValues.OponentHudX;
        _currentEnemyCard.Y = 10;
        _currentEnemyCard.PortraitSrc = new Rectangle(11 * UIValues.TileSz, 0, UIValues.PortraitW, UIValues.PortraitH);

        _playerCard = new BattleCardViewModel("Player", _player.MaxHealth, _player.Health, 6);
        _playerCard.X = UIValues.PlayerPoirtraitX;
        _playerCard.Y = UIValues.PlayerPoirtraitY;
        _playerCard.PortraitSrc = new Rectangle(7 * UIValues.TileSz, 0, UIValues.PortraitW, UIValues.PortraitH);

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

        _fadeImpl.Flash(0.2f, _stack, () => { }, () =>
        {
            _fadeImpl.DoFade(0.8f, _stack, () => ready = true);
        });
        OnPlaySound(Sounds.EnterBattle);
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
        _enterBattleEffect = content.Load<SoundEffect>("enterBattle3");

        InitBattle();

    }

    private void OnPlaySound(Sounds sound)
    {
        var instance = sound switch
        {
            Sounds.Attack_Tackle => _battleTackleEffect.CreateInstance(),
            Sounds.TakeDamage => _battleHurtEffect.CreateInstance(),
            Sounds.XpUP => _battleXpUpEffect.CreateInstance(),
            Sounds.EnterBattle => _enterBattleEffect.CreateInstance(),
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
                _currentRoot.SelectNext();
            if (_input.IsKeyPressed(KeyName.Up))
                _currentRoot.SelectPrevious();
            if (_input.IsKeyPressed(KeyName.Back))
            {
                _currentRoot = _rootMenu;
            }

            if (_input.IsKeyPressed(KeyName.Select))
            {
                _currentRoot.Select();
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
                var chooseMon = _playerMons.Select(x => new Choice(x.Name, () =>
                {
                    _player = x;
                    InitBattle();
                }));
                var yesChoice = new Choice("Yes", () =>
                {
                    SelectChoice("Which mon: ", chooseMon.ToArray());
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
        //Disable palette effect for battles, this causes alphas to work as expected again
        //batch.End(); // end global effect, which is null
        //_paletteEffect.EffectBegin(batch); //start drawing again usingpalette... ?
        DrawBattle(batch);
    }



    private void DrawBattle(SpriteBatch batch)
    {
        if (ready)
        {
            //TODO clear the screen black so that the battle does not render uppon the level, 221011
            _graphics.Clear(Color.Black);

            ListView.DrawUIList(_currentRoot, new Vector2(UIValues.PlayerMenuX, UIValues.PlayerMenuY * UIValues.TileSz), batch, font);
            BattleCardView.DrawTopCard(batch, new Vector2(0, 2), font, _spriteMap, _currentEnemyCard);
            BattleCardView.Draw(batch, new Vector2(UIValues.PlayerMenuX, _playerCard.Y + 15), font, _spriteMap, _playerCard);
        }
        _currentEnemyCard.SetHealth(_mob.Health);
        _playerCard.SetHealth(_player.Health);
        _playerCard.SetXp(_player.Xp);



        //batch.End();
        //batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null, _effect);
        //_paletteEffect.EffectBegin(batch); //since we stop to draw the fade reenable palette effect
    }
}
