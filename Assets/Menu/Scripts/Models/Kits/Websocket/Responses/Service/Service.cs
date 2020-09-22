using System.Collections.Generic;
using BestHTTP.WebSocket;

namespace GT.Websocket
{
    public enum ServiceId
    {
        IsAlive,

        WasRemovedFromMatchSearch,

        IsReady,
        GameStarted,
        StoppedGame,

        DoubleCubeRequest,
        AcceptDouble,
        RefuseDouble,

        PlayerRolled,

        ItemsChanged,

        SendMoveBroadCast,
        StartBankTime,
        MatchTimeOut,
        MatchIsCanceled,
        SendChat,
        UpdateUsedItems,
        NotifyCashInSuccess,
        NotifyCashInFailed,

        FriendInviteToPlay,
        FriendInviteToPlayAnswer,
        MessageEvents,

        UpdateTourneyDetails,
        StartPreTourney,
        StartTourney,
        CloseTourney,
        IsTourneyReady,
        MatchStarted,
        SendTourneyChat,
        CancelTourneySearch,
        TourneysStatus
    }

    public class Service : WebSocketMessage
    {
        private ServiceHandler serviceEventHandler;
        public ServiceId ServiceId { get; private set; }

        public Service(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(webSocket, data, rawData)
        {
            ServiceId = serviceId;
            serviceEventHandler = eventHandler;
        }

        public override void TriggerEvent()
        {
            if (serviceEventHandler != null)
                serviceEventHandler(this);
        }
    }
}
