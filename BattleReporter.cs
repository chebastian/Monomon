using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomon.Battle;
using Monomon.Input;
using Monomon.State;
using Monomon.Views.Scenes;
using System;
using System.Collections.Generic;

namespace Monomon
{
    public class BattleReporter
    {
        private readonly SpriteBatch batch;
        private readonly StateStack<double> _stack;
        private Texture2D _sprites;
        private SpriteFont _font;
        private IINputHandler _input;
        private GraphicsDevice _gd;

        public List<string> Messages { get; set; }
        public BattleReporter(SpriteBatch batch, GraphicsDevice gd, State.StateStack<double> stack, IINputHandler input, SpriteFont font, Texture2D sprites)
        {
            _sprites = sprites;
            _font = font;
            _input = input;
            _gd = gd;
            if (batch is null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            this.batch = batch;
            _stack = stack;
            Messages = new List<string>();
        }

        public void OnAttack(BattleMessage message, Mons.Mobmon _oponent, Action continueWith)
        {
            var str = $"{message.attacker} attacked {message.receiver} for {message.damage} points of damage!";
            var view = new MessageScene(_gd, str, _font);

            var attackInfo = new MessageScene(_gd, $"{message.attacker} choose {message.name}", _font);
            var attackMessage = new MessageScene(_gd, str, _font);

            _stack.Push(new TimeoutState(attackInfo, 1000, _input, onCancel: () => { _stack.Pop(); }),
                onCompleted:
                    () =>
                    {
                        _stack.Pop();
                        _stack.Push(new TimeoutState(attackMessage, 1000, _input, () => { _stack.Pop(); }),
                            onCompleted:
                                () => { 
                                    _stack.Pop();

                                    var animation = new List<Animation.Frame>() 
                                    { 
                                        new Animation.Frame(0,16),
                                        new Animation.Frame(24,16),
                                        new Animation.Frame(48,16),
                                        new Animation.Frame(72,16,() => { _stack.Pop(); continueWith(); }),
                                    };
                                    var anim = new AnimationScene(_gd, new Microsoft.Xna.Framework.Vector2(200, 0), _sprites, animation);

                                    var health = _oponent.Health;
                                    var tween = new TweenState((arg) => _oponent.Health= (float)(health -arg.lerp), () => {
                                        _oponent.Health = health - message.damage;
                                        _stack.Pop(); continueWith(); }, 0.0f, message.damage, 1.0f);

                                    var fs = new SceneState(anim,_input); 
                                    //_stack.Push(fs,() => { });
                                    _stack.Push(tween, () => {});
                                });
                    });

        }
    }
}