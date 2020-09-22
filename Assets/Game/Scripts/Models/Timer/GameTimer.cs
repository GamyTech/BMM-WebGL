namespace GT.Backgammon.Logic
{
    public class GameTimer 
    {
        public delegate void TimeChangedHandler(float time);
        public delegate void TurnTimeRanOutHandler();
        public delegate void BankTimeRanOutHandler();

        public event TimeChangedHandler OnTurnTimeChanged;
        public event TimeChangedHandler OnBankTimeChanged;
        public event TurnTimeRanOutHandler OnTurnTimeRanOut;
        public event BankTimeRanOutHandler OnBankTimeRanOut;

        private Timer turnTimer;
        private Timer bankTimer;

        public GameTimer()
        {
            turnTimer = new Timer();
            turnTimer.OnTimerChanged += TurnTimer_OnTimerChanged;
            turnTimer.OnTimerEnded += TurnTimer_OnTimerEnded;

            bankTimer = new Timer();
            bankTimer.OnTimerChanged += BankTimer_OnTimerChanged;
            bankTimer.OnTimerEnded += BankTimer_OnTimerEnded;
        }

        public void StartTurn(float turnTime, float bankTime)
        {
            turnTimer.SetTimer(turnTime);
            bankTimer.SetTimer(bankTime);

            if (turnTime > 0)
                turnTimer.SetEnabled(true);
            else
                bankTimer.SetEnabled(true);
        }

        public void StopTurn()
        {
            turnTimer.SetEnabled(false);
            bankTimer.SetEnabled(false);
        }

        public void StartBankTime()
        {
            bankTimer.SetEnabled(true);
        }

        public void UpdateTimers(float deltaTime)
        {
            turnTimer.UpdateTimer(deltaTime);
            bankTimer.UpdateTimer(deltaTime);
        }

        private void TurnTimer_OnTimerChanged(float currentTime)
        {
            if(OnTurnTimeChanged != null)
                OnTurnTimeChanged(currentTime);
        }

        private void TurnTimer_OnTimerEnded()
        {
            turnTimer.SetEnabled(false);

            if(OnTurnTimeRanOut != null)
                OnTurnTimeRanOut();
        }

        private void BankTimer_OnTimerChanged(float currentTime)
        {
            if (OnBankTimeChanged != null)
                OnBankTimeChanged(currentTime);
        }

        private void BankTimer_OnTimerEnded()
        {
            bankTimer.SetEnabled(false);

            if(OnBankTimeRanOut != null)
                OnBankTimeRanOut();
        }
    }
}
