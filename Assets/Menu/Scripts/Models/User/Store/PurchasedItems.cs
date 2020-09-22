using UnityEngine;
using System.Collections.Generic;

namespace GT.Store
{
    public class PurchasedItems
    {
        internal List<PurchasedItem> items = new List<PurchasedItem>(); 

        internal PurchasedItems(List<object> list)
        {
            Init(list);
        }

        private void Init(List<object> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                PurchasedItem item = new PurchasedItem((Dictionary<string, object>)list[i]);
                items.Add(item);
            }
        }
    }

    internal class PurchasedItem
    {
        internal Enums.StoreType storeType;
        internal string itemId;
        internal int chargesAmount;
        internal string purchaseId;

        internal PurchasedItem(Dictionary<string,object> dict)
        {
            object o;
            if (!dict.TryGetValue("StoreSubType", out o)|| !Utils.TryParseEnum(o.ToString(), out storeType))
            {
                Debug.LogError("PurchasedItem :: Unrecognized Store Type " + dict.Display());
                return;
            }

            if (dict.TryGetValue("ItemId", out o))
                itemId = o.ToString();

            if (dict.TryGetValue("PurchaseId", out o))
                purchaseId = o.ToString();

            if (dict.TryGetValue("ChargesAmount", out o))
                chargesAmount = o.ParseInt();
        }
    }
}
