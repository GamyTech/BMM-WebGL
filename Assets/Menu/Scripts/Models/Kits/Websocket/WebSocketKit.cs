using UnityEngine;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using BestHTTP;
using System;
using MiniJSON;

namespace GT.Websocket
{

    internal enum ServerRequestKey
    {
        VariablesToPass,
        RequestedParamaters,
        ParamatersToUpdate,
        UserName,
        UserId,
        Email,
        Password,
        Phone,
        DeviceId,
        LastMatchId,
        TourneyId,
    }

    public enum RequestId
    {
        IsAlive,

        PlayFriend,
        PlayFriendAnswer,

        SearchMatch,
        CancelSearch,
        ReadyToPlay,
        Rolled,
        SendMove,
        GetCurrentMoveState,
        StopGame,
        DoubleCube,
        DoubleCubeAnswer,
        Rematch,
        SendChat,
        ChangeRoomItem,

        GetTourneysInfo,
        RegisterTourney,
        UnRegisterTourney,
        SearchTourneyhMatch,
        CancelTourneySearch,
        ReadyToPlayTourneyMatch,
        StopTourneyMatch,
        GetSpecificTourneyDetails,
        ChangeTourneyRoomItem,
        TourneyRolled,
        GetTourneyCurrentMoveState,
        SendTourneyMove,
        TourneyDoubleCube,
        TourneyDoubleCubeAnswer,
        SendTourneyChat,
        ListenToTourneysEvents,
        StopListenToTourneysEvents,

        Login,
        Logout,
        RegisterUser,
        CashOutRequest,
        CashIn,
        CashInEasyTransact,
        CashInPaySafe,
        CashInSkrill,
        ContactGamytech,
        ForgotPassword,
        CollectTimelyBonus,
        GetMatchesHistory,
        PurchaseItem,
        UpdateMessageEvent,
        SendCashinInstructions,
        AddPendingVerification,
        GetMatchHistoryMoves,
        GetWalletChangeLogs,
        GetSmsValidation,
        ValidateSmsByUser,
        DeleteCard,
        UpdateVerificationDetails,

        ApiRequest,
        ApiUpdate,
    }

    public delegate void ServiceHandler(Service service);
    public delegate void AckHandler(Ack ack);
    public delegate void VoidHandler();
    public delegate void CloseHandler(ushort code, string msg);
    public delegate void ErrorHandler(string error);

