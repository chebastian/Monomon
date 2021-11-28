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
    }
}
