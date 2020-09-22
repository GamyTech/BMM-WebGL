using System.Collections.Generic;
using GT.User;

namespace GT.Backgammon.Player
{
    public class PlayerData
    {
        public delegate void SelectedItemsUpdate(Dictionary<Enums.StoreType, string[]> SelectedItems);

        public event SelectedItemsUpdate OnSelectedItemsUpdate;

        public string UserName { get; internal set; }
        public PlayerColor Color { get; private set; }
        public float CurrentTurnTime { get; internal set; }
        public float TotalTurnTime { get; internal set; }
        public float CurrentBankTime { get; internal set; }
        public float TotalBankTime { get; internal set; }
        public SpriteData Avatar { get; internal set; }
        public Dictionary<Enums.StoreType, string[]> SelectedItems;
        public Dictionary<string, string> RollingItems { get; internal set; }
        public bool IsBot { get; protected set; }
        public float TotalTime { get; internal set; }
        public int Points { get; protected set; }

        public PlayerData(GameUser userData, float currentTurnTime, float totalTurnTime, float totalBankTime)
        {
            CurrentTurnTime = totalTurnTime - currentTurnTime;
            TotalTurnTime = totalTurnTime;
            TotalBankTime = totalBankTime;

            UserName = userData.UserName;
            Avatar = new SpriteData(userData.PictureUrl);
            Color = userData.Color;
            CurrentBankTime = userData.BankTime;
            TotalTime = userData.TotalTime;
            IsBot = userData.IsBot;
            Points = userData.Points;
            SelectedItems = userData.SelectedItems;
            RollingItems = userData.RollingItems;
        }

        public PlayerData(GTUser user, float currentTurnTime, float totalTurnTime, float totalBankTime, GameUser userData)
        {
            CurrentTurnTime = totalTurnTime - currentTurnTime;
            TotalTurnTime = totalTurnTime;
            TotalBankTime = totalBankTime;

            Color = userData.Color;
            Points = userData.Points;
            TotalTime = userData.TotalTime;

            UserName = user.UserName;
            Avatar = new SpriteData(user.Avatar);

            RollingItems = new Dictionary<string, string>();
            SelectedItems = new Dictionary<Enums.StoreType, string[]>();
            
            AddItemsFromStore(user.StoresData.storesDict, Enums.StoreType.LuckyItems);
            AddItemsFromStore(user.StoresData.storesDict, Enums.StoreType.Mascots);
        }

        private void AddItemsFromStore(Dictionary<Enums.StoreType, Store.Store> stores, Enums.StoreType type)
        {
            Store.Store store;
            if (stores.TryGetValue(type, out store) && store.selected.selectedIds != null && 
                store.selected.selectedIds.Length != 0)
            {
                SelectedItems.Add(store.storeType, store.selected.selectedIds);
            }
        }

        public PlayerData(PlayerFoundData foundplayer, float currentTurnTime, float totalTurnTime, float totalBankTime, GameUser userData)
        {
            CurrentTurnTime = totalTurnTime - currentTurnTime;
            TotalTurnTime = totalTurnTime;
            TotalBankTime = totalBankTime;

            Color = userData.Color;
            Points = userData.Points;
            TotalTime = userData.TotalTime;

            UserName = foundplayer.UserName;
            Avatar = foundplayer.Avatar;
            IsBot = foundplayer.IsBot;

            SelectedItems = foundplayer.SelectedItems;
            RollingItems = new Dictionary<string, string>();
        }

        public PlayerData(string name, SpriteData avatar, float currentTurnTime, float totalTurnTime, float currentBankTime, float totalBankTime, PlayerColor color)
        {
            UserName = name;
            Avatar = avatar;
            CurrentTurnTime = totalTurnTime - currentTurnTime;
            TotalTurnTime = totalTurnTime;
            CurrentBankTime = currentBankTime;
            TotalBankTime = totalBankTime;
            Color = color;
        }

        public void ResetCurrentTime()
        {
            CurrentTurnTime = TotalTurnTime;
            CurrentBankTime = TotalBankTime;
        }

        public void UpdateUsedItems(Enums.StoreType type, string[] itemsId)
        {
            SelectedItems.AddOrOverrideValue(type, itemsId);
            
            if (OnSelectedItemsUpdate != null)
                OnSelectedItemsUpdate(SelectedItems);
        }

        public void SetRollingItems(Dictionary<string, string> rollingItems)
        {
            if (rollingItems != null)
                RollingItems = rollingItems;
            else
                RollingItems.Clear();
        }

        public override string ToString()
        {
            return "m_userName " + UserName + " m_currentTurnTime " +
                CurrentTurnTime + " m_totalTurnTime " + TotalTurnTime + " m_currentBankTime " +
                CurrentBankTime + " m_totalBankTime " + TotalBankTime + 
                " m_picture " + Avatar + " m_selectedItems " + printSelected() + " m_isBot " + IsBot;
        }

        private string printSelected()
        {
            string s = "";
            foreach (var item in SelectedItems)
            {
                s += item.Key.ToString() + " " + item.Value.Display() + "\n";
            }
            return s;
        }
    }

    public class GameUser
    {
        public string UserId { get; protected set; }
        public string UserName { get; protected set; }
        public string PictureUrl { get; protected set; }
        public PlayerColor Color { get; protected set; }
        public int BankTime { get; protected set; }
        public int TotalTime { get; protected set; }
        public int Points { get; protected set; }
        public bool IsBot { get; protected set; }
        public Dictionary<string, object> RawData;
        public Dictionary<Enums.StoreType, string[]> SelectedItems;
        public Dictionary<string, string> RollingItems { get; internal set; }

        public GameUser(Dictionary<string, object> dict)
        {
            RawData = dict;
            object o;
            if (dict.TryGetValue("UserId", out o))
                UserId = o.ToString();
            if (dict.TryGetValue("UserName", out o))
                UserName = o.ToString();
            if (dict.TryGetValue("PictureUrl", out o))
                PictureUrl = o.ToString();
            PlayerColor color;
            if (dict.TryGetValue("Color", out o) && Utils.TryParseEnum(o.ToString(), out color))
                Color = color;
            if (dict.TryGetValue("BankTime", out o))
                BankTime = o.ParseInt();
            if (dict.TryGetValue("TotalTime", out o))
                TotalTime = o.ParseInt();
            if (dict.TryGetValue("Points", out o))
                Points = o.ParseInt();
            if (dict.TryGetValue("IsBot", out o))
                IsBot = o.ParseBool();
            RollingItems = new Dictionary<string, string>();
            SelectedItems = new Dictionary<Enums.StoreType, string[]>();
            if (dict.TryGetValue("Data", out o) && !string.IsNullOrEmpty(o.ToString()))
            {
                string data = o.ToString();
                Dictionary<string, object> dataDict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(data);
                foreach (var item in dataDict)
                {
                    Enums.StoreType type = (Enums.StoreType)System.Enum.Parse(typeof(Enums.StoreType), item.Key);
                    List<object> list = (List<object>)item.Value;
                    List<string> str = list.ConvertAll(x => x.ToString());
                    SelectedItems.Add(type, str.ToArray());
                }
            }
        }
    }
}
