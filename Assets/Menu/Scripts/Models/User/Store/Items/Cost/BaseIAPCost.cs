using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace GT.Store
{
    public abstract class BaseIAPCost
    {
        public abstract float Cost { get; }
        public abstract string ItemId { get; }
        public abstract string PurchaseMethod { get; }
        public bool Consumable { get; private set; }

        public BaseIAPCost(bool consumable)
        {
            Consumable = consumable;
        }

        public string GetIAPCostString()
        {
            return Wallet.CashPostfix + Wallet.AmountToString(Cost, 2);
        }

        public static BaseIAPCost CreateIAPCost(bool consumable, Dictionary<string, object> dict)
        {
            BaseIAPCost IAPCost = null;
            object o;
            float cost = 0;
            if (dict.TryGetValue("IAPCost", out o))
                cost = o.ParseFloat();
            else
                return null;

#if UNITY_IOS && !UNITY_EDITOR
            string id = null;
            if (dict.TryGetValue("ItunesItemId", out o))
                id = o.ToString();

            if (id == null || id.Equals("0"))
                return null;

            IAPCost = new ItunesIAPCost(consumable, id, cost);
#elif UNITY_WEBGL
            string id = null;
            if (dict.TryGetValue("FacebookItemId", out o))
                id = o.ToString();

            if (id == null || id.Equals("0"))
                return null;

            IAPCost = new FacebookIAPCost(consumable, id, cost);
#elif UNITY_ANDROID && !UNITY_EDITOR
            string id = null;
            if (dict.TryGetValue("GogglePlayItemId", out o))
                id = o.ToString();

            if (id == null || id.Equals("0"))
                return null;

            IAPCost = new GooglePlayIAPCost(consumable, id, cost);
#else
            string id = null;
            if (dict.TryGetValue("ItunesItemId", out o))
                id = o.ToString();

            if (id == null || id.Equals("0"))
                return null;

            IAPCost = new ItunesIAPCost(consumable, id, cost);
#endif
            return IAPCost;
        }
    }
}