    public class WebSocketKit 
    {
        #region Singleton
        private static WebSocketKit m_instance;
        public static WebSocketKit Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new WebSocketKit();
                return m_instance;
            }
        }
        #endregion Singleton

        public event VoidHandler OnOpenEvent;
        public event CloseHandler OnCloseEvent;
        public event ErrorHandler OnErrorEvent;

        public Dictionary<ServiceId, ServiceHandler> ServiceEvents = new Dictionary<ServiceId, ServiceHandler>();
        public Dictionary<RequestId, AckHandler> AckEvents = new Dictionary<RequestId, AckHandler>();

        #region Changed Variable - Delegates And Events
        public delegate void APIVariableChanged(object newValue);

        private Dictionary<APIResponseVariable, APIVariableChanged> ChangeEventsDict = new Dictionary<APIResponseVariable, APIVariableChanged>();
        #endregion Changed Variable - Delegates And Events

        public WebSocket webSocket;


        /// <summary>
        /// Default Constructor
        /// </summary>
        private WebSocketKit()
        {
            foreach (APIResponseVariable var in Enum.GetValues(typeof(APIResponseVariable)))
                ChangeEventsDict.Add(var, delegate { });

            foreach (var item in Enum.GetValues(typeof(ServiceId)))
                ServiceEvents.Add((ServiceId)item, delegate { });

            foreach (var item in Enum.GetValues(typeof(RequestId)))
                AckEvents.Add((RequestId)item, delegate { });
        }

        #region Public Methods
        public bool IsOpen()
        {
            return webSocket != null && webSocket.IsOpen;
        }

        /// <summary>
        /// Connect to server ip
        /// </summary>
        /// <param name="type"></param>
        public void OpenConnection(string ip, string hostIp)
        {
            if (webSocket != null && webSocket.IsOpen)
            {
                Debug.Log("Closing a WebSocket connection to create a new one");
                webSocket.Close();
            }

            Dictionary<string, object> identifier = new Dictionary<string, object>() {
                { "DeviceId", NetworkController.DeviceId },
                { "GameId", AppInformation.GAME_ID },
                { "Ip", hostIp },
                { "OS", Application.platform },
                { "ClientId", NetworkController.Instance.ClientId }
            };

            if (URLParameters.GetSearchParameters().ContainsKey("token"))
            {
                string token = URLParameters.GetSearchParameters()["token"];
                identifier["Token"] = token;
            }

            string uri = ip + Json.Serialize(identifier);
            Debug.Log("URI " + uri);
            webSocket = new WebSocket(new Uri(uri), ip, "");

#if !BESTHTTP_DISABLE_PROXY && !UNITY_WEBGL
            if (HTTPManager.Proxy != null)
                webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif

            webSocket.OnOpen += OnOpen;
            webSocket.OnClosed += OnClosed;
            webSocket.OnError += OnError;
            webSocket.OnMessage += OnMessage;

            webSocket.Open();
        }

        private void Send(string message)
        {
            if (webSocket != null && webSocket.IsOpen)
            {
                if(!message.Contains(RequestId.IsAlive.ToString()))
                    Debug.Log("<color=white>Sending: " + message + "</color>");
                webSocket.Send(message);
            }
            else
                Debug.Log("Cant Send. Connection Closed");
        }

        public void SendRequest(RequestId request, Dictionary<string,object> paramsDict)
        {
            Dictionary<string, object> dict = BuildMessage(request);
            dict.Merge(paramsDict);
            Send(Json.Serialize(dict));
        }

        public void SendRequest(RequestId resquest)
        {
            Send(Json.Serialize(BuildMessage(resquest)));
        }

        public void Login(string email, string pass, string lastMatchId = null)
        {
            Dictionary<string, object> dict = BuildMessage(RequestId.Login);
            dict.Add(ServerRequestKey.Email.ToString(), email.ToLower());
            dict.Add(ServerRequestKey.Password.ToString(), pass);
            dict.Add(ServerRequestKey.DeviceId.ToString(), NetworkController.DeviceId);

            string oldTourneyId = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.TourneyId);
            if (!string.IsNullOrEmpty(oldTourneyId))
                dict.Add(ServerRequestKey.TourneyId.ToString(), oldTourneyId);

            if(!string.IsNullOrEmpty(lastMatchId))
                dict.Add(ServerRequestKey.LastMatchId.ToString(), lastMatchId);

            Send(Json.Serialize(dict));
        }

        public void Register(string UserName, string email, string pass)
        {
            Dictionary<string, object> dict = BuildMessage(RequestId.RegisterUser);
            dict.Add(ServerRequestKey.UserName.ToString(), UserName.ToLower());
            dict.Add(ServerRequestKey.Email.ToString(), email.ToLower());
            dict.Add(ServerRequestKey.Password.ToString(), pass);
            dict.Add(ServerRequestKey.DeviceId.ToString(), NetworkController.DeviceId);
            //dict.Add(ServerRequestKey.Phone.ToString(), phone);
            Send(Json.Serialize(dict));
        }

        public void SendAPIRequest(APIGetVariable vars, Dictionary<PassableVariable, object> variablesToPass = null)
        {
            Dictionary<string, object> dict = BuildMessage(RequestId.ApiRequest);
            if (variablesToPass != null)
                dict.Add(ServerRequestKey.VariablesToPass.ToString(), variablesToPass);
            dict.Add(ServerRequestKey.RequestedParamaters.ToString(), APIGetVariableToList(vars));
            Send(Json.Serialize(dict));
        }

        public void SendAPIUpdate(APIUpdateVariable vars, Dictionary<PassableVariable, object> variablesToPass = null)
        {
            Dictionary<string, object> dict = BuildMessage(RequestId.ApiUpdate);
            if (variablesToPass != null)
                dict.Add(ServerRequestKey.VariablesToPass.ToString(), variablesToPass);
            dict.Add(ServerRequestKey.ParamatersToUpdate.ToString(), Utils.EnumToList(vars));
            Send(Json.Serialize(dict));
        }

        #region Register/Unregister for API Variable Changed Events
        public void RegisterForAPIVariableEvent(APIResponseVariable var, APIVariableChanged e)
        {
            ChangeEventsDict[var] += e;
        }

        public void UnregisterForAPIVariableEvent(APIResponseVariable var, APIVariableChanged e)
        {
            ChangeEventsDict[var] -= e;
        }
        #endregion Register/Unregister for API Variable Changed Events
        #endregion Public Methods

        #region WebSocket Events
        private void OnOpen(WebSocket webSocket)
        {
            Debug.Log("<color=white>WebSocket OnOpen isOpen: </color>" + webSocket.IsOpen);
            if (OnOpenEvent != null)
                OnOpenEvent();
        }
        private void OnClosed(WebSocket webSocket, ushort code, string message)
        {
            Debug.Log("WebSocket OnClosed code: " + code + ", message: " + message + ", isOpen: " + webSocket.IsOpen);
            if (OnCloseEvent != null)
                OnCloseEvent(code, message);
        }
        private void OnError(WebSocket webSocket, Exception ex)
        {
            if (ex != null)
            {
                Debug.Log("WebSocket OnError: " + ex.Message);
                Debug.Log("WebSocket OnError StackTrace: " + ex.StackTrace);
                Debug.Log("WebSocket OnError Source: " + ex.Source);
                Debug.Log("WebSocket OnError Data: " + ex.Data);
            }
            this.webSocket.Close();
            if (OnErrorEvent != null && ex != null)
                OnErrorEvent(ex.Message);
        }
        private void OnMessage(WebSocket webSocket, string messageString)
        {
            string decryptedString = tryDecrypt(messageString);
            if(!decryptedString.Contains("IsAlive"))
                Debug.Log("<color=white>OnRawMessage: " + decryptedString + "</color>");

            Dictionary<string, object> messageDict = Json.Deserialize(decryptedString) as Dictionary<string, object>;
            if (messageDict == null)
            {
                Debug.LogError("Failed to parse message " + decryptedString);
                return;
            }

            WebSocketMessage message = null;
            object o;

            if (messageDict.TryGetValue("Service", out o))
                message = Factory.GetService(webSocket, o.ToString(), ServiceEvents, messageDict, decryptedString);
            else if (messageDict.TryGetValue("Response", out o))
                message = Factory.GetAck(webSocket, o.ToString(), AckEvents, messageDict, decryptedString);
            else
            {
                Debug.LogError("Unrecognised message " + decryptedString);
                return;
            }

            message.TriggerEvent();
        }
        #endregion WebSocket Events

        #region Private Methods
        private Dictionary<string, object> BuildMessage(RequestId request)
        {
            return new Dictionary<string, object>() { { "Service", request.ToString() } };
        }

        private List<string> APIGetVariableToList(APIGetVariable vars)
        {
            return new List<string>(vars.ToString().Replace(" ", string.Empty).Split(','));
        }

        /// <summary>
        /// Try decrypting text.
        /// If seccessfull will return decrypted string.
        /// If failed will return original string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string tryDecrypt(string text)
        {
            try
            {
                return Encryption.AesBase64Wrapper.Decrypt(text);
            }
            catch
            {
                return text;
            }
        }

        /// <summary>
        /// Send variables changed events.
        /// </summary>
        /// <param name="varsDict"></param>
        public void SendVariablesChangedEvents(Dictionary<APIResponseVariable, object> varsDict)
        {
            if (varsDict == null)
                return;

            foreach (KeyValuePair<APIResponseVariable, object> item in varsDict)
                if (ChangeEventsDict[item.Key] != null)
                    ChangeEventsDict[item.Key](item.Value);
        }
        #endregion Private Methods
    }
}
