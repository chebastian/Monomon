using Monomon.Mons;
using System;
using System.Threading.Tasks;

namespace Monomon.Battle
{
    public enum BattleOutcome
    {
        Win,
        Lose,
    }

    public class BattleManager
    {
        private IBattleReporter _reporter;
        private Mobmon _player;
        private Mobmon _attacker;
        private Mobmon _oponent;
        private Turn _currentTurn;
        private bool _isPlayerTurn;

        public BattleManager(Mons.Mobmon player, Mons.Mobmon oponent, IBattleReporter reporter)
        {
            _reporter = reporter;
            _player = player;
            _attacker = player;
            _oponent = oponent;
            _currentTurn = new Turn(x => { });
            _isPlayerTurn = true;
        }

        private void SetTurn(Turn t)
        {
            _currentTurn.Completed = false;

            t.Completed = false;
            _currentTurn = t;
            _currentTurn.Execute();
        }

        internal bool TurnIsDone()
        {
            return _currentTurn.Completed;
        }

        internal void Attack(AttackCommand attackCommand)
        {
            SetTurn(new Turn(c =>
            {
                Task.Run(async () =>
                {
                    var task = attackCommand.attackType switch
                    { 
                        AttackType.Tackle => Tackle(attackCommand,c),
                        AttackType.Slash => Swipe(attackCommand,c),
                        AttackType.Wrap => Wrap(attackCommand,c),
                        _ => throw new ArgumentOutOfRangeException($"{attackCommand.attackType}")
                    };

                    task.Start();
                    await task;
                });

            }));
        }

        Random rand = new Random();
        private Task Wrap(AttackCommand attackCommand, Turn c)
        {
            var count = rand.Next(1, 4);
            return new Task(async () =>
            {
                var total = 0;
                for (var i = 0; i < count; i++)
                {
                    var attackDamage = rand.Next(1, 2);
                    _oponent.Health -= attackDamage;
                    total += attackDamage;
                    await Task.Delay(300);
                }

                _reporter.OnAttack(new BattleMessage(_attacker.Name, _oponent.Name, total));
                await Task.Delay(1000);
                c.Completed = true;
            }); 
        }

        Task Tackle(AttackCommand attackCommand, Turn t)
        {
            return new Task(async () =>
            {
                await Task.Delay(200);
                _oponent.Health -= attackCommand.stat.attack;
                _reporter.OnAttack(new BattleMessage(_attacker.Name, _oponent.Name, attackCommand.stat.attack));
                await Task.Delay(1000);
                t.Completed = true;
            }); 
        }

        Task Swipe(AttackCommand attackCommand, Turn t)
        {
            return new Task(async () =>
            {
                await Task.Delay(200);
                _oponent.Health -= attackCommand.stat.attack;
                await Task.Delay(200);
                _oponent.Health -= attackCommand.stat.attack;
                _reporter.OnAttack(new BattleMessage(_attacker.Name, _oponent.Name, attackCommand.stat.attack * 2));
                await Task.Delay(1000);
                t.Completed = true;
            }); 
        }

        internal void Start()
        {
            SetTurn(new Turn(x => { }));
        }

        internal void NextTurn()
        {
            Start();
            _isPlayerTurn = !_isPlayerTurn;

            //swap player and mon
            {
                var temp = _attacker;
                _attacker = _oponent;
                _oponent = temp;
            }

            if (!_isPlayerTurn)
            {
                Attack(new AttackCommand(AttackType.Tackle, _attacker.Stats));
            }

        }

        internal bool BattleOver()
        {
            return _attacker.Health <= 0 || _oponent.Health <= 0;
        }

        internal BattleOutcome GetOutcome()
        {
            return _player.Health > 0 ? BattleOutcome.Win : BattleOutcome.Lose;
        }
    }
}
