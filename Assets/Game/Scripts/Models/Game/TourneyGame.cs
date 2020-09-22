using GT.Backgammon.Logic;
using GT.Backgammon.Player;
using GT.Websocket;

public class TourneyGame : RemoteGame
{
    public event TimerChangedHandler OnTotalTimerChanged = delegate { };
    
    protected GameTimer gameTimer;

    public TourneyGame(Stake stake, int matchId, params IPlayer[] players) : base(stake, matchId, players)
    {
        gameTimer = new GameTimer();
        gameTimer.OnTurnTimeChanged += OnGameTimerChangedEvent;
    }
    public override void Unregister()
    {
        gameTimer.OnTurnTimeChanged -= OnGameTimerChangedEvent;
        base.Unregister();
    }

    public override void UpdateGame(float deltaTime)
    {
        gameTimer.UpdateTimers(deltaTime);
        base.UpdateGame(deltaTime);
    }
    protected override void StopTimer(IPlayer player)
    {
        gameTimer.StopTurn();
        base.StopTimer(player);
    }

    protected override void StartTimer(IPlayer player)
    {
        if (player.playerData != null)
            gameTimer.StartTurn(player.playerData.TotalTime, 0);
        base.StartTimer(player);
    }

    protected override void ReceivedTurn(MoveReceivedService service)
    {
        if (service.TotalTime > 0)
        {
            GetPlayerFromId(service.NextTurnPlayer).playerData.TotalTime = service.TotalTime;
        }
        base.ReceivedTurn(service);
    }

    protected override void ReceivedDoubleResponse(MoveReceivedService service)
    {
        if (service.TotalTime > 0)
        {
            GetPlayerFromId(service.NextTurnPlayer).playerData.TotalTime = service.TotalTime;
        }
        base.ReceivedDoubleResponse(service);
    }

    protected override void UpdateStakeOnDoubleReceived(MoveReceivedService service)
    {
        m_stake.DoubleDone(GetPlayerFromId(service.CantDoubleId), 1 << service.CurrentDoubleAmount, 0, service.CurrentDoubleAmount);
    }

    protected override void UpdateStakeOnDoubleRequest(DoubleCubeRequestService doubleCubeRequest)
    {
        m_stake.DoubleDone(GetPlayerFromId(doubleCubeRequest.SenderId), 1 << doubleCubeRequest.CurrentDouble, 0, true);
    }

    protected void OnGameTimerChangedEvent(float time)
    {
        PlayerData data = CurrentTurnPlayer.playerData;
        if (data != null)
        {
            data.TotalTime = time;
            OnTotalTimerChanged(CurrentTurnPlayer, data);
        }
    }

    public override void Rematch(bool isRematch) {}
}
