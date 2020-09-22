using UnityEngine;
using GT.Backgammon.Player;
using System.Collections.Generic;
using GT.Websocket;
using System.Threading;
using System;

namespace GT.Backgammon.Logic
{
    public class RemoteGame : LiveGame
    {
        protected string nextDice;
        protected int MatchId;
        protected bool IsCanceled;
        protected PlayerStats player1Stats;
        protected PlayerStats player2Stats;
        protected int Loyalty;
        protected int UserScores;
        protected int OpponentScores;

        private IPlayer m_userPlayer;
        public IPlayer UserPlayer {
            get {
                if (m_userPlayer != null)
                    return m_userPlayer;
                if (UserController.Instance != null && UserController.Instance.gtUser != null)
                    m_userPlayer = GetPlayerFromId(UserController.Instance.gtUser.Id);
                return m_userPlayer;
            }
        }
        private IPlayer m_opponentPlayer;
        public IPlayer OpponentPlayer { get { return m_opponentPlayer ?? (m_opponentPlayer = NextPlayer(UserPlayer)); } }
        public bool isDoubleCube { get; private set; }

        public RemoteGame(Stake stake, int matchId, params IPlayer[] players) : base(stake, players)
        {
            MatchId = matchId;
            WebSocketKit.Instance.ServiceEvents[ServiceId.StoppedGame] += WebSocketKit_StoppedGame;
            WebSocketKit.Instance.ServiceEvents[ServiceId.MatchTimeOut] += WebSocketKit_StoppedGame;
            WebSocketKit.Instance.ServiceEvents[ServiceId.PlayerRolled] += WebSocketKit_PlayerRolled;
            WebSocketKit.Instance.ServiceEvents[ServiceId.ItemsChanged] += WebSocketKit_ItemsChanged;
            WebSocketKit.Instance.ServiceEvents[ServiceId.SendMoveBroadCast] += WebSocketKit_SendMoveBroadCast;
            WebSocketKit.Instance.ServiceEvents[ServiceId.DoubleCubeRequest] += WebSocketKit_DoubleCubeRequest;
            WebSocketKit.Instance.ServiceEvents[ServiceId.StartBankTime] += WebSocketKit_StartBankTime;

            WebSocketKit.Instance.AckEvents[RequestId.GetCurrentMoveState] += WebSocketKit_OnSyncGameStatus;
        }

        public void InitPlayersByOrder()
        {
            m_userPlayer = players[0];
            m_opponentPlayer = players[1];
        }

        public virtual void Unregister()
        {
            Debug.Log("Game destroyed");

            StopSyncLoop();

            WebSocketKit.Instance.ServiceEvents[ServiceId.StoppedGame] -= WebSocketKit_StoppedGame;
            WebSocketKit.Instance.ServiceEvents[ServiceId.MatchTimeOut] -= WebSocketKit_StoppedGame;
            WebSocketKit.Instance.ServiceEvents[ServiceId.PlayerRolled] -= WebSocketKit_PlayerRolled;
            WebSocketKit.Instance.ServiceEvents[ServiceId.ItemsChanged] -= WebSocketKit_ItemsChanged;
            WebSocketKit.Instance.ServiceEvents[ServiceId.SendMoveBroadCast] -= WebSocketKit_SendMoveBroadCast;
            WebSocketKit.Instance.ServiceEvents[ServiceId.DoubleCubeRequest] -= WebSocketKit_DoubleCubeRequest;
            WebSocketKit.Instance.ServiceEvents[ServiceId.StartBankTime] -= WebSocketKit_StartBankTime;

            WebSocketKit.Instance.AckEvents[RequestId.GetCurrentMoveState] -= WebSocketKit_OnSyncGameStatus;
        }

        #region Overrides
        public virtual void StartGame(string playerId, string startingDice)
        {
            nextDice = startingDice;
            base.StartGame(GetPlayerFromId(playerId));
        }

        ~RemoteGame()
        {
            Unregister();
        }

        public virtual void ResumeMatch(string currentPlayerId, int turnIndex, string currentBoard, string nextTurnDice, bool isRequestingDoubleCube, bool isRolled)
        {
            nextDice = nextTurnDice;
            base.ResumeMatch(GetPlayerFromId(currentPlayerId), turnIndex, currentBoard, isRolled);

            if (isRequestingDoubleCube)
                CurrentTurnPlayer.RequestedDouble(GameBoard);

            if (isRolled)
                CurrentTurnPlayer.RollDice();
        }

        protected override GameStopEventArgs CreateStopGameEventArgs(IPlayer winner)
        {
            if (TourneyController.Instance.IsInTourney())
                return new TourneyGameStopEventArgs(IsCanceled, winner, m_stake.Kind, m_stake.CurrentBet, player1Stats, player2Stats, UserScores, OpponentScores);
            return new RemoteGameStopEventArgs(IsCanceled, winner, m_stake.Kind, m_stake.CurrentBet, m_stake.CurrentFee, Loyalty, player1Stats, player2Stats);
        }

