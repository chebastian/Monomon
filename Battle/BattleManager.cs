using MonoGameBase.Input;
using Monomon.Mons;
using Monomon.ViewModels;
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
        private BattleCardViewModel _attackerCard;
        private BattleCardViewModel _oponentCard;

        public BattleManager(Mons.Mobmon player, Mons.Mobmon oponent, BattleReporter reporter, IINputHandler input, BattleCardViewModel playerCard, BattleCardViewModel oponentCard)
        {
            _reporter = reporter;
            _player = player;
            _attacker = player;
            _oponent = oponent;
            _currentTurn = new Turn(x => { });
            _isPlayerTurn = true;
            _input = input;
            _attackerCard = playerCard;
            _oponentCard = oponentCard;
        }

        private void SetTurn(Turn t)
        {
            _currentTurn.Completed = false;

            t.Completed = false;
            _currentTurn = t;
            _currentTurn.Execute();
        }

        internal void Execute(BattleCommand cmd)
        {
            if (cmd is AttackCommand attack)
                DoTackle(attack);
            if (cmd is PotionCommand potion)
                _reporter.OnItem(new PotionMessage(_attacker.Name, "potion",_attacker.Health, potion.hpRestore), _attacker, () => { NextTurn(); });
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
            var msg = new AttackMessage(_attacker.Name, _oponent.Name, attackCommand.attackType.ToString(), attackCommand.stat.attack);
            _reporter.OnAttack(msg, _attacker, _oponent, () =>
            {
                NextTurn();
            }, _attackerCard, _oponentCard, _attacker == _player);
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
                var temp = (_attacker, _attackerCard);
                _attacker = _oponent;
                _attackerCard = _oponentCard;
                _oponent = temp._attacker;
                _oponentCard = temp._attackerCard;
            }

            if (!_isPlayerTurn)
            {
                if (_attacker.HealthPercentage < 0.3 && Random.Shared.Next(255) > 200)
                {
                    Execute(new PotionCommand((int)(_attacker.MaxHealth / 0.3f)));
                }
                else
                    Execute(new AttackCommand(AttackType.Tackle, _attacker.Stats));
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
