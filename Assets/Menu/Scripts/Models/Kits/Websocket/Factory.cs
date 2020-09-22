using BestHTTP.WebSocket;
using System.Collections.Generic;

namespace GT.Websocket
{
    public static class Factory
    {
        public static Ack GetAck(WebSocket websocket, string requestIdString, Dictionary<RequestId, AckHandler> events, Dictionary<string, object> data, string rawData)
        {
            Ack ack = null;
            RequestId requestId;
            AckHandler ackEventHandler = null;
            string request = requestIdString.ToString().Replace("Ack", "");
            if (Utils.TryParseEnum(request, out requestId))
                ackEventHandler = events[requestId];
            
            switch (requestId)
            {
                case RequestId.Login:
                    ack = new LoginAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.RegisterUser:
                    ack = new RegisterAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.CashInEasyTransact:
                case RequestId.CashIn:
                case RequestId.CashInPaySafe:
                case RequestId.CashInSkrill:
                    ack = new CashInAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.AddPendingVerification:
                    ack = new AddPendingVerificationAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.GetCurrentMoveState:
                    ack = new SyncAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.GetMatchHistoryMoves:
                    ack = new GetMatchHistoryMovesAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.GetWalletChangeLogs:
                    ack = new GetWalletChangeLogsAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.ValidateSmsByUser:
                    ack = new ValidateSmsByUserAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.GetTourneysInfo:
                    ack = new GetTourneysInfoAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.GetSpecificTourneyDetails:
                    ack = new GetSpecificTourneyDetailsAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                case RequestId.RegisterTourney:
                case RequestId.UnRegisterTourney:
                    ack = new RegisterTourneyAck(requestId, websocket, ackEventHandler, data, rawData);
                    break;
                default:
                    ack = new Ack(requestId, websocket, ackEventHandler, data, rawData);
                    break;
            }
            return ack;
        }

        public static Service GetService(WebSocket websocket, string serviceIdString, Dictionary<ServiceId, ServiceHandler> events, Dictionary<string, object> data, string rawData)
        {
            Service service = null;
            ServiceId serviceId;
            ServiceHandler serviceEventHandler = null;
            if (Utils.TryParseEnum(serviceIdString, out serviceId))
            {
                serviceEventHandler = events[serviceId];
            }
            switch (serviceId)
            {
                case ServiceId.ItemsChanged:
                    service = new ItemsChangedService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.SendChat:
                case ServiceId.SendTourneyChat:
                    service = new ChatReceivedService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.UpdateUsedItems:
                    service = new UpdateUsedItems(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.IsTourneyReady:
                case ServiceId.IsReady:
                    service = new OpponentFoundService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.MatchStarted:
                case ServiceId.GameStarted:
                    service = new GameStartedService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.StoppedGame:
                    service = new GameStoppedService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.DoubleCubeRequest:
                    service = new DoubleCubeRequestService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.AcceptDouble:
                    service = new RematchService(serviceId, websocket, serviceEventHandler, data, rawData, true);
                    break;
                case ServiceId.RefuseDouble:
                    service = new RematchService(serviceId, websocket, serviceEventHandler, data, rawData, false);
                    break;
                case ServiceId.PlayerRolled:
                    service = new PlayerRolledService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.SendMoveBroadCast:
                    service = new MoveReceivedService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.StartBankTime:
                    service = new BankTimeService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.MatchTimeOut:
                    service = new GameStoppedService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.FriendInviteToPlay:
                    service = new FriendInviteService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.NotifyCashInSuccess:
                    service = new NotifyCashInSuccessService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.NotifyCashInFailed:
                    service = new NotifyCashInFailedService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.MessageEvents:
                    service = new ServerPopupService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.UpdateTourneyDetails:
                    service = new UpdateTourneyDetailsService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.StartPreTourney:
                    service = new StartPreTourneyService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.CloseTourney:
                    service = new CloseTourneyService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.StartTourney:
                    service = new StartTourneyService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                case ServiceId.TourneysStatus:
                    service = new TourneysStatusService(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
                default:
                    service = new Service(serviceId, websocket, serviceEventHandler, data, rawData);
                    break;
            }
            return service;
        }
    }
}
