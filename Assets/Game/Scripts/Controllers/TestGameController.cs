using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GT.Backgammon.View;
using GT.Backgammon.Logic;
using GT.Backgammon.Player;
using GT.Backgammon;
using GT.Websocket;

public class TestGameController : MonoBehaviour, IGameController
{
    enum GameControl
    {
        // ---------- Pre game controls ----------
        ChangePlayDirection,
        StartGame,
        // ---------- Pre game controls ----------

        // ---------- Normal controls ----------
        RollDice,
        Double,
        EndTurn,
        Undo,
        // ---------- Normal controls ----------

        // ---------- Testing controls ----------
        Edit,
        EndEdit,
        Clear,
        // ---------- Testing controls ----------
    }

    Dictionary<GameControl, GameControlButton> gameControlButtons;

    public TestBoardView boardView;
    public TestPlayerDiceView diceView;
    public ButtonsView buttonsView;
    public GameLoading loadingView;
    public GameDataView dataView;

    public PlayDirection playDirection = PlayDirection.CounterClockwiseRight;
    public bool logDebug = false;
    public bool oneClickMove = false;

    public int testDie1 = 0;
    public int testDie2 = 0;

    private Scheduler scheduler;

    private IGame game;

    #region Unity Methods
    void Start()
    {
        if (UserController.Instance != null && UserController.Instance.gtUser != null)
        {
            if (UserController.Instance.reconnectData == null)
                RequestInGameController.Instance.SendReady();
            else
                StartGameAfterReconnect(UserController.Instance.reconnectData);
        }
        else
        {
            DebugGame();
        }
    }

    void OnEnable()
    {
        BaseGame.OnGameInitialized += Game_OnGameInitialized;

        scheduler = new Scheduler(this);
        RegisterBoardView(boardView);
        diceView.InitView(new TestDiceViewInitData(1f));
        InitializeControlButtons();
    }

    void OnDisable()
    {
        BaseGame.OnGameInitialized -= Game_OnGameInitialized;

        UnregisterBoardView(boardView);
        UnregisterGameEvents(game);
        scheduler = null;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (scheduler != null)
        {
            scheduler.debug = logDebug;
        }
    }
#endif

