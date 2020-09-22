using System.Collections.Generic;
using System;
using GT.Assets;
using UnityEngine;

namespace GT.Store
{
    public class Store
    {
        public Enums.StoreType storeType;
        public string name;
        public char icon;
        public bool inGameStore;
        public bool isStandalone;
        public Enums.PageId pageId = Enums.PageId.LoyaltyStore;

        public List<StoreItem> items = new List<StoreItem>();
        public Selected selected;

        internal Store(Dictionary<string, object> dict)
        {
            object o;
            if (!dict.TryGetValue("StoreSubType", out o) || !Utils.TryParseEnum(o.ToString(), out storeType))
                Debug.LogError("Unrecognised store sub type " + o);

            if (dict.TryGetValue("StoreName", out o))
                name = o.ToString();

            if (dict.TryGetValue("Icon", out o))
                icon = o.ToString()[0];

            if (dict.TryGetValue("InGameStore", out o))
                inGameStore = o.ParseBool();

            if (dict.TryGetValue("IsStandalone", out o))
                isStandalone = o.ParseBool();

            StoreData data;
            if (!Stores.StoresDataDict.TryGetValue(storeType, out data))
            {
                Debug.LogError("Missing store data for type " + storeType);
                return;
            }

            if (dict.TryGetValue("StoreItems", out o))
            {
                List<object> list = o as List<object>;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Dictionary<string, object> d = list[i] as Dictionary<string, object>;
                        StoreItem item = StoreItem.CreateStoreItem(d, data.itemPrefab, data.BuyString, data.canBeGiven, data.consumeOnDeselect, data.BoughtSound);
                        items.Add(item);
                    }
                }
            }

            pageId = data.pageId;

            selected = new Selected(storeType, data.selectedCount, data.canBeUnselected);
            if (selected != null)
                selected.InitItems(items);
        }

        public StoreItem GetItemById(string Id)
        {
            return items.Find(i => i.Id == Id);
        }

        public override string ToString()
        {
            return "Type " + storeType + " Name: " + name + " Icon: " + icon + " Items:" + Environment.NewLine + items.Display("," + Environment.NewLine);
        }
    }
}
