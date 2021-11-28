using System;

namespace Monomon.Battle
{
    public class Turn
    {
        private readonly Action<Turn> t;

        public Turn(Action<Turn> t)
        {
            this.t = t;
        }

        public void Execute()
        {
            t(this);
        }

        internal bool Completed
        {
            get;set;
        }
    }
}
