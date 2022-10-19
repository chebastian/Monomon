using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBase.Input;
using Monomon.Battle;
using Monomon.Mons;
using Monomon.State;
using Monomon.ViewModels;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;

namespace Monomon;

public enum Sounds
{
    Attack_Tackle,
    TakeDamage,
    XpUP,
    EnterBattle
}

public class BattleReporter
{
    protected readonly SceneStack _stack;
    protected ContentManager _content;
    protected Action<Sounds> _soundCallback;
    protected Texture2D _sprites;
    protected SpriteFont _font;
    protected IINputHandler _input;
    protected GraphicsDevice _gd;

    public List<string> Messages { get; set; }
    public BattleReporter(GraphicsDevice gd, SceneStack stack, IINputHandler input, SpriteFont font, Texture2D sprites, Action<Sounds> soundCallback, ContentManager mgr)
    {
        _content = mgr;
        _soundCallback = soundCallback;
        _sprites = sprites;
        _font = font;
        _input = input;
        _gd = gd;

        _stack = stack;
        Messages = new List<string>();
    }

    protected TimedState TimedMessage(string message)
    {
        return new TimedState(new MessageScene(_gd, message, _font, _sprites, _content), 2500, _input);
    }

    protected ConfirmState ConfirmMessage(string message)
    {
        return new ConfirmState(new MessageScene(_gd, message, _font, _sprites, _content, true), _input);
    }

    public void OnItem(ItemMessage message, Mons.Mobmon user, Action continueWith)
    {
        var attackInfoState = TimedMessage($"{message.user} used {message.name}");
        _stack.BeginStateSequence();
        _stack.AddState(attackInfoState);

        var handler = new PotionHandler(_gd, _stack, _input, _font, _sprites, _soundCallback, _content);
        if (message is PotionMessage potion)
            handler.Execute(potion, user, continueWith);
    }

    public void OnSwap(Mobmon swapper, Mobmon swapTo, Action doSwap,Action continueWith, BattleCardViewModel card)
    {
        var handler = new SwapMonHandler(_gd,_stack,_input,_font,_sprites, _soundCallback, _content);
        handler.Execute(swapper, swapTo, doSwap,continueWith, card, card);
    }

    public void OnAttack(BattleMessage message, Mons.Mobmon attacker, Mons.Mobmon _oponent, Action continueWith, BattleCardViewModel attackerCard, BattleCardViewModel oponentCard, bool isPlayer)
    {
        var handler = new AttackHandler( _gd, _stack, _input, _font, _sprites, _soundCallback, _content);
        handler.Execute(message,attacker,_oponent,continueWith,attackerCard,oponentCard,isPlayer);
    }

}