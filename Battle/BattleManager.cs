using Monomon.Mons;
using System;
using System.Threading.Tasks;

namespace Monomon.Battle
{
    public class BattleManager
    {
        private Mobmon _attacker;
        private Mobmon _oponent;
        private Turn _currentTurn;
        private bool _isPlayerTurn;

        public BattleManager(Mons.Mobmon player, Mons.Mobmon oponent)
        {
            _attacker = player;
            _oponent = oponent;
            _currentTurn = new Turn(x => { });
            _isPlayerTurn = true;
        }

        public void SetTurn(Turn t)
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

        internal Turn Next()
        {
            return new Turn(c => { });
        }

        internal void Attack(AttackCommand attackCommand)
        {
            SetTurn(new Turn(c =>
            {
                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    _oponent.Health -= attackCommand.stat.attack;
                    await Task.Delay(200);
                    _oponent.Health -= attackCommand.stat.attack;
                    await Task.Delay(200);
                    c.Completed = true;
                });
            }));
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
                Attack(new AttackCommand(AttackType.Normal,new MonStatus(1,2,3)));
            }

        }
    }
}
