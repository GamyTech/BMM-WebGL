using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GT.Backgammon.Logic;
using GT.Backgammon.Player;
using GT.Websocket;
using GT.Store;
using UnityEngine.Events;

public class RemoteGameController : MonoBehaviour, IGameController
{
    #region Public Members
    private static RemoteGameController m_instance;
    public static RemoteGameController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<RemoteGameController>()); } private set { m_instance = value; } }

    private Dictionary<string, string> PendingRollingItems = new Dictionary<string, string>();

    public BoardView boardView;
    public DoubleCubeView doublingCubeView;
    public DoubleTheBetView doubleTheBetView;
    public WinView winView;
    public TourneyWinView tourneyWinView;
    public WaitingForDoubleView waitingForDoubleView;
    public AfterRollingButtonsView afterRollingButtonsView;
    public GameMenuView gameMenuView;
    public GameDiceView diceView;
    public GameStoreView StoreView;
    public ChatView chatView;
    public GameSettingView settingView;
    public MascotsView mascotView;
    public BlessingView blessingView;
    public BeforeRollingButtonsView beforeRollingButtonsView;
    public Swipe swipeView;
    public MatchIDView matchIdView;

    public bool isAuto;
    public bool isDouble;
    public bool isRoll;
    public bool HasStarted;

    public float startGameTimeout = 15f;
    #endregion Public Members

    #region Private Members
    private RemoteGame GameInstance;
    private List<UpdatedUsedItem> ItemsChanged = new List<UpdatedUsedItem>();
    private bool pendingChangementSignal;
    private Scheduler jobScheduler;
    private Coroutine startMatchTimer;
    private bool isStarted;
    #endregion Private Members

    #region Events Register and Unregister
    private void RegisterGameEvents()
    {
        boardView.OnView_ExecuteMove += BoardView_OnView_ExecuteMove;

        GameInstance.OnGameStarted += GameStartedOrResumed;
        GameInstance.OnGameResumed += GameStartedOrResumed;
        GameInstance.OnGameStopped += Game_OnGameStopped;
        GameInstance.OnStartTurn += Game_OnStartTurn;
        GameInstance.OnEndTurn += Game_OnEndTurn;
        GameInstance.OnDiceSet += Game_OnDiceSet;
        GameInstance.OnDiceRolled += Game_OnDiceRolled;
        GameInstance.OnMoveDone += Game_OnMoveDone;
        GameInstance.OnUndoDone += Game_OnUndoDone;
        GameInstance.OnTimerChanged += Game_OnTimerChanged;
        GameInstance.OnTurnTimerEnded += Game_OnTurnTimerEnded;
        GameInstance.OnDoubleRequest += Game_OnDoubleRequest;
        GameInstance.OnBetChanged += Game_OnBetChanged;

        if (GameInstance is TourneyGame)
        {
            (GameInstance as TourneyGame).OnTotalTimerChanged += Game_OnTotalTimerChanged;
        }
    }

    private void UnregisterGameEvents()
    {
        boardView.OnView_ExecuteMove -= BoardView_OnView_ExecuteMove;

        if (GameInstance == null)
            return;

        GameInstance.OnGameStarted -= GameStartedOrResumed;
        GameInstance.OnGameResumed -= GameStartedOrResumed;
        GameInstance.OnGameStopped -= Game_OnGameStopped;
        GameInstance.OnStartTurn -= Game_OnStartTurn;
        GameInstance.OnEndTurn -= Game_OnEndTurn;
        GameInstance.OnDiceSet -= Game_OnDiceSet;
        GameInstance.OnDiceRolled -= Game_OnDiceRolled;
        GameInstance.OnMoveDone -= Game_OnMoveDone;
        GameInstance.OnUndoDone -= Game_OnUndoDone;
        GameInstance.OnTimerChanged -= Game_OnTimerChanged;
        GameInstance.OnTurnTimerEnded -= Game_OnTurnTimerEnded;
        GameInstance.OnDoubleRequest -= Game_OnDoubleRequest;
        GameInstance.OnBetChanged -= Game_OnBetChanged;

        if (GameInstance is TourneyGame)
        {
            (GameInstance as TourneyGame).OnTotalTimerChanged -= Game_OnTotalTimerChanged;
        }
    }
    #endregion Events Register and Unregister

    private void Awake()
    {
        jobScheduler = new Scheduler(this);

        isStarted = false;

        WebSocketKit.Instance.ServiceEvents[ServiceId.MatchStarted] += WebSocketKit_MatchStarted;
        WebSocketKit.Instance.ServiceEvents[ServiceId.GameStarted] += WebSocketKit_GameStarted;

        WebSocketKit.Instance.ServiceEvents[ServiceId.UpdateUsedItems] += WebSocketKit_UpdateUsedItems;

        UnityAction startingAction;
        if (UserController.Instance.reconnectData == null)
            startingAction = () => { RequestInGameController.Instance.SendReady(); };
        else
            startingAction = () => StartGameAfterReconnect(UserController.Instance.reconnectData);

        StartCoroutine(Utils.WaitForCoroutine(InstallGameAsset, startingAction));
        startMatchTimer = StartCoroutine(MatchWaitingTimeout());
    }

    private void OnDestroy()
    {
        UnregisterGameEvents();
        
        WebSocketKit.Instance.ServiceEvents[ServiceId.MatchStarted] -= WebSocketKit_MatchStarted;
        WebSocketKit.Instance.ServiceEvents[ServiceId.GameStarted] -= WebSocketKit_GameStarted;

        WebSocketKit.Instance.ServiceEvents[ServiceId.UpdateUsedItems] -= WebSocketKit_UpdateUsedItems;
        jobScheduler.StopAllJobs();
        jobScheduler = null;
        m_instance = null;
    }

    private void Update()
    {
        if (GameInstance != null)
            GameInstance.UpdateGame(Time.deltaTime);
    }

    private IEnumerator MatchWaitingTimeout()
    {
        yield return new WaitForSeconds(startGameTimeout);

        PopupController.Instance.ShowSmallPopup("Slow connection", new string[] { "You are waiting for too long.", "Please try to reconnect" },
            new SmallPopupButton[] { new SmallPopupButton("Reconnect", Reconnect), new SmallPopupButton("Keep waiting", ResetStartTimeout) });        
    }

    private void Reconnect()
    {
        NetworkController.Instance.Reconnect();
    }

    private void ResetStartTimeout()
    {
        if (!isStarted)
        {
            PopupController.Instance.HideSmallPopup();
            startMatchTimer = StartCoroutine(MatchWaitingTimeout());
        }
    }

    private void StopWitingForGame()
    {
        isStarted = true;
        StopCoroutine(startMatchTimer);
        PopupController.Instance.HideSmallPopup();
    }

    private void StartGameAfterReconnect(ReconnectData recoData)
    {
        Debug.Log("Resume game = " + recoData.ToString());

        StopWitingForGame();

        if (TourneyController.Instance.IsInTourney())
            CreateTourneyGame(recoData.MatchId, recoData.Players, recoData.MaxDoubleAmount, recoData.CantDoublePlayer, recoData.DoublesCount);
        else
            CreateGame(recoData.MatchId, recoData.Players, recoData.Kind, recoData.Bet, recoData.Fee, recoData.MaxBet, recoData.CantDoublePlayer, recoData.DoublesCount);

        PlayerData opponent = GameInstance.OpponentPlayer.playerData;
        UserController.Instance.opponentFoundData = new PlayerFoundData(GameInstance.OpponentPlayer.playerId, opponent.UserName, opponent.Avatar, opponent.IsBot, opponent.SelectedItems);

        GameInstance.ResumeMatch(recoData.CurrentTurnPlayer, recoData.MoveId, recoData.NewBoard, recoData.CurrentDice, recoData.IsRequestingDouble, recoData.IsRolled);
    }

    #region Websocket Events
    public void WebSocketKit_GameStarted(Service service)
    {
        StopWitingForGame();

        GameStartedService startData = service as GameStartedService;
         Dictionary<string, PlayerData> players = GetPlayersFrom(startData);

         CreateGame(startData.MatchId, players, startData.Kind, startData.CurrentBet, startData.CurrentFee, startData.MaxBet, startData.WhoCantDouble, 0);
         SaveItemsAndStart(startData);
    }

    public void WebSocketKit_MatchStarted(Service service)
    {
        StopCoroutine(startMatchTimer);

        GameStartedService startData = service as GameStartedService;
        Dictionary<string, PlayerData> players = GetPlayersFrom(startData);

        CreateTourneyGame(startData.MatchId, players, startData.MaxDoubleAmount, startData.WhoCantDouble, 0);
        SaveItemsAndStart(startData);
    }

    public void WebSocketKit_UpdateUsedItems(Service service)
    {
        UpdateUsedItems NewUsedItems = service as UpdateUsedItems;

        for (int x = 0; x < NewUsedItems.ItemList.Count; ++x)
        {
            Enums.StoreType storeType;
            if (Utils.TryParseEnum(NewUsedItems.ItemList[x], out storeType))
            {
                if (storeType == Enums.StoreType.LuckyItems)
                {
                    bool sentByUser = NewUsedItems.Sender == UserController.Instance.gtUser.Id;
                    bool GiftFromOpponent = !sentByUser && NewUsedItems.ItemList[x].Gifted;
                    IPlayer updatedPlayer = (sentByUser && !NewUsedItems.ItemList[x].Gifted) || GiftFromOpponent ? GameInstance.UserPlayer : GameInstance.OpponentPlayer;
                    updatedPlayer.UpdateSelectedItems(Enums.StoreType.LuckyItems, new string[1] { NewUsedItems.ItemList[x].Id });

                    if (GiftFromOpponent)
                    {
                        Store store = UserController.Instance.gtUser.StoresData.GetStore(storeType);
                        if (store != null) store.selected.SelectItem(true, store.GetItemById(NewUsedItems.ItemList[x].Id));
                    }

                    jobScheduler.AddJob(new EnumeratorJob(gameMenuView.ShowNewLuckyItem(NewUsedItems.ItemList[x].Id, sentByUser, NewUsedItems.ItemList[x].Gifted)));
                }
            }
            else
                Debug.LogError("StoreType not found");
        }
    }
    #endregion Websocket Events

    #region Logic Events
    private void GameStartedOrResumed(GameStartEventArgs args)
    {
        boardView.InitView(0.4f, GameInstance.UserPlayer.playerColor == PlayerColor.White);
        boardView.ShowBoardState(Board.Serialize(GameInstance.GameBoard));

        ResetViews();

        LiveGameStartEventArgs nArgs = args as LiveGameStartEventArgs;
        AddPendingRollingItemIfExists(Enums.StoreType.Dices);
        AddPendingRollingItemIfExists(Enums.StoreType.Blessings);

        Debug.Log("GameStartedOrResumed : " + nArgs.CubeNum + " " + nArgs.CantDoublePlayerId);

        string doubleBet = Wallet.AmountToString(nArgs.BetValue * 2, nArgs.Kind);
        string doubleFee = Wallet.AmountToString(nArgs.FeeValue * 2, nArgs.Kind);
        beforeRollingButtonsView.SetDoubleButton(doubleBet, doubleFee);

        gameMenuView.InitStakeData(Wallet.AmountToString(nArgs.BetValue, nArgs.Kind), Wallet.AmountToString(nArgs.FeeValue, nArgs.Kind), nArgs.MaxBet);
        gameMenuView.InitPlayersData(GameInstance.UserPlayer.playerData, GameInstance.OpponentPlayer.playerData);
        mascotView.InitPlayersData(GameInstance.UserPlayer, GameInstance.OpponentPlayer);

        if (TourneyController.Instance.IsInTourney())
            tourneyWinView.InitPlayersData(GameInstance.UserPlayer, GameInstance.OpponentPlayer);
        else
            winView.InitPlayersData(GameInstance.UserPlayer, GameInstance.OpponentPlayer);

        string diceUser;
        GameInstance.UserPlayer.playerData.RollingItems.TryGetValue(Enums.StoreType.Dices.ToString(), out diceUser);
        string diceOpponent;
        GameInstance.OpponentPlayer.playerData.RollingItems.TryGetValue(Enums.StoreType.Dices.ToString(), out diceOpponent);

        diceView.Init(diceUser, diceOpponent);
        SetPips();

        LoadingController.Instance.HideSceneLoading();
    }

    private void Game_OnGameStopped(GameStopEventArgs args)
    {
        HasStarted = false;
        StopAllCoroutines();
        CleanMatchData();

        RemoteGameStopEventArgs nArgs = args as RemoteGameStopEventArgs;
        Debug.Log("Game_OnGameStopped " + Time.time + " args.isCanceled: " + nArgs.IsCanceled);

        PopupController.Instance.HideSmallPopup();
        HideAllViews();

        if (nArgs.IsCanceled)
        {
            if (TourneyController.Instance.IsInTourney() && TourneyController.Instance.IsFinishedByTime())
                PopupController.Instance.ShowSmallPopup("Game Cancelled",
                new string[] { "The tournament has ended", "All the unfinished games were cancelled" },
                new SmallPopupButton("Back To Home", ReturnHome));
            else
                PopupController.Instance.ShowSmallPopup("Game Cancelled",
                new string[] { "Our support staff will review this match", "and will refund you if it was a technical error" },
                new SmallPopupButton("Back To Home", ReturnHome));
        }
        else
        {
            PlayerStats localPlayer = nArgs.Player1Stats.id == UserController.Instance.gtUser.Id ? nArgs.Player1Stats : nArgs.Player2Stats;
            PlayerStats remotePlayer = localPlayer.id == nArgs.Player1Stats.id ? nArgs.Player2Stats : nArgs.Player1Stats;

            if (TourneyController.Instance.IsInTourney())
            {
                TourneyGameStopEventArgs tArgs = nArgs as TourneyGameStopEventArgs;
                tourneyWinView.Show(localPlayer, remotePlayer, args.Winner);
                tourneyWinView.ShowTourneyView(
                    TourneyController.Instance.GetUserScore(localPlayer.id), tArgs.UserScores,
                    TourneyController.Instance.GetUserScore(remotePlayer.id), tArgs.OpponentScores);
            }
            else
            {
                winView.Show(localPlayer, remotePlayer, args.Winner);
                winView.ShowWinAmount(nArgs.Kind, nArgs.Bet * 2, nArgs.Fee, nArgs.Loyalty);
            }
        }
    }

    private void Game_OnBetChanged(BetChangedEventArgs eventArgs)
    {
        doublingCubeView.MoveDoubleDice(eventArgs.CantDoublePlayer.playerId == GameInstance.UserPlayer.playerId, eventArgs.CubeNum);
        beforeRollingButtonsView.SetDoubleCubeValue(eventArgs.CubeNum * 2);

        if (eventArgs.IsRequest == false)
            waitingForDoubleView.StopWaitForOpponent();

        string doubleBet = eventArgs.CurrentPostFix + Wallet.AmountToString(eventArgs.CurrentBet * 2);
        string doubleFee = eventArgs.CurrentPostFix + Wallet.AmountToString(eventArgs.CurrentFee * 2, 2);

        beforeRollingButtonsView.Reset();
        beforeRollingButtonsView.PlayerCanDouble((GameInstance.UserPlayer as HumanPlayer).CanDouble());
        beforeRollingButtonsView.SetDoubleButton(doubleBet, doubleFee);
        gameMenuView.UpdateBetAmounts(eventArgs.CurrentPostFix + Wallet.AmountToString(eventArgs.CurrentBet));
    }

    private void Game_OnDoubleRequest(DoubleRequestEventArgs eventArgs)
    {
        HideAllViews();

        Action yes = () =>
        {
            eventArgs.RequestedPlayer.DoubleResponse(DoubleResponse.Yes);
            beforeRollingButtonsView.Double(false);
            doubleTheBetView.Hide();
        };

        Action no = () =>
        {
            eventArgs.RequestedPlayer.DoubleResponse(DoubleResponse.GiveUp);
            doubleTheBetView.Hide();
        };

        Action doubleAgain = null;
        if (eventArgs.CanDoubleAgain)
        {
            doubleAgain = () =>
            {
                eventArgs.RequestedPlayer.DoubleResponse(DoubleResponse.DoubleAgain);
                PlayerData opponentData = GameInstance.OpponentPlayer.playerData;
                StartCoroutine(waitingForDoubleView.StartWaitForOpponent(opponentData.CurrentBankTime + opponentData.TotalTurnTime));
                beforeRollingButtonsView.Double(false);
                doubleTheBetView.Hide();
            };
        }

        jobScheduler.AddJob(new ActionJob(() => doubleTheBetView.Init(GameInstance.players, eventArgs, yes, no, doubleAgain)));
    }

    private void Game_OnTurnTimerEnded(IPlayer player)
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.BankTime);
        SettingsController.Instance.Vibrate();
        gameMenuView.DimUserPic(player, player is HumanPlayer);
    }

    private void Game_OnTimerChanged(IPlayer player, PlayerData data)
    {
        if (player is IHumanPlayer)
            gameMenuView.PlayerTimer(data);
        else
            gameMenuView.OpponentTimer(data);
    }

    private void Game_OnTotalTimerChanged(IPlayer player, PlayerData data)
    {
        if (player is IHumanPlayer)
            gameMenuView.PlayerTotalTimer(data);
        else
            gameMenuView.OpponentTotalTimer(data);
    }
    #endregion Logic Events

    #region Player Events
    private void Game_OnDiceRolled(IPlayer player, Dice dice)
    {
        bool isUser = player is IHumanPlayer;
        SetCurrentRolingItems(isUser, player.playerData.RollingItems);

        Action callback = () => boardView.ShowMovesIndicators();
        if (isUser)
        {
            callback += () => { beforeRollingButtonsView.SetActive(false); };

            if ((player as IHumanPlayer).IsFinishedMoving())
                callback += () => { afterRollingButtonsView.DisplayDoneButton(true); };
        }

        jobScheduler.AddJob(new EnumeratorJob(blessingView.ShowBlessing(isUser)));
        jobScheduler.AddJob(new EnumeratorJob(diceView.ShowDice(dice.GetListOfDice(), isUser), callback));
    }

    private void Game_OnDiceSet(IPlayer player, Dice dice)
    {
        Dictionary<int, HashSet<int>> possibleMovesDictionary = player is IHumanPlayer ? (player as IHumanPlayer).GetPossibleMoves() : null;
        jobScheduler.AddJob(new ActionJob(() =>
        {
            boardView.SetMovesDict(possibleMovesDictionary);
            PlayerTurnActions();
        }));
    }

    private void Game_OnStartTurn(IPlayer player)
    {
        Action startTurnAction = () =>
        {
            bool isUser = player is IHumanPlayer;
            gameMenuView.ActivatePlayersTime(isUser);
            swipeView.SetActive(!isUser);
            beforeRollingButtonsView.PlayerCanDouble((GameInstance.UserPlayer as HumanPlayer).CanDouble());
            beforeRollingButtonsView.ActivateArrowButton(!isUser);

            Debug.Log("Game_OnStartTurn CanDouble? " + (GameInstance.UserPlayer as HumanPlayer).CanDouble());

            if (isUser)
            {
                if (!isRoll && !isAuto)
                {
                    beforeRollingButtonsView.DisableRollToggle();
                    beforeRollingButtonsView.SetActive(true);

                    if (!isDouble)
                        beforeRollingButtonsView.WaitingForPlayerIndicators();
                }
            }
            else
            {
                beforeRollingButtonsView.DisableRollToggle();
                beforeRollingButtonsView.DisableDoubleToggle();
                beforeRollingButtonsView.SetActive(true);
                beforeRollingButtonsView.StopIndicators();
            }
        };
        jobScheduler.AddJob(new ActionJob(startTurnAction));
    }

    private void Game_OnEndTurn(IPlayer currentPlayer, IPlayer nextPlayer)
    {
        Action action = () => diceView.HideDice();

        if (currentPlayer is IHumanPlayer)
            action += () => { ResetAfterRollingButtons(currentPlayer as IHumanPlayer); } ;

        jobScheduler.AddJob(new ActionJob(action));
    }

    private void Game_OnMoveDone(IPlayer player, params Move[] moves)
    {
        Debug.Log("Game_OnMoveDone " + moves.Display());

        ShowMove(false, player, moves);
    }

    private void Game_OnUndoDone(IPlayer player, params Move[] moves)
    {
        ShowMove(true, player, moves);
    }

    private void ShowMove(bool isUndo, IPlayer player, params Move[] moves)
    {
        Func<Move, IEnumerator> MoverAction;

        if (isUndo)
            MoverAction = boardView.Undo;
        else
            MoverAction = boardView.Move;

        bool isHuman = player is IHumanPlayer;

        for (int i = 0; i < moves.Length; i++)
        {
            Action callback = null;

            if (isHuman && i == moves.Length - 1)
            {
                IHumanPlayer human = player as IHumanPlayer;
                callback = () =>
                {
                    boardView.SetMovesDict(human.GetPossibleMoves());
                    boardView.ShowMovesIndicators();
                    ResetAfterRollingButtons(human);
                };
            }

            callback += SetPips;

            jobScheduler.AddJob(new EnumeratorJob(MoverAction(moves[i]), callback));
        }
    }

    #endregion Player Events

    #region Private Methods

    private Dictionary<string, PlayerData> GetPlayersFrom(GameStartedService startData)
    {
        Dictionary<string, PlayerData> players = new Dictionary<string, PlayerData>();

        string userId = UserController.Instance.gtUser.Id;
        players.Add(userId, new PlayerData(UserController.Instance.gtUser, startData.CurrentTurnTime, startData.TotalTurnTime, startData.TotalBankTime, startData.GetPlayerById(userId)));

        string opponentId = UserController.Instance.opponentFoundData.UserID;
        players.Add(opponentId, new PlayerData(UserController.Instance.opponentFoundData, startData.CurrentTurnTime, startData.TotalTurnTime, startData.TotalBankTime, startData.GetPlayerById(opponentId)));
        return players;
    }

    private void CreateGame(int matchId, Dictionary<string, PlayerData> players, Enums.MatchKind kind, float bet, float fee, float maxBet, string cantDoublePlayer, int doublesCount)
    {
        PrepareToGame(matchId);
        GameInstance = GameFactory.CreateRemoteGame(players, kind, bet, fee, maxBet, cantDoublePlayer, doublesCount, matchId);
        RegisterGameEvents();
    }

    private void CreateTourneyGame(int matchId, Dictionary<string, PlayerData> players, int maxDoubles, string cantDoublePlayer, int doublesCount)
    {
        PrepareToGame(matchId);
        GameInstance = GameFactory.CreateTourneyGame(players, maxDoubles, cantDoublePlayer, doublesCount, matchId);
        RegisterGameEvents();
    }

    private void PrepareToGame(int matchId)
    {
        if (HasStarted)
            return;
        HasStarted = true;

        matchIdView.SetMatchID(matchId);
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.LastMatchId, matchId);

        TrackingKit.OnTotalMatchesChanged(++UserController.Instance.gtUser.TotalMatchesCount);

        if (GameInstance != null)
            UnregisterGameEvents();
    }

    private void SaveItemsAndStart(GameStartedService startData)
    {
        foreach (var usedItem in GameInstance.UserPlayer.playerData.SelectedItems)
            if (usedItem.Value != null)
                for (int x = 0; x < usedItem.Value.Length; ++x)
                    SaveNewItemChanging(usedItem.Key, usedItem.Value[x], false, true);

        GameInstance.StartGame(startData.CurrentTurnPlayer, startData.Dice);
    }

    private IEnumerator InstallGameAsset()
    {
        string selectedBoard = UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.Boards).selected.selectedIds[0];
        yield return boardView.StartCoroutine(boardView.LoadBoard(selectedBoard));
    }

    private void ResetViews()
    {
        HideAllViews();
        beforeRollingButtonsView.Reset();
        winView.Reset();
        tourneyWinView.Reset();
        gameMenuView.Reset();

        jobScheduler.StopAllJobs();
    }

    private void ResetAfterRollingButtons(IHumanPlayer player)
    {
        afterRollingButtonsView.DisplayDoneButton((player).IsFinishedMoving());
        afterRollingButtonsView.DisplayUndoButton((player).CanUndo());
    }

    private IEnumerator SendChangedItems(bool instant)
    {
        if (instant || !pendingChangementSignal)
        {
            pendingChangementSignal = true;
            if(!instant)
                yield return new WaitForSeconds(5.0f);

            Dictionary<string, object> storeList = new Dictionary<string, object>();
            foreach (UpdatedUsedItem change in ItemsChanged)
            {
                string key = change.Type.ToString();
                if (storeList.ContainsKey(key))
                {
                    storeList[key] = new string[] { change.Id }.MergeArray((string[])storeList[key]);
                }
                else
                {
                    storeList.Add(key, new string[] { change.Id });
                }
            }
            RequestInGameController.Instance.SendItemChanged(storeList);
            ItemsChanged.Clear();
            pendingChangementSignal = false;
        }
    }

    private void AddPendingRollingItemIfExists(Enums.StoreType storeType)
    {
        Store store = UserController.Instance.gtUser.StoresData.GetStore(storeType);
        if (store == null) return;

        string id = store.selected.GetFirstSelectedId();
        if (!string.IsNullOrEmpty(id))
            SavePendingRollingItem(storeType, id);
    }

    private void SetCurrentRolingItems(bool islocalPlayer, Dictionary<string, string> rollingItemIds)
    {
        if (rollingItemIds == null)
            return;

        string item;
        if (rollingItemIds.TryGetValue(Enums.StoreType.Dices.ToString(), out item) && !string.IsNullOrEmpty(item))
            diceView.SetDiceId(islocalPlayer, item);
        
        if (rollingItemIds.TryGetValue(Enums.StoreType.Blessings.ToString(), out item) && !string.IsNullOrEmpty(item))
            blessingView.SetBlessingId(islocalPlayer, item);
    }

    private void BoardView_OnView_ExecuteMove(int from, int to)
    {
        if (to == -1)
            GameInstance.ApplyBestMove(from);
        else
            GameInstance.ApplyMove(from, to);
    }

    private void SetPips()
    {
        int whiteStepsLeft;
        int blackStepsLeft;
        GameInstance.GameBoard.GetPip(out whiteStepsLeft, out blackStepsLeft);

        if(GameInstance.UserPlayer.playerColor == PlayerColor.White)
            boardView.SetStepsToWin(blackStepsLeft, whiteStepsLeft);
        else
            boardView.SetStepsToWin(whiteStepsLeft, blackStepsLeft);
    }

    private void HideAllViews()
    {
        afterRollingButtonsView.DisplayDoneButton(false);
        afterRollingButtonsView.DisplayUndoButton(false);
        beforeRollingButtonsView.SetActive(false);
        StoreView.SetActive(false);
        chatView.Hide();
        settingView.Hide();
        diceView.HideDice();
        doubleTheBetView.Hide();
        waitingForDoubleView.StopWaitForOpponent();
    }
    #endregion Private Events

    #region Public Methods
    public void PlayerTurnActions()
    {
        if (GameInstance.CurrentTurnPlayer is IHumanPlayer)
        {
            if (isDouble)
            {
                isDouble = false;
                PlayerData opponentData = GameInstance.OpponentPlayer.playerData;
                StartCoroutine(waitingForDoubleView.StartWaitForOpponent(opponentData.CurrentBankTime + opponentData.TotalTurnTime));
                GameInstance.InitiateDoubleRequest();
                beforeRollingButtonsView.SetActive(false);
                beforeRollingButtonsView.DisableDoubleToggle();
            }

            else if (isRoll || isAuto)
            {
                isRoll = false;
                beforeRollingButtonsView.SetActive(false);
                GameInstance.InitiateRollingRequest(PendingRollingItems);
                PendingRollingItems.Clear();
            }
        }
    }

    public void SavePendingRollingItem(Enums.StoreType type, string Id)
    {
        PendingRollingItems.AddOrOverrideValue(type.ToString(), Id);
    }

    public void SaveNewItemChanging(Enums.StoreType type, string itemID, bool gifted, bool instant = false)
    {
        UpdatedUsedItem item = new UpdatedUsedItem(type, itemID, gifted);
        bool overrided = false;
        for (int x = 0; x < ItemsChanged.Count; ++x)
        {
            if (ItemsChanged[x].Type == type)
            {
                ItemsChanged[x] = item;
                overrided = true;
                break;
            }
        }

        if(!overrided)
            ItemsChanged.Add(item);

        StartCoroutine(SendChangedItems(instant));
    }

    public void ChangeBoard(string itemId)
    {
        boardView.ChangeBoard(itemId);
    }

    public void UndoMove()
    {
        GameInstance.UndoMove();
    }

    public void EndTurn()
    {
        GameInstance.EndTurn();
    }

    public void Surrender()
    {
        GameInstance.Surrender();
    }

    public void ReturnHome()
    {
        GameInstance.Rematch(false);
        SceneController.Instance.ChangeScene(SceneController.SceneName.Menu);
    }

    public void Rematch()
    {
        UnregisterGameEvents();
        GameInstance.Rematch(true);
    }

    public void AddFriend()
    {
        // == TODO == //
    }

    public static void CleanMatchData()
    {
        GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.LastMatchId);
        GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.LastMatchBotDoublesCount);
    }

    public int GetCurrentTurnIndex()
    {
        return GameInstance.CurrentTurnIndex;
    }
    
    public bool IsDoubleCube()
    {
        return GameInstance.isDoubleCube;
    }
    #endregion Public Methods

}

