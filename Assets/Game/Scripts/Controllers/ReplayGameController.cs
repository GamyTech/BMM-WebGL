using UnityEngine;
using System;
using System.Collections.Generic;
using GT.Backgammon.Logic;
using GT.Backgammon.Player;
using GT.Backgammon;

public class ReplayGameController : MonoBehaviour, IGameController
{
    public delegate void EmptyEvent();
    public event EmptyEvent OnStopPlaying;
    public event EmptyEvent OnStartPlayingForward;
    public event EmptyEvent OnStartPlayingBackward;

    public delegate void IntEvent(int index);
    public event IntEvent OnShowStep;

    public delegate void StatesEvent(string matchId, List<GameState> logs);
    public event StatesEvent OnLoadNewMatch;

    #region Public Members
    private static ReplayGameController m_instance;
    public static ReplayGameController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<ReplayGameController>());} }

    public GT.Assets.BoardItemData board;

    public float MoveDuration = 0.4f;
    public float InterStepDuration = 1.0f;
    public float InfoDisplayDuration = 1.0f;
    public float DiceSpeed = 1.0f;

    public BoardView boardView;
    public GameDiceView diceView;
    public DoubleCubeView doublingCubeView;

    public ReplayMenuView replayMenuView;
    public GlobalInfoView globalInfoView;
    #endregion Public Members

    #region Private Members
    private ReplayMatch MatchInstance;
    private Scheduler jobScheduler;
    public int ShownState { get; private set; }
    private int targetSate = -1;
    private bool instantJump;
    private bool Moving;
    private bool Playing;
    private bool IsWhiteOnBottom;
    private PlayerColor BottomColor;
    #endregion Private Members

    #region Events Register and Unregister
    private void RegisterGameEvents()
    {
        MatchInstance.OnJumpToState += Match_JumpToState;
        MatchInstance.OnGoToNextState += Match_GoToNextState;
        MatchInstance.OnGoToPreviousState += Match_GoToPreviousState;
    }

    private void UnregisterGameEvents()
    {
        if (MatchInstance == null)
            return;

        MatchInstance.OnJumpToState -= Match_JumpToState;
        MatchInstance.OnGoToNextState -= Match_GoToNextState;
        MatchInstance.OnGoToPreviousState -= Match_GoToPreviousState;
    }
    #endregion Events Register and Unregister

    private void Awake()
    {
        jobScheduler = new Scheduler(this);

        boardView.LoadBoard(board);
        boardView.InitView(MoveDuration, IsWhiteOnBottom);
        ResetViews();

        if(LoadingController.Instance != null)
            LoadingController.Instance.HideSceneLoading();

        if(UserController.Instance != null && UserController.Instance.WatchedReplayMatch != null)
            ConnectNewGame(UserController.Instance.WatchedReplayMatch);
    }

    private void OnDestroy()
    {
        UnregisterGameEvents();
        jobScheduler.StopAllJobs();
        jobScheduler = null;
        m_instance = null;
    }

    private void Update()
    {
        if (Moving || MatchInstance == null || ShownState == targetSate)
            return;

        Moving = true;
        if (instantJump)
        {
            instantJump = false;
            MatchInstance.JumpToState(targetSate);
        }
        else
        {
            if(ShownState == -1)
                MatchInstance.JumpToState(0);
            else
            {
                if (targetSate > MatchInstance.CurrentStateIndex)
                    MatchInstance.NextState();
                else
                    MatchInstance.PreviousState();
            }
        }
    }

    #region Server Events
    private void DataBase_GetServerMatchHistoryMoves(GT.Database.GetServerMatchHistoryMoves response)
    {
       ConnectNewGame(response.responseCode == GT.Database.GSResponseCode.OK? response.MatchData : null);
    }
    #endregion Server Events

    #region Logic Events
    private void ConnectNewGame(ReplayMatchData matchData)
    {
        ResetViews();
        if (MatchInstance != null)
            UnregisterGameEvents();
        StopAllCoroutines();
        jobScheduler.StopAllJobs();

        if(matchData == null || string.IsNullOrEmpty(matchData.Winner) || matchData.GameLogs.Count == 0)
        {
            if(UserController.Instance != null && UserController.Instance.WatchedHistoryMatch != null)
                globalInfoView.Say("MatchId " + UserController.Instance.WatchedHistoryMatch.Id + "\nis not visible yet.");
            else
                globalInfoView.Say("This MatchId does not exist\nor is not visible yet.");
            return;
        }

        Dictionary<string, PlayerData> players = matchData.Players;
        PlayerData BottomPlayer = null;
        PlayerData TopPlayer = null;

        if (UserController.Instance != null && UserController.Instance.gtUser != null && players.ContainsKey(UserController.Instance.gtUser.Id))
        {
            string opponentId = players.GetFirstKey() != UserController.Instance.gtUser.Id ? players.GetFirstKey() : players.GetLastKey();
            BottomPlayer = players[UserController.Instance.gtUser.Id];
            TopPlayer = players[opponentId];
        }
        else
        {
            foreach (var p in players)
            {
                if (p.Value.Color == PlayerColor.White)
                    BottomPlayer = p.Value;
                else
                    TopPlayer = p.Value;
            }
        }

        BottomColor = BottomPlayer.Color;
        IsWhiteOnBottom = BottomColor == PlayerColor.White;

        replayMenuView.SetPlayersData(BottomPlayer, TopPlayer);
        replayMenuView.SetStakeData(Wallet.AmountToString(matchData.StartingBet, matchData.Kind), Wallet.AmountToString(matchData.MaxBet, matchData.Kind));
        boardView.InitView(MoveDuration, IsWhiteOnBottom);
        diceView.Init();

        Debug.Log("StartGame " + matchData.ToString());
        MatchInstance = new ReplayMatch(matchData);
        RegisterGameEvents();
        ShownState = -1;
        if (OnLoadNewMatch != null)
            OnLoadNewMatch(MatchInstance.MatchId, MatchInstance.GameStates);

        PlayForward();
    }

    private void Match_JumpToState(GameState state)
    {
        Debug.Log("Jump To log (" + state.LogId + ") : " + state.LogType);
        StopAllCoroutines();
        jobScheduler.StopAllJobs();

        boardView.ShowBoardState(state.CurrentSerializedBoard);

        SetViewToState(state);

        jobScheduler.AddJob(new ActionJob(SaveShownToCurrentState));
    }

    private void Match_GoToNextState(GameState state)
    {
        Debug.Log("Shift to next log (" + state.LogId + ") : " + state.LogType);

        SetViewToState(state);

        if (state.LogType == GameLogType.SendMove && state.CurrentMoves != null)
            ShowMove(false, state.CurrentMoves);

        jobScheduler.AddJob(new ActionJob(SaveShownToCurrentState));
    }

    private void Match_GoToPreviousState(GameState state)
    {
        Debug.Log("Shift to previous log (" + state.LogId + ") : " + state.LogType);

        if (state.NextMoves != null)
            ShowMove(true, state.NextMoves);

        SetViewToState(state);

        jobScheduler.AddJob(new ActionJob(SaveShownToCurrentState));
    }

    private void ShowMove(bool backward, params Move[] moves)
    {
        if (backward)
        {
            for (int i = moves.Length -1; i >= 0; i--)
                jobScheduler.AddJob(new EnumeratorJob(boardView.Undo(moves[i])));
        }
        else
        {
            for (int i = 0; i < moves.Length; i++)
                jobScheduler.AddJob(new EnumeratorJob(boardView.Move(moves[i])));
        }

    }

    #endregion Logic Events

    #region Private Methods

    private void ResetViews()
    {
        globalInfoView.Reset();
        diceView.HideDice();
    }

    private void SetViewToState(GameState state)
    {
        ResetViews();
        if(OnShowStep != null)
            OnShowStep(state.Index);

        SetPips(state.CurrentBoard);
        globalInfoView.SetStateTime(state.RelativeTime);
        float currentBet = MatchInstance.StartingBet * (float)(state.AmountOfDoubles + 1);
        string currentBetString = Wallet.AmountToString(currentBet, MatchInstance.Kind);
        replayMenuView.setCurrentBet(currentBetString);

        if (state.AmountOfDoubles == 0)
            doublingCubeView.Reset();
        else
            doublingCubeView.MoveDoubleDice(state.CanDoublePlayer != BottomColor, 1 << state.AmountOfDoubles);

        IJob job = null;
        switch (state.LogType)
        {
            case GameLogType.SendMove:
                job = new EnumeratorJob(diceView.ShowDice(state.CurrentDice, state.CurrentPlayerColor == BottomColor));
                break;
            case GameLogType.DoubleCubeYes:
                job = new ActionThenWaitJob(() => globalInfoView.Say(GetPlayerName(state.CurrentPlayerId) + " accepted\nto double to " + currentBetString), InterStepDuration);
                break;
            case GameLogType.SendDoubleCubeLogic:
                job = new ActionThenWaitJob(() => globalInfoView.Say(GetPlayerName(state.CurrentPlayerId) + " asked\nto double to " + Wallet.AmountToString(currentBet * 2, MatchInstance.Kind)), InterStepDuration);
                break;
            case GameLogType.StoppedGame:
                job = new ActionJob(() => globalInfoView.Say(state.Winner != "Canceled" ? GetPlayerName(state.Winner) + " Won " + currentBetString : state.Winner));
                break;
            default:
                break;
        }
        if (job != null)
            jobScheduler.AddJob(job);
    }

    private void SaveShownToCurrentState()
    {
        if(Playing)
            jobScheduler.AddJob(new EnumeratorJob(Utils.Wait(InterStepDuration)));

        jobScheduler.AddJob(new ActionJob(() => { ShownState = MatchInstance.CurrentStateIndex; Moving = false; }));
    }

    private void SetPips(Board board)
    {
        Action action = () =>
        {
            int whiteStepsLeft;
            int blackStepsLeft;
            board.GetPip(out whiteStepsLeft, out blackStepsLeft);

            if (IsWhiteOnBottom)
                boardView.SetStepsToWin(blackStepsLeft, whiteStepsLeft);
            else
                boardView.SetStepsToWin(whiteStepsLeft, blackStepsLeft);
        };

        jobScheduler.AddJob(new ActionJob(action));
    }
    #endregion Private Events

    #region Public Methods
    public void RequestMatch(string matchId)
    {
        StartCoroutine(GT.Database.DatabaseKit.GetMatchLog(matchId, DataBase_GetServerMatchHistoryMoves));
    }

    public void MoveToNextState()
    {
        if(MatchInstance != null && targetSate +1 < MatchInstance.GameStates.Count)
        {
            Debug.Log("going to the next state");
            ++targetSate;
            instantJump = false;
        }
        else
            Debug.Log("Impossible to go to the next state");
    }

    public void MoveToPreviousState()
    {
        if (MatchInstance != null && targetSate -1 >= 0)
        {
            Debug.Log("going to the previous state");
            --targetSate;
            instantJump = false;
        }
        else
            Debug.Log("Impossible to go to the previous state");
    }

    public void JumpToState(int index)
    {
        if (MatchInstance != null && index >= 0 && index < MatchInstance.GameStates.Count)
        {
            Debug.Log("Jumping to state " + index);
            targetSate = index;
            instantJump = true;
        }
        else
            Debug.Log("Impossible to jump to state " + index);
    }

    public void JumpToFirstState()
    {
        JumpToState(0);
    }

    public void JumpToLastState()
    {
        if (MatchInstance != null)
            JumpToState(MatchInstance.GameStates.Count - 1);
    }

    public void PlayForward()
    {
        if (MatchInstance != null && targetSate +1 < MatchInstance.GameStates.Count)
        {
            Debug.Log("Playing Forward");
            if (!Playing && OnStartPlayingForward != null)
                OnStartPlayingForward();
            Playing = true;
            targetSate = MatchInstance.GameStates.Count - 1;
            instantJump = false;
        }
        else
            Debug.Log("Play Forward Impossible");
    }

    public void PlayBackward()
    {
        if (MatchInstance != null && targetSate - 1 >= 0)
        {
            Debug.Log("Playing Backward");
            Playing = true;
            if (!Playing && OnStartPlayingBackward != null)
                OnStartPlayingBackward();
            targetSate = 0;
            instantJump = false;
        }
        else
            Debug.Log("Play Backward Impossible");
    }

    public void StopPlaying()
    {
        if (MatchInstance != null)
        {
            if (OnStopPlaying != null)
                OnStopPlaying();
            targetSate = MatchInstance.CurrentStateIndex;
            Playing = false;
        }
    }

    public void SetStepSpeed(float moveDuration, float interStepDuration, float displayDuration, float diceSpeed)
    {
        MoveDuration = moveDuration;
        InterStepDuration = interStepDuration;
        InfoDisplayDuration = displayDuration;

        if(MatchInstance != null)
        {
            diceView.SetDiceAnimationSpeed(diceSpeed);
            boardView.ChangeMoveDuration(this.MoveDuration);
        }
    }

    public string GetPlayerName(string id)
    {
        PlayerData player;
        if (MatchInstance.Players.TryGetValue(id, out player))
            return player.UserName;
        Debug.Log(id + " is not in the players list");
        return id;
    }
    #endregion Public Methods
}

