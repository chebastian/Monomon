using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameBase.Input;
using Monomon.State;
using Monomon.Views.Scenes;
using System;
using System.Linq;

namespace Monomon.Battle;
public interface ICommand<TArg>
{
    void Execute(TArg arg);
}

public enum State { Win, Lose, Run };
public record FinishedBattleArg(State state);
public record Choice(string name, Action action);
public record ConfirmArg(string message, params Choice[] choises);

public class ConfirmMessageHandler : ICommand<ConfirmArg>
{
    private IINputHandler _input;
    SceneStack _stack;
    GraphicsDevice _graphics;
    SpriteFont font;
    Texture2D _spriteMap;
    ContentManager _content;

    public ConfirmMessageHandler(SceneStack stack, GraphicsDevice graphics, SpriteFont font, Texture2D spriteMap, ContentManager content, IINputHandler input)
    {
        _input = input;
        _stack = stack;
        _graphics = graphics;
        this.font = font;
        _spriteMap = spriteMap;
        _content = content;
    }

    public void Execute(ConfirmArg arg)
    {
        var choices = arg.choises;
        var message = arg.message;

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
}

public class FinishBattleHandler : ICommand<FinishedBattleArg>
{
    private StateStack<double> _stack;

    public FinishBattleHandler(StateStack<double> stack)
    {
        _stack = stack;
    }

    public void Execute(FinishedBattleArg arg)
    {
        _stack.Pop();
    }
}

public class BattleScene : SceneView
{
    private ICommand<FinishedBattleArg> _finishCommand;
    private ICommand<ConfirmArg> _confirmCommand;


    public BattleScene(GraphicsDevice gd, ContentManager content, IINputHandler input, ICommand<FinishedBattleArg> finishBattle, ICommand<ConfirmArg> confirmCmd) : base(gd, content)
    {
        _finishCommand = finishBattle;
        _confirmCommand = confirmCmd;
    }

    public override void LoadScene(ContentManager content)
    {
    }

    public override void Update(double time)
    {
        var confirmArg = new ConfirmArg("did you win?",
            new Choice("Yes", () => _finishCommand.Execute(new FinishedBattleArg(State.Win))),
            new Choice("No", () => _finishCommand.Execute(new FinishedBattleArg(State.Lose))));

        _confirmCommand.Execute(confirmArg);
    }

    protected override void OnDraw(SpriteBatch batch)
    {
    }
}
