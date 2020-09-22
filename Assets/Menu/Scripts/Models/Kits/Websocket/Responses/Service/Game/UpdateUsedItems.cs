using BestHTTP.WebSocket;
using GT.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace GT.Websocket
{
    public class UpdateUsedItems : Service
    {

        public string Sender { get; private set; }
        public List<UpdatedUsedItem> ItemList { get; private set; }


        public UpdateUsedItems(ServiceId serviceId, WebSocket webSocket, ServiceHandler eventHandler, Dictionary<string, object> data, string rawData) :
            base(serviceId, webSocket, eventHandler, data, rawData)
        {
            Debug.Log("OpponentChangeItemsSent : " + rawData);
            Init(data);
        }

        private void Init(Dictionary<string, object> data)
        {
            object o;

            if (data.TryGetValue("Sender", out o))
                Sender = o.ToString();
            else
                Debug.LogError("Sender is missing from Dictionnary");

            if (data.TryGetValue("Items", out o))
            {
                ItemList = new List<UpdatedUsedItem>();
                List<object> ItemsList = o as List<object>;
                for (int x = 0; x < ItemsList.Count; ++x)
                    ItemList.Add(new UpdatedUsedItem(ItemsList[x] as Dictionary<string, object>));
            }
            else
                Debug.LogError("Items is missing from Dictionnary");
        }
    }
}

public class UpdatedUsedItem
{
    public Enums.StoreType Type { get; private set; }
    public string Id { get; private set; }
    public bool Gifted { get; private set; }
    public Dictionary<string, object> Dict;

    public UpdatedUsedItem(Dictionary<string, object> dict)
    {
        Dict = new Dictionary<string, object>(dict);
        object o;
        Enums.StoreType type;
        if (Dict.TryGetValue("StoreType", out o) && Utils.TryParseEnum(o.ToString(), out type))
            Type = type;
        else
            Debug.LogError("StoreType is missing from Dictionnary");

        if (dict.TryGetValue("Id", out o))
            Id = o.ToString();
        else
            Debug.LogError("StoreType is missing from Dictionnary");

        if (dict.TryGetValue("Gifted", out o))
            Gifted = o.ParseBool();
        else
            Debug.LogError("StoreType is missing from Dictionnary");
    }

    public UpdatedUsedItem(Enums.StoreType type, string id, bool gifted)
    {
        Type = type;
        Id = id;
        Gifted = gifted;

        Dict = new Dictionary<string, object>();
        Dict.Add("StoreType", Type);
        Dict.Add("Id", Id);
        Dict.Add("Gifted", Gifted);
    }
}
