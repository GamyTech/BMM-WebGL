using BestHTTP.WebSocket;
using System;
using System.Collections.Generic;
using GT.Websocket;

namespace GT.Websocket
{
    public abstract class WebSocketMessage
    {
        public WSResponseCode Code { get; private set; }

        public WebSocket WebSocket { get; private set; }

        public Dictionary<string, object> Data { get; private set; }

        public string RawData { get; private set; }

        public WebSocketMessage(WebSocket webSocket, Dictionary<string, object> data, string rawData)
        {
            WebSocket = webSocket;
            Data = data;
            RawData = rawData;
            InitCode(data);
            HandleAPIChangedEvents(data);
        }

        public abstract void TriggerEvent();

        public virtual string ToString(bool dict)
        {
            return dict ? Data.Display<string, object>() : ToString();
        }

        public override string ToString()
        {
            return RawData;
        }

        private void InitCode(Dictionary<string, object> data)
        {
            object o;
            if (data.TryGetValue("ErrorCode", out o))
            {
                WSResponseCode errorCode;
                Code = Utils.TryParseEnum(o, out errorCode) ? errorCode : WSResponseCode.UnrecognizedError;
            }
            else
                Code = WSResponseCode.OK;
        }

        private void HandleAPIChangedEvents(Dictionary<string, object> data)
        {
            Dictionary<APIResponseVariable, object> eventsDict = new Dictionary<APIResponseVariable, object>();
            foreach (KeyValuePair<string, object> responseItem in data)
            {
                APIResponseVariable apiResponse;
                if (Utils.TryParseEnum(responseItem.Key, out apiResponse))
                    eventsDict.Add(apiResponse, responseItem.Value);
            }
            WebSocketKit.Instance.SendVariablesChangedEvents(eventsDict);
        }
    }
}
