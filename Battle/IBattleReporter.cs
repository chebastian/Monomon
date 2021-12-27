using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Battle
{
    public interface IBattleReporter
    {
        public void OnAttack(BattleMessage message, Action continueWith);
    }
}
