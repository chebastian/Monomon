using Monomon.Input;
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
        private BattleReporter _reporter;
        private Mobmon _player;
        private Mobmon _attacker;
        private Mobmon _oponent;
        private Turn _currentTurn;
        private bool _isPlayerTurn;
        private IINputHandler _input;

        public BattleManager(Mons.Mobmon player, Mons.Mobmon oponent, BattleReporter reporter, IINputHandler input)
        {
            _reporter = reporter;
            _player = player;
            _attacker = player;
            _oponent = oponent;
            _currentTurn = new Turn(x => { });
            _isPlayerTurn = true;
            _input = input;
        }

        private void SetTurn(Turn t)
        {
            _currentTurn.Completed = false;

            t.Completed = false;
            _currentTurn = t;
            _currentTurn.Execute();
        }
        internal void Attack(AttackCommand attackCommand)
        {
            //SetTurn(new Turn(c =>
            //{
            //    Task.Run(async () =>
            //    {
            //        _executing = true;
            //        var task = attackCommand.attackType switch
            //        {
            //            AttackType.Tackle => Tackle(attackCommand, c),
            //            AttackType.Slash => Swipe(attackCommand, c),
            //            AttackType.Wrap => Wrap(attackCommand, c),
            //            _ => throw new ArgumentOutOfRangeException($"{attackCommand.attackType}")
            //        };

            //        task.Start();
            //        await task.ContinueWith(
            //        x =>
            //        {
            //            c.Completed = true;
            //            _executing = false;
            //        });
            //    });

            //}));
            DoTackle(attackCommand);
        }

        public bool IsInteractive()
        {
            return _isPlayerTurn && !_executing;
        }

        Random rand = new Random();
        private bool _executing;

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

                await Task.Delay(1000);
                //c.Completed = true;
            });
        }

        void DoTackle(AttackCommand attackCommand)
        {
            //DisplayAttackName(attackCommand.attackType);
            //RenderAnimationToCompletion(attackCommand.attackType);
            //UpdateHealthBar(attackCommand.stat.attack);
            //DislpayOptionalAttackMessage(attackCommand);
            //NextRound();

            //MVP
            // display attackname
            // update health
            // next round

            var msg = new BattleMessage(_attacker.Name, _oponent.Name, attackCommand.attackType.ToString(), attackCommand.stat.attack);
            _reporter.OnAttack(msg,_attacker,_oponent, () => {
                NextTurn();
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
                await Task.Delay(1000);
                t.Completed = true;
            });
        }

        internal void Start()
        {
            SetTurn(new Turn(x => { }));
            _executing = false;
        }

        internal void NextTurn()
        {
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

        public bool IsPlayerTurn()
        {
            return _isPlayerTurn;
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
