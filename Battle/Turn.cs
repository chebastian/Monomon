using System;

namespace Monomon.Battle
{
    public class Turn
    {
        private readonly Action<Turn> t;
        private bool _completed;

        public Turn(Action<Turn> t)
        {
            this.t = t;
            CanExecute = true;
            Executing = false;
            _completed = false;
        }

        public void Execute()
        {
            Executing = true;
            t(this);
        }

        public bool CanExecute { get; set; }

        internal bool Completed
        {
            get;set;
        }
        public bool Executing { get; set; }
    }
}