    void Update()
    {
        if (game != null && game is LiveGame)
        {
            (game as LiveGame).UpdateGame(Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RefreshViews();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            game.StartGame();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            RefreshViews();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Move move = new Move(1, 2, PlayerColor.Black, 1, false);
            Debug.Log("before: " + move);
            string str = Move.Serialize(move);
            Debug.Log("string: " + str);
            Move move2 = Move.Deserialize(str);
            Debug.Log("after: " + move2);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            string board = boardView.GetBoardString();
            PlayerPrefs.SetString("TempGame", board);
            PlayerPrefs.Save();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            game.GameBoard = new Board(PlayerPrefs.GetString("TempGame"));
            RefreshViews();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            scheduler.debug = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            scheduler.debug = false;
        }
    }
    #endregion Unity Methods

    #region Register/Unregister Events
    //private void RegisterLogger(IGame g, GameLogger logger)
    //{
    //    if (logger == null) return;
    //    g.OnGameStarted += p => logger.GameStarted(p.CurrentPlayer.playerColor);
    //    g.OnGameStopped += a => { logger.GameEnded(a.Winner.playerColor); logger.PrintCurrentScore(); };

    //    g.OnEndTurn += (c, n) => logger.TurnEnded();
    //    g.OnDiceSet += (p, d) => logger.DiceSet(d.FirstDie, d.SecondDie);
    //    g.OnMoveDone += (p, m) => logger.MoveDone(m);
    //    if (g is NormalGame)
    //    {
    //        NormalGame ng = g as NormalGame;
    //        ng.OnUndoDone += (p, m) => logger.UndoMove(m);
    //    }
    //}

    private void RegisterGameEvents(IGame g)
    {
        g.OnGameStarted += Game_OnGameStarted;
        g.OnGameStopped += Game_OnGameStopped;
        g.OnGameResumed += Game_OnGameResumed;
        g.OnGameBoardReset += Game_OnGameBoardReset;

        if (g is LiveGame)
        {
            LiveGame ng = g as LiveGame;
            ng.OnTimerChanged += Game_OnTimerChanged;
            ng.OnTurnTimerEnded += Game_OnTurnTimerEnded;
            ng.OnBankTimerEnded += Game_OnBankTimerEnded;
            ng.OnDoubleRequest += Game_OnDoubleRequest;
            ng.OnBetChanged += Game_OnBetChanged;
            ng.OnUndoDone += Game_OnUndoDone;
        }

        g.OnStartTurn += Game_OnStartTurn;
        g.OnEndTurn += Game_OnEndTurn;
        g.OnDiceSet += Game_OnDiceSet;
        g.OnDiceRolled += Game_OnDiceRolled;
        g.OnMoveDone += Game_OnMoveDone;
    }

    private void UnregisterGameEvents(IGame g)
    {
        if (g == null) return;
        g.OnGameStarted -= Game_OnGameStarted;
        g.OnGameStopped -= Game_OnGameStopped;
        g.OnGameResumed -= Game_OnGameResumed;
        g.OnGameBoardReset -= Game_OnGameBoardReset;

        if (g is LiveGame)
        {
            LiveGame ng = g as LiveGame;
            ng.OnTimerChanged -= Game_OnTimerChanged;
            ng.OnTurnTimerEnded -= Game_OnTurnTimerEnded;
            ng.OnBankTimerEnded -= Game_OnBankTimerEnded;
            ng.OnDoubleRequest -= Game_OnDoubleRequest;
            ng.OnUndoDone -= Game_OnUndoDone;
        }

        g.OnStartTurn -= Game_OnStartTurn;
        g.OnEndTurn -= Game_OnEndTurn;
        g.OnDiceSet -= Game_OnDiceSet;
        g.OnDiceRolled -= Game_OnDiceRolled;
        g.OnMoveDone -= Game_OnMoveDone;
    }

    private void RegisterBoardView(IBoardView bv)
    {
        bv.OnView_ExecuteMove += BoardView_OnView_ExecuteMove;

        if (bv is TestBoardView)
            (bv as TestBoardView).OnEditBoard += BoardView_OnView_OnEditBoard;
    }

    private void UnregisterBoardView(IBoardView bv)
    {
        bv.OnView_ExecuteMove -= BoardView_OnView_ExecuteMove;

        if (bv is TestBoardView)
        {
            (bv as TestBoardView).OnEditBoard -= BoardView_OnView_OnEditBoard;
        }
    }
    #endregion Register/Unregister Events

    #region View Events
    private void BoardView_OnView_ExecuteMove(int from, int to)
    {
        if (game is LiveGame)
        {
            if (oneClickMove)
            {
                (game as LiveGame).ApplyBestMove(from);
            }
            else
            {
                (game as LiveGame).ApplyMove(from, to);
            }
        }
    }

    private void BoardView_OnView_OnEditBoard(bool inEdit)
    {
        buttonsView.DeactivateButtons(inEdit ? GameControl.Edit.ToString() : GameControl.EndEdit.ToString());
        buttonsView.ActivateButtons(gameControlButtons[inEdit ? GameControl.EndEdit : GameControl.Edit]);
        if (inEdit == false) // exit edit
        {
            game.ResumeMatch(game.CurrentTurnPlayer, game.CurrentTurnIndex, boardView.GetBoardString(), false);
        }
    }
    #endregion View Events

    #region Game Events
    private void Game_OnGameInitialized(GameInitializedEventArgs args)
    {
        Debug.Log("Game_OnGameInitialized.");
        bool whiteBottom = true;
        if (game is RemoteGame)
        {
            string myId = UserController.Instance.gtUser.Id;
            IPlayer p = args.players[0];
            whiteBottom = p.playerId.Equals(myId) ? p.playerColor == PlayerColor.White : p.playerColor != PlayerColor.White;
        }

        boardView.InitView(.4f, whiteBottom, playDirection);
        dataView.InitGameData(args.players);

        RegisterGameEvents(args.game);
    }

    private void Game_OnGameStarted(GameStartEventArgs args)
    {
        Debug.Log("Game_OnGameStarted. starting " + args.CurrentPlayer.playerColor);
        GameStartedOrResumed(args);
    }

    private void Game_OnGameResumed(GameStartEventArgs args)
    {
        Debug.Log("Game_OnGameResumed");
        GameStartedOrResumed(args);
    }

    private void Game_OnGameStopped(GameStopEventArgs args)
    {
        Debug.Log("Game_OnGameStopped. winner: " + args.Winner);
        buttonsView.DeactivateAllButtons();
        if (game is RemoteGame)
        {
            LiveGameStopEventArgs nArgs = args as LiveGameStopEventArgs;
            scheduler.AddJob(new ActionJob(() =>
                PopupController.Instance.ShowSmallPopup("GAME OVER", new string[] { "WINNER: " + args.Winner.playerId, "CURRENT BALANCE: " + UserController.Instance.wallet.TotalCash.ToString("#,##0.##") },
                    new SmallPopupButton("DOUBLE (Raise to " + (nArgs.Bet * 2).ToString("#,##0.##") + ")", () => FinishGame(true)),
                    new SmallPopupButton("HOME", () => FinishGame(false)))));
        }
        else if (game is LocalGame)
        {
            scheduler.AddJob(new ActionJob(() =>
                PopupController.Instance.ShowSmallPopup("GAME OVER", new string[] { "WINNER: " + args.Winner.playerColor },
                    new SmallPopupButton("PLAY AGAIN", game.StartGame),
                    new SmallPopupButton("DONE", () =>
                    {
                        buttonsView.ActivateButtons(
                            gameControlButtons[GameControl.StartGame],
                            gameControlButtons[GameControl.ChangePlayDirection]);
                    }))));
        }
        else
        {
            StartCoroutine(startAgain());
        }
        scheduler.AddJob(new ActionJob(() => loadingView.HideLoading()));
        scheduler.AddJob(new ActionJob(RefreshViews));
    }

    private void Game_OnGameBoardReset(Board newBoard)
    {
        RefreshViews();
    }

    private void Game_OnDoubleRequest(DoubleRequestEventArgs args)
    {
        Debug.Log("Game_OnDoubleRequest " + args.RequestedPlayer.playerColor);
        buttonsView.DeactivateAllButtons();

        List<SmallPopupButton> buttons = new List<SmallPopupButton>();
        buttons.Add(new SmallPopupButton("YES", () =>
        {
            loadingView.ShowLoading("Waiting for response");
            args.RequestedPlayer.DoubleResponse(DoubleResponse.Yes);
        }));

        if (args.CanDoubleAgain)
        {
            buttons.Add(new SmallPopupButton("DOUBLE AGAIN", () =>
            {
                loadingView.ShowLoading("Waiting for response");
                args.RequestedPlayer.DoubleResponse(DoubleResponse.DoubleAgain);
            }));
        }

        buttons.Add(new SmallPopupButton("NO, GIVE UP", () =>
        {
            loadingView.ShowLoading("Waiting for response");
            args.RequestedPlayer.DoubleResponse(DoubleResponse.GiveUp);
        }));

        scheduler.AddJob(new ActionJob(() =>
        PopupController.Instance.ShowSmallPopup("DOUBLE THE BET?", new string[]
        {
            "YOUR OPPONENT ASKED FOR DOUBLE",
            "IF YOU DECLINE YOU WILL FORFEIT THE GAME.",
            //"CURRENT BALANCE: " + UserController.Instance.wallet.Cash.ToString("#,##0.##"),
            "IF YOU ACCEPT: " + args.NewBet + " FEE: " + args.NewFee,
            "IF YOU DOUBLE AGAIN: " + args.DoubleAgainBet + " Fee: " + args.DoubleAgainFee
        }, buttons.ToArray())));

        scheduler.AddJob(new ActionJob(() => loadingView.HideLoading()));
    }

    private void Game_OnBetChanged(BetChangedEventArgs eventArgs)
    {
        if (eventArgs.IsRequest == false)
        {
            loadingView.HideLoading();
        }
        dataView.SetBetData(Wallet.AmountToString(eventArgs.CurrentBet), Wallet.AmountToString(eventArgs.CurrentFee)
            , Wallet.AmountToString(eventArgs.MaxBet), eventArgs.CubeNum, eventArgs.CantDoublePlayer.playerId);
    }

    private void Game_OnTimerChanged(IPlayer player, PlayerData data)
    {
    }

    private void Game_OnTurnTimerEnded(IPlayer player)
    {
        Debug.Log("Game_OnTurnTimerEnded " + player);
    }

    private void Game_OnBankTimerEnded(IPlayer player)
    {
        Debug.Log("Game_OnBankTimerEnded " + player);
        if (game is LocalGame)
        {
            game.StopGame(game.NextPlayer(player));
        }
    }
    #endregion Game Events

    #region Player Events
    private void Game_OnStartTurn(IPlayer player)
    {
        if (logDebug) Debug.Log("Game_OnStartTurn " + player);
        int turn = game.CurrentTurnIndex;
        int whitePip, blackPip;
        game.GameBoard.GetPip(out whitePip, out blackPip);

        scheduler.AddJob(new ActionJob("TurnStarted", () => boardView.TurnStarted(turn, whitePip, blackPip)));
    }

    private void Game_OnEndTurn(IPlayer currentPlayer, IPlayer nextPlayer)
    {
        if (logDebug) Debug.Log("Game_OnEndTurn " + currentPlayer);

        scheduler.AddJob(new ActionJob("OnEndTurn", () =>
        {
            diceView.HideDice();
            buttonsView.DeactivateButtons(GameControl.Undo.ToString(), GameControl.EndTurn.ToString());
        }));
    }

    private void Game_OnDiceSet(IPlayer player, Dice dice)
    {
        if (logDebug) Debug.Log("Game_OnDiceSet player: " + player);

        Dictionary<int, HashSet<int>> possibleMovesDictionary = player is IHumanPlayer ? (player as IHumanPlayer).GetPossibleMoves() : null;
        GameControlButton[] buttonsToActivate = GetButtonsToActivate(player);

        scheduler.AddJob(new ActionJob(() =>
        {
            boardView.SetMovesDict(possibleMovesDictionary);
            buttonsView.ActivateButtons(buttonsToActivate);
        }));
    }

    private void Game_OnDiceRolled(IPlayer player, Dice dice)
    {
        if (logDebug) Debug.Log("Game_OnDiceRolled dice " + dice + " player: " + player);

        int[] diceList = dice.GetListOfDice();

        scheduler.AddJob(new ActionJob(() => buttonsView.DeactivateButtons(GameControl.RollDice.ToString(), GameControl.Double.ToString())));
        scheduler.AddJob(new EnumeratorJob("OnDiceRolled", diceView.ShowDice(diceList, player), boardView.ShowMovesIndicators));
    }

    private void Game_OnMoveDone(IPlayer player, params Move[] moves)
    {
        if (logDebug) Debug.Log("Game_OnMoveDone " + moves.Display());

        ShowMove(false, player, moves);
    }

    private void Game_OnUndoDone(IPlayer player, params Move[] moves)
    {
        if (logDebug) Debug.Log("Game_OnUndoDone " + moves.Display());

        ShowMove(true, player, moves);
    }
    #endregion Player Events

    #region Private Methods
    private void RollButton()
    {
        if (testDie1 != 0 && testDie2 != 0)
        {
            game.CurrentTurnPlayer.dice.ResetDice();
            game.CurrentTurnPlayer.SetDice(testDie1, testDie2, game.GameBoard);
        }

        game.CurrentTurnPlayer.RollDice();
    }

    private void InitializeControlButtons()
    {
        gameControlButtons = new Dictionary<GameControl, GameControlButton>()
        {
            {GameControl.ChangePlayDirection, new GameControlButton(GameControl.ChangePlayDirection.ToString(), "Play Dir", () => playDirection = TestBoardView.NextPlayDirection(playDirection) ) },
            {GameControl.StartGame, new GameControlButton(GameControl.StartGame.ToString(), "Start Game", StartTestGame) },

            {GameControl.RollDice, new GameControlButton(GameControl.RollDice.ToString(), "Roll", RollButton) },
            {GameControl.Double, new GameControlButton(GameControl.Double.ToString(), "Double", () =>
            {
                loadingView.ShowLoading("Waiting for response");
                buttonsView.DeactivateButtons(GameControl.Double.ToString());
                (game as LiveGame).InitiateDoubleRequest();
            } ) },
            {GameControl.EndTurn, new GameControlButton(GameControl.EndTurn.ToString(), "End Turn" , () => (game as LiveGame).EndTurn() ) },
            {GameControl.Undo, new GameControlButton(GameControl.Undo.ToString(), "Undo", () => (game as LiveGame).UndoMove() ) },

            {GameControl.Edit, new GameControlButton(GameControl.Edit.ToString(), "Edit" , () => (boardView as TestBoardView).editMode = true ) },
            {GameControl.EndEdit, new GameControlButton(GameControl.EndEdit.ToString(), "End Edit",() => (boardView as TestBoardView).editMode = false ) },
            {GameControl.Clear, new GameControlButton(GameControl.Clear.ToString(), "Clear" ) },
        };
    }

    private void GameStartedOrResumed(GameStartEventArgs args)
    {
        if (LoadingController.Instance != null)
            LoadingController.Instance.HideSceneLoading();
        RefreshViews();

        buttonsView.DeactivateAllButtons();
        if (args is LiveGameStartEventArgs)
        {
            LiveGameStartEventArgs nArgs = args as LiveGameStartEventArgs;
            dataView.SetBetData(nArgs.CurrentBet, nArgs.CurrentFee, nArgs.MaxBet, nArgs.CubeNum, nArgs.CantDoublePlayerId);
        }
    }

    private void RefreshViews()
    {
        scheduler.StopAllJobs();
        boardView.ShowBoardState(Board.Serialize(game.GameBoard));
        diceView.HideDice();
    }

    private void ShowMove(bool isUndo, IPlayer player, params Move[] moves)
    {
        if (boardView == null) return;

        scheduler.AddJob(new ActionJob(() => buttonsView.DeactivateButtons(GameControl.Undo.ToString(), GameControl.EndTurn.ToString())));

        Func<Move, IEnumerator> viewMoverFunc;
        if (isUndo)
        {
            viewMoverFunc = boardView.Undo;
        }
        else
        {
            viewMoverFunc = boardView.Move;
        }

        Dictionary<int, HashSet<int>> possibleMovesDictionary = game.CurrentTurnPlayer is IHumanPlayer ? (game.CurrentTurnPlayer as IHumanPlayer).GetPossibleMoves() : null;
        GameControlButton[] buttonsToActivate = GetButtonsToActivate(player);

        for (int i = 0; i < moves.Length; i++)
        {
            Action callback = null;
            Move move = moves[i];
            if (i == moves.Length - 1)
            {
                callback = () =>
                {
                    //diceView.SetDieUsed(move.dice, !isUndo);
                    boardView.SetMovesDict(possibleMovesDictionary);
                    boardView.ShowMovesIndicators();
                    buttonsView.ActivateButtons(buttonsToActivate);
                };
            }
            //else
                //callback = () => { diceView.SetDieUsed(move.dice, !isUndo); };

            scheduler.AddJob(new EnumeratorJob("OnMove", viewMoverFunc(move), callback));
        }
    }

    private GameControlButton[] GetButtonsToActivate(IPlayer player)
    {
        List<GameControlButton> buttons = new List<GameControlButton>();
        if (player is IHumanPlayer)
        {
            IHumanPlayer human = player as IHumanPlayer;
            if (human.CanRoll())
            {
                buttons.Add(gameControlButtons[GameControl.RollDice]);
            }
            else if (human.IsFinishedMoving())
            {
                buttons.Add(gameControlButtons[GameControl.EndTurn]);
            }
            if (human.CanUndo())
            {
                buttons.Add(gameControlButtons[GameControl.Undo]);
            }
            if (human.CanDouble())
            {
                buttons.Add(gameControlButtons[GameControl.Double]);
            }

            if (boardView is TestBoardView && game is LocalGame)
            {
                if ((boardView as TestBoardView).editMode)
                {
                    buttons.Add(new GameControlButton(GameControl.EndEdit.ToString(), "End Edit", () =>
                    {
                        (boardView as TestBoardView).editMode = false;
                        game.GameBoard = new Board(boardView.GetBoardString());
                        RefreshViews();
                    }));
                }
                else
                {
                    buttons.Add(gameControlButtons[GameControl.Edit]);
                }
            }
        }

        return buttons.ToArray();
    }

    private void FinishGame(bool isRematch)
    {
        (game as LiveGame).Rematch(isRematch);
    }

    private void DebugGame()
    {
        // if debug game allowed show buttons
        // if not show error
        buttonsView.ActivateButtons(gameControlButtons[GameControl.StartGame], gameControlButtons[GameControl.ChangePlayDirection]);
    }

    private void StartGameAfterReconnect(ReconnectData reconnectData)
    {
        if (TourneyController.Instance.IsInTourney())
            game = GameFactory.CreateTourneyGame(reconnectData.Players, reconnectData.MaxDoubleAmount, reconnectData.CantDoublePlayer, reconnectData.DoublesCount, reconnectData.MatchId);
        else
            game = GameFactory.CreateRemoteGame(reconnectData.Players, reconnectData.Kind,
                reconnectData.Bet, reconnectData.Fee, reconnectData.MaxBet, reconnectData.CantDoublePlayer, reconnectData.DoublesCount, reconnectData.MatchId);

        (game as RemoteGame).ResumeMatch(reconnectData.CurrentTurnPlayer, reconnectData.MoveId,
            reconnectData.NewBoard, reconnectData.CurrentDice, reconnectData.IsRequestingDouble, reconnectData.IsRolled);
    }
    #endregion Private Methods

    #region Public Methods
    public void StartGameReceived(GameStartedService service)
    {
        Debug.Log("StartGame " + service.RawData);
        Dictionary<string, PlayerData> players = new Dictionary<string, PlayerData>();
        
        string userId = UserController.Instance.gtUser.Id;
        players.Add(userId, new PlayerData(UserController.Instance.gtUser, service.CurrentTurnTime, service.TotalTurnTime, service.TotalBankTime, service.GetPlayerById(userId)));

        string opponentId = UserController.Instance.opponentFoundData.UserID;
        players.Add(opponentId, new PlayerData(UserController.Instance.opponentFoundData, service.CurrentTurnTime, service.TotalTurnTime, service.TotalBankTime, service.GetPlayerById(opponentId)));

        if (TourneyController.Instance.IsInTourney())
            game = GameFactory.CreateTourneyGame(players, service.MaxDoubleAmount, service.WhoCantDouble, 0, service.MatchId);
        else
            game = GameFactory.CreateRemoteGame(players, service.Kind, service.CurrentBet, service.CurrentFee, service.MaxBet, string.Empty, 0, service.MatchId);

        (game as RemoteGame).StartGame(service.CurrentTurnPlayer, service.Dice);
    }
    #endregion Public Methods

    #region Testing
    private void StartTestGame()
    {
        PlayerData p1Data = new PlayerData("sss", null, 20000, 20000, 30000, 30000, PlayerColor.White);
        PlayerData p2Data = new PlayerData("ddd", null, 20000, 20000, 30000, 30000, PlayerColor.Black);

        IPlayer player1 = new HumanPlayer("11", PlayerColor.White, p1Data);
        //IPlayer player1 = new RandomMovePlayer("11", PlayerColor.Black, p1Data, this, 1f, 5f);
        //IPlayer player1 = new TDGammonPlayer("11", PlayerColor.Black, p1Data, this, 1f, 5f, 3000000);
        IPlayer player2 = new TDGammonPlayer("22", PlayerColor.Black, p2Data, 1f, 5f, 3000000);
        //IPlayer player2 = new HumanPlayer("22", PlayerColor.White, p2Data);

        Stake stake = new Stake(AppInformation.MATCH_KIND, 10f, .005f, 20000000f, string.Empty, 0);
        game = GameFactory.CreateLocalGame(stake, player1, player2);

        game.StartGame();
    }

    int total = 99;
    int current = 0;
    private IEnumerator startAgain()
    {
        if (current < total)
        {
            yield return null;
            current++;
            game.StartGame();
        }
    }
    #endregion Testing
}
