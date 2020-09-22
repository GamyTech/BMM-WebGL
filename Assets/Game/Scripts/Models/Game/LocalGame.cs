using GT.Backgammon.Player;

namespace GT.Backgammon.Logic
{
    public class LocalGame : LiveGame
    {
        private IPlayer playerBeforeDouble;

        #region Constructor
        public LocalGame(Stake stake, params IPlayer[] players) : base(stake, players) { }
        #endregion Constructor

        #region Overrides
        public override void Rematch(bool isRematch)
        {

        }

        public override void Surrender()
        {
            StopGame(NextPlayer(CurrentTurnPlayer));
        }

        public override void ResumeMatch(IPlayer currentTurnPlayer, int turnIndex, string currentBoard, bool isRolled)
        {
            base.ResumeMatch(currentTurnPlayer, turnIndex, currentBoard, isRolled);

            int f, s;
            if (turnIndex == START_TURN_INDEX)
                Dice.GetRandomStartDice(out f, out s);
            else
                Dice.GetRandomDice(out f, out s);

            CurrentTurnPlayer.SetDice(f, s, GameBoard);
        }

        protected override void StartNextTurn()
        {
            base.StartNextTurn();

            int f, s;
            if (CurrentTurnIndex == START_TURN_INDEX)
                Dice.GetRandomStartDice(out f, out s);
            else
                Dice.GetRandomDice(out f, out s);
            CurrentTurnPlayer.SetDice(f, s, GameBoard);
        }

        public override void InitiateDoubleRequest()
        {
            if (playerBeforeDouble == null)
                playerBeforeDouble = CurrentTurnPlayer;
            base.InitiateDoubleRequest();
        }

        protected override void OnDoubleResponseEvent(DoubleResponse response, IPlayer player)
        {
            if (response == DoubleResponse.GiveUp)
                StopGame(NextPlayer(player));
            else
            {
                base.OnDoubleResponseEvent(response, player);

                if (response == DoubleResponse.DoubleAgain)
                    InitiateDoubleRequest();
                else if (response == DoubleResponse.Yes)
                {
                    m_stake.DoubleDone(NextPlayer(player));

                    CurrentTurnPlayer = playerBeforeDouble;
                    playerBeforeDouble = null;
                    StartNextTurn();
                }
            }
        }

        protected override void OnTurnTimerRanOutEvent()
        {
            base.OnTurnTimerRanOutEvent();
            turnTimer.StartBankTime();
        }
        #endregion Overrides
    }
}
