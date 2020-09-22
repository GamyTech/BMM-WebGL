using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GT.Backgammon.Logic;
using GT.Backgammon.Player;
using GT.Websocket;
using GT.Store;
using UnityEngine.Events;
using GT.Database;

public class SpectatorGameController : MonoBehaviour, IGameController
{
    #region Public Members
    private static SpectatorGameController m_instance;
    public static SpectatorGameController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<SpectatorGameController>()); } private set { m_instance = value; } }

    public float SecondsPerGame = 60;

    public GT.Assets.BoardItemData board;
    public GT.Assets.DiceItemData dice;

    public BoardView boardView;
    public DoubleCubeView doublingCubeView;
    public GameMenuView gameMenuView;
    public GameDiceView diceView;
    public MascotsView mascotView;
    public BlessingView blessingView;
    public MatchIDView matchIdView;
    public SpectatorOverlayView overlayView;

    public bool HasStarted;
    public int MaxMatches = 5;
    #endregion Public Members

    #region Private Members
    private RemoteGame GameInstance;
    //private List<UpdatedUsedItem> ItemsChanged = new List<UpdatedUsedItem>();
    private bool pendingChangementSignal;
    private Scheduler jobScheduler;
    private List<int> availableMatches = new List<int>();
    private int currentMatch = -1;
    #endregion Private Members

    #region Events Register and Unregister
    private void RegisterGameEvents()
    {
        GameInstance.OnGameResumed += GameStartedOrResumed;
        GameInstance.OnGameStopped += Game_OnGameStopped;
        GameInstance.OnStartTurn += Game_OnStartTurn;
        GameInstance.OnEndTurn += Game_OnEndTurn;
        GameInstance.OnDiceRolled += Game_OnDiceRolled;
        GameInstance.OnMoveDone += Game_OnMoveDone;
        GameInstance.OnTimerChanged += Game_OnTimerChanged;
        GameInstance.OnTurnTimerEnded += Game_OnTurnTimerEnded;
        GameInstance.OnBetChanged += Game_OnBetChanged;
    }

    private void UnregisterGameEvents()
    {
        GameInstance.OnGameResumed -= GameStartedOrResumed;
        GameInstance.OnGameStopped -= Game_OnGameStopped;
        GameInstance.OnStartTurn -= Game_OnStartTurn;
        GameInstance.OnEndTurn -= Game_OnEndTurn;
        GameInstance.OnDiceRolled -= Game_OnDiceRolled;
        GameInstance.OnMoveDone -= Game_OnMoveDone;
        GameInstance.OnTimerChanged -= Game_OnTimerChanged;
        GameInstance.OnTurnTimerEnded -= Game_OnTurnTimerEnded;
        GameInstance.OnBetChanged -= Game_OnBetChanged;
    }
    #endregion Events Register and Unregister

    private void Awake()
    {
        jobScheduler = new Scheduler(this);
        if (MainSceneController.Instance == null)
        {
            Camera.main.gameObject.AddComponent(typeof(AudioListener));
            gameObject.AddComponent(typeof(SettingsController));
            gameObject.AddComponent(typeof(PooledObjectsContainer));
            gameObject.AddComponent(typeof(ObjectPoolManager));
            gameObject.AddComponent(typeof(UnityEngine.EventSystems.EventSystem));
            gameObject.AddComponent(typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
        
        boardView.LoadBoard(board);
        boardView.InitView(0.4f, true);
        ResetViews();

        overlayView.SetMaxTimeValue(SecondsPerGame);
    }

    void OnEnable()
    {
        WebSocketKit.Instance.OnOpenEvent += WebSocketKit_OnOpenEvent;
        //WebSocketKit.Instance.AckEvents[RequestId.GetCurrentMatches] += WebSocketKit_GetCurrentMatches;
        //WebSocketKit.Instance.AckEvents[RequestId.ConnectToMatch] += WebSocketKit_ConnectToMatch;

        overlayView.OnNextMatch += NextMatch;

        StartCoroutine(DatabaseKit.GetGlobalServerDetails(StartWebSocketConection));
    }

    void OnDisable()
    {
        WebSocketKit.Instance.OnOpenEvent -= WebSocketKit_OnOpenEvent;
        //WebSocketKit.Instance.AckEvents[RequestId.GetCurrentMatches] -= WebSocketKit_GetCurrentMatches;
        //WebSocketKit.Instance.AckEvents[RequestId.ConnectToMatch] -= WebSocketKit_ConnectToMatch;

        overlayView.OnNextMatch -= NextMatch;
    }

    private void Update()
    {
        if (GameInstance != null)
            GameInstance.UpdateGame(Time.deltaTime);
    }

    private void StartWebSocketConection()
    {
        AssetController.Instance.InitializeAssetBundles();
        NetworkController.Instance.Connect();
    }

    private void RequestCurrentMatches()
    {
        RequestInGameController.Instance.RequestCurrentMatches(MaxMatches);
    }

    private IEnumerator StartMatchRotation()
    {
        if (availableMatches.Count > 0)
        {
            currentMatch = 0;
            ReconnectToMatch(availableMatches[currentMatch]);
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
            RequestCurrentMatches();
        }
    }

    private void GotoNextMatch()
    {
        if (++currentMatch >= availableMatches.Count)
        {
            --currentMatch;
            RequestCurrentMatches();
        }
        else
        {
            ReconnectToMatch(availableMatches[currentMatch]);
        }
    }

    private void GotoPrevMatch()
    {
        if (--currentMatch < 0)
            currentMatch = 0;

        ReconnectToMatch(availableMatches[currentMatch]);
    }

    private void ReconnectToMatch(int matchID)
    {
        HasStarted = false;
        ResetViews();
        HideAllViews();

        RequestInGameController.Instance.ConnectToMatch(matchID);
        overlayView.ShowLoading();
    }

    #region Public functions
    public void PreviousMatch()
    {
        if (GameInstance != null)
            UnregisterGameEvents();

        GotoPrevMatch();
    }

    public void NextMatch()
    {
        if (GameInstance != null)
            UnregisterGameEvents();

        GotoNextMatch();
    }
    #endregion Public functions

    private void ConnectToGame(ReconnectData recoData)
    {
        Debug.Log("Run game = " + recoData.ToString());
        overlayView.HideTitle();
        overlayView.StartTimer();

        CreateGame(recoData);
        GameInstance.ResumeMatch(recoData.CurrentTurnPlayer, recoData.MoveId, recoData.NewBoard, recoData.CurrentDice, recoData.IsRequestingDouble, recoData.IsRolled);
    }

    private void CreateGame(ReconnectData recoData)
    {
        if (HasStarted)
            return;
        HasStarted = true;

        matchIdView.SetMatchID(recoData.MatchId);

        GameInstance = GameFactory.CreateRemoteGame(
            recoData.Players, recoData.Kind, recoData.Bet, recoData.Fee, recoData.MaxBet, 
            recoData.CantDoublePlayer, recoData.DoublesCount, recoData.MatchId);
        GameInstance.InitPlayersByOrder();
        RegisterGameEvents();
    }

    private void OnDestroy()
    {
        UnregisterGameEvents();
        
        jobScheduler.StopAllJobs();
        jobScheduler = null;
        m_instance = null;
    }

    private void ResetViews()
    {
        gameMenuView.Reset();
        doublingCubeView.Reset();
        jobScheduler.StopAllJobs();
    }

    private void HideAllViews()
    {
        diceView.HideDice();
    }

    private void SetCurrentRolingItems(bool isBottomPlayer, Dictionary<string, string> rollingItemIds)
    {
        if (rollingItemIds == null)
            return;

        string item;
        if (rollingItemIds.TryGetValue(Enums.StoreType.Dices.ToString(), out item) && !string.IsNullOrEmpty(item))
            diceView.SetDiceId(isBottomPlayer, item);

        if (rollingItemIds.TryGetValue(Enums.StoreType.Blessings.ToString(), out item) && !string.IsNullOrEmpty(item))
            blessingView.SetBlessingId(isBottomPlayer, item);
    }

    private bool isMainPlayer(IPlayer player)
    {
        return player.playerId.Equals(GameInstance.UserPlayer.playerId);
    }

    #region Server Events
    private void WebSocketKit_OnOpenEvent()
    {
        RequestCurrentMatches();
    }

    public void WebSocketKit_GetCurrentMatches(Ack service)
    {
        /*GetCurrentMatchesAck getMatchesAck = service as GetCurrentMatchesAck;
        if (service.Code == WSResponseCode.OK && getMatchesAck.MatchIds != null)
        {
            availableMatches.AddRange(getMatchesAck.MatchIds);
            GotoNextMatch();
        }
        else
        {
            Console.Log("Error in loading current games: " + service.Code);
        }*/
    }

    public void WebSocketKit_ConnectToMatch(Ack service)
    {
        /*ConnectToMatchAck connectToMatchAck = service as ConnectToMatchAck;
        if (service.Code == WSResponseCode.OK && connectToMatchAck.ReconnectData != null)
        {
            ConnectToGame(connectToMatchAck.ReconnectData);
        }
        else
        {
            Console.Log("Can't connect to the game: " + availableMatches[currentMatch] + ", error code: " + service.Code);
        }*/
    }
    #endregion Server Events

    #region Logic Events
    private void GameStartedOrResumed(GameStartEventArgs args)
    {
        boardView.InitView(0.4f, GameInstance.UserPlayer.playerColor == PlayerColor.White);
        boardView.ShowBoardState(Board.Serialize(GameInstance.GameBoard));

        ResetViews();

        LiveGameStartEventArgs nArgs = args as LiveGameStartEventArgs;

        Debug.Log("GameStartedOrResumed : " + nArgs.CubeNum + " " + nArgs.CantDoublePlayerId);

        gameMenuView.InitStakeData(Wallet.AmountToString(nArgs.BetValue, nArgs.Kind), Wallet.AmountToString(nArgs.FeeValue, nArgs.Kind), nArgs.MaxBet);
        gameMenuView.InitPlayersData(GameInstance.UserPlayer.playerData, GameInstance.OpponentPlayer.playerData);
        mascotView.InitPlayersData(GameInstance.UserPlayer, GameInstance.OpponentPlayer);

        string diceUser;
        GameInstance.UserPlayer.playerData.RollingItems.TryGetValue(Enums.StoreType.Dices.ToString(), out diceUser);
        string diceOpponent;
        GameInstance.OpponentPlayer.playerData.RollingItems.TryGetValue(Enums.StoreType.Dices.ToString(), out diceOpponent);

        diceView.Init(diceUser, diceOpponent);
    }
    private void Game_OnGameStopped(GameStopEventArgs args)
    {
        HasStarted = false;
        StopAllCoroutines();
        UnregisterGameEvents();

        RemoteGameStopEventArgs nArgs = args as RemoteGameStopEventArgs;
        Debug.Log("Game_OnGameStopped " + Time.time + " args.isCanceled: " + nArgs.IsCanceled);

        overlayView.StopTimer();

        if (nArgs.IsCanceled)
        {
            overlayView.ShowMessage("The game is cancelled");
        }
        else
        {
            overlayView.ShowMessage(nArgs.Winner.playerData.UserName + " (" + nArgs.Winner.playerColor + ") won!");
        }

        HasStarted = false;
        ResetViews();
        HideAllViews();

        StartCoroutine(WaitAndStartNewMatch());
    }

    private IEnumerator WaitAndStartNewMatch()
    {
        yield return new WaitForSeconds(5.0f);
        GotoNextMatch();
    }

    private void Game_OnBetChanged(BetChangedEventArgs eventArgs)
    {
        Debug.Log("Game_OnBetChanged : " + eventArgs.CantDoublePlayer + " " + eventArgs.CubeNum);

        doublingCubeView.MoveDoubleDice(isMainPlayer(eventArgs.CantDoublePlayer), eventArgs.CubeNum);
        gameMenuView.UpdateBetAmounts(eventArgs.CurrentPostFix + Wallet.AmountToString(eventArgs.CurrentBet));
    }

    private void Game_OnTurnTimerEnded(IPlayer player)
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.BankTime);
        gameMenuView.DimUserPic(player, isMainPlayer(player));
    }

    private void Game_OnTimerChanged(IPlayer player, PlayerData data)
    {
        if (isMainPlayer(player))
            gameMenuView.PlayerTimer(data);
        else
            gameMenuView.OpponentTimer(data);
    }
    #endregion Logic Events

    #region Player Events
    private void Game_OnDiceRolled(IPlayer player, Dice dice)
    {
        Debug.Log("Game_OnDiceRolled");

        bool isUser = isMainPlayer(player);
        SetCurrentRolingItems(isUser, player.playerData.RollingItems);

        Action callback = () => boardView.ShowMovesIndicators();

        jobScheduler.AddJob(new EnumeratorJob(blessingView.ShowBlessing(isUser)));
        jobScheduler.AddJob(new EnumeratorJob(diceView.ShowDice(dice.GetListOfDice(), isUser), callback));
    }
     
    private void Game_OnStartTurn(IPlayer player)
    {
        Action startTurnAction = () =>
        {
            gameMenuView.ActivatePlayersTime(isMainPlayer(player));
        };
        jobScheduler.AddJob(new ActionJob(startTurnAction));
    }

    private void Game_OnEndTurn(IPlayer currentPlayer, IPlayer nextPlayer)
    {
        Action action = () => diceView.HideDice();
        jobScheduler.AddJob(new ActionJob(action));
    }

    private void Game_OnMoveDone(IPlayer player, params Move[] moves)
    {
        Debug.Log("Game_OnMoveDone " + moves.Display());
        for (int i = 0; i < moves.Length; i++)
        {
            jobScheduler.AddJob(new EnumeratorJob(boardView.Move(moves[i]), null));
        }
    }
    #endregion Player Events
}