        public override void Surrender()
        {
            RequestInGameController.Instance.SendStopGame(OpponentPlayer.playerId, Board.Serialize(GameBoard), OpponentPlayer.playerColor.ToString());
        }

        public override void Rematch(bool isRematch)
        {
            RequestInGameController.Instance.SendRematch(isRematch);
        }

        protected override void OnEndTurnEvent(IPlayer player)
        {
            if (player is IHumanPlayer && CurrentGameState == GameStatus.Started)
                RequestInGameController.Instance.SendMove((player as ILocalPlayer).movesLastTurn, (player as ILocalPlayer).eatenLastTurn, GameBoard);

            base.OnEndTurnEvent(player);
        }

        public override void InitiateDoubleRequest()
        {
            isDoubleCube = true;
            RequestInGameController.Instance.SendDoubleRequest(CurrentTurnPlayer.playerColor.ToString());
        }

        public void InitiateRollingRequest(Dictionary<string, string> pendingRollingItems)
        {
            RequestInGameController.Instance.SendRoll(pendingRollingItems);
        }

        protected override void StartNextTurn()
        {
            base.StartNextTurn();

            if (nextDice != null)
                CurrentTurnPlayer.SetDice(nextDice[0].ParseInt(), nextDice[1].ParseInt(), GameBoard);
            nextDice = null;

            StopSyncLoop();
            if (CurrentTurnPlayer is IRemotePlayer)
                StartSyncLoop();
        }

        protected override void EndGame(IPlayer player)
        {
            RequestInGameController.Instance.SendStopGame(player.playerId, player.playerColor.ToString(), Board.Serialize(GameBoard));
        }

        protected override void NextTurnReady()
        {
        }

        protected override void OnDoubleResponseEvent(DoubleResponse response, IPlayer player)
        {
            isDoubleCube = false;
            base.OnDoubleResponseEvent(response, player);

            if (player is IHumanPlayer)
                RequestInGameController.Instance.SendDoubleResponse(response, CurrentTurnPlayer.playerColor, GameBoard);
        }
        #endregion Overrides

        #region Network Event Functions

        private void WebSocketKit_OnSyncGameStatus(Ack ack)
        {
            SyncAck syncAck = ack as SyncAck;

            if (!syncAck.NeedToUpdate || CurrentTurnIndex == syncAck.MoveId)
                return;

            Debug.LogWarning("WebSocketKit_OnSyncGameStatus : need to update to moveID " + syncAck.MoveId);

            IPlayer LastPlayer = NextPlayer(GetPlayerFromId(syncAck.CurrentTurnPlayerID));

            //if (syncAck.IsDoubleCube)
            //{
            //    LastPlayer.RequestedDouble(GameBoard);
            //    return;
            //}

            bool showThrowing = nextDice == null;
            nextDice = syncAck.CurrentDice;
            if(showThrowing)
                LastPlayer.RollDice();

            if (LastPlayer is IRemotePlayer)
                (LastPlayer as IRemotePlayer).ReceivedMove(GameBoard, syncAck.Move);

            //if (syncAck.HaveBankTimeStarted)
            //    turnTimer.StartBankTime();

            StartNextTurn();
        }

        private void WebSocketKit_StoppedGame(Service service)
        {
            GameStoppedService gameStopped = service as GameStoppedService;
            if(gameStopped.MatchId != MatchId)
            {
                Debug.LogError("Stopped Game service targets a different match Id");
                return;
            }

            IsCanceled = gameStopped.IsCanceled;
            Loyalty = gameStopped.Loyalty;

            bool isFirst = gameStopped.FirstUserId == UserController.Instance.gtUser.Id;
            UserScores = isFirst ? gameStopped.FirstPoints : gameStopped.SecondPoints;
            OpponentScores = !isFirst ? gameStopped.FirstPoints : gameStopped.SecondPoints;

            player1Stats = new PlayerStats(gameStopped.Data, 0);
            player2Stats = new PlayerStats(gameStopped.Data, 1);
            Unregister();
            StopGame(GetPlayerFromId(gameStopped.WinnerId));
        }

        private void WebSocketKit_SendMoveBroadCast(Service service)
        {
            MoveReceivedService moveReceived = service as MoveReceivedService;
            if (moveReceived.TurnIndex < CurrentTurnIndex)
            {
                Debug.LogError("Invalid turn index.  received current: " + CurrentTurnIndex + " received: " + moveReceived.TurnIndex);
                return;
            }

            if (moveReceived.DoubleResponse)
                ReceivedDoubleResponse(moveReceived);
            else
                ReceivedTurn(moveReceived);
        }

        protected virtual void ReceivedTurn(MoveReceivedService service)
        {
            if (service.TurnIndex > CurrentTurnIndex && !(CurrentTurnPlayer is IHumanPlayer))
                (CurrentTurnPlayer as IRemotePlayer).ReceivedMove(GameBoard, service.SerializedMove);

            nextDice = service.NextDice;
            StartNextTurn();
        }

