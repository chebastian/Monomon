namespace Monomon.Battle
{
    public class TurnManager
    {
        private Turn _currentTurn;
        public TurnManager()
        {
            _currentTurn = new Turn(x => { });
        }

        public void SetTurn(Turn t)
        {
            _currentTurn.Executing = false;
            _currentTurn.Completed = false;
            _currentTurn.CanExecute = false;

            t.CanExecute = true;
            t.Completed = false;
            t.Executing = false;
            _currentTurn = t;
            _currentTurn.Execute();
        }

        internal bool TurnIsOver()
        {
            return _currentTurn.Completed;
        }

        internal Turn Next()
        {
            return new Turn(c => { });
        }
    }
}
