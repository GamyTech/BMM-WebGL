using UnityEngine;
using System.Collections.Generic;
using GT.Store;
using GT.User;

namespace GT.Backgammon.Player
{
    public class PlayerFoundData
    {

        public string UserID { get; internal set; }
        public string UserName { get; internal set; }
        public SpriteData Avatar { get; internal set; }
        public Dictionary<Enums.StoreType, string[]> SelectedItems { get; internal set; }
        public bool IsBot { get; protected set; }

        public PlayerFoundData(Dictionary<string, object> playerDict)
        {
            object o;

            if (playerDict.TryGetValue("UserId", out o))
                UserID = o.ToString();

            if (playerDict.TryGetValue("UserName", out o))
                UserName = o.ToString();

            if (playerDict.TryGetValue("PictureUrl", out o))
                Avatar = new SpriteData(o.ToString());

            if (playerDict.TryGetValue("IsBot", out o))
                IsBot = o.ParseBool();

            SelectedItems = new Dictionary<Enums.StoreType, string[]>();
            if (playerDict.TryGetValue("Data", out o) && !string.IsNullOrEmpty(o.ToString()))
            {
                string data = o.ToString();
                Dictionary<string, object> dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(data);
                foreach (var item in dict)
                {
                    Enums.StoreType type = (Enums.StoreType)System.Enum.Parse(typeof(Enums.StoreType), item.Key);
                    List<object> list = (List<object>)item.Value;
                    List<string> str = list.ConvertAll(x => x.ToString());
                    SelectedItems.Add(type, str.ToArray());
                }
            }
        }

        public PlayerFoundData(string id, string userName, SpriteData profilPicture, bool isBot, Dictionary<Enums.StoreType, string[]> selectedItems)
        {
            UserID = id;
            UserName = userName;
            Avatar = profilPicture;
            IsBot = isBot;
            SelectedItems = selectedItems;
        }
    }
}