        protected virtual void ReceivedDoubleResponse(MoveReceivedService service)
        {
            StopTimer(CurrentTurnPlayer);
            UpdateStakeOnDoubleReceived(service);

            // set next dice
            CurrentTurnPlayer = GetPlayerFromId(service.NextTurnPlayer);
            nextDice = service.NextDice;
            StartNextTurn();
        }

        protected virtual void UpdateStakeOnDoubleReceived(MoveReceivedService service)
        {
            if (TourneyController.Instance.IsInTourney())
                m_stake.DoubleDone(GetPlayerFromId(service.CantDoubleId), 1 << service.CurrentDoubleAmount, 0); 
            else
                m_stake.DoubleDone(GetPlayerFromId(service.CantDoubleId), service.CurrentBet, service.CurrentFee);
        }

        public void WebSocketKit_PlayerRolled(Service service)
        {
            PlayerRolledService playerRolled = service as PlayerRolledService;
            CurrentTurnPlayer.playerData.SetRollingItems(playerRolled.RollingItemIds);
            CurrentTurnPlayer.RollDice();
        }

        public void WebSocketKit_ItemsChanged(Service service)
        {
            ItemsChangedService itemsChanged = service as ItemsChangedService;
            Dictionary<string, object> byUser = itemsChanged.GetChangesByUser();
            foreach (KeyValuePair<string, object> userData in byUser)
            {
                IPlayer player = (userData.Key == m_userPlayer.playerId ? m_userPlayer : m_opponentPlayer);
                Dictionary<string, object> itemsRawData = userData.Value as Dictionary<string, object>;
                foreach (KeyValuePair<string, object> storeData in itemsRawData)
                {
                    Enums.StoreType storeType;
                    if (Utils.TryParseEnum(storeData.Key, out storeType))
                    {
                        List<object> itemRawIds = storeData.Value as List<object>;
                        string[] itemIds = new string[itemRawIds.Count];
                        for (int i = 0; i < itemRawIds.Count; i++)
                            itemIds[i] = itemRawIds[i].ToString();
                        
                        player.UpdateSelectedItems(storeType, itemIds);
                    }
                }
            }
        }

        private void WebSocketKit_DoubleCubeRequest(Service service)
        {
            DoubleCubeRequestService result = service as DoubleCubeRequestService;
            UpdateStakeOnDoubleRequest(result);

            IPlayer next;
            if (result == null || string.IsNullOrEmpty(result.NextPlayerId))
                next = NextPlayer(CurrentTurnPlayer);
            else
                next = GetPlayerFromId(result.NextPlayerId);

            next.RequestedDouble(GameBoard);
        }

        protected virtual void UpdateStakeOnDoubleRequest(DoubleCubeRequestService doubleCubeRequest)
        {
            m_stake.DoubleDone(GetPlayerFromId(doubleCubeRequest.SenderId), doubleCubeRequest.CurrentBet, doubleCubeRequest.CurrentFee, true);
        }

        private void WebSocketKit_StartBankTime(Service service)
        {
            BankTimeService bankTimeService =  service as BankTimeService;
            CurrentTurnPlayer.playerData.CurrentBankTime = bankTimeService.bankTime;
            turnTimer.StartBankTime();
        }
        #endregion Network Event Functions

        private void StartSyncLoop()
        {
#if UNITY_WEBGL || UNITY_EDITOR
            SyncLoopCoRoutine = SyncLoop();
            RequestInGameController.Instance.StartCoroutine(SyncLoopCoRoutine);
#else
            SyncLoopThread = new Thread(SyncLoop);
            SyncLoopThread.Start();
#endif
        }

        private void StopSyncLoop()
        {
#if UNITY_WEBGL || UNITY_EDITOR
            if (SyncLoopCoRoutine != null)
                RequestInGameController.Instance.StopCoroutine(SyncLoopCoRoutine);
            SyncLoopCoRoutine = null;
#else
            if (SyncLoopThread != null)
                SyncLoopThread.Abort();
            SyncLoopThread = null;
#endif
        }

#if UNITY_WEBGL || UNITY_EDITOR
        System.Collections.IEnumerator SyncLoopCoRoutine;
        private System.Collections.IEnumerator SyncLoop()
        {
            while (SyncLoopCoRoutine != null)
            {
                yield return new WaitForSeconds(20);
                RequestInGameController.Instance.GetCurrentMoveState(CurrentTurnIndex, isDoubleCube);
            }
        }
#else
    Thread SyncLoopThread;
    private void SyncLoop()
    {
        while (SyncLoopThread != null)
        {
            Thread.Sleep(20000); 
            RequestInGameController.Instance.GetCurrentMoveState(CurrentTurnIndex, isDoubleCube);
        }
    }
#endif
    }
}
