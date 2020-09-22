using UnityEngine;
using System.Collections.Generic;

namespace GT.Store
{
    public class ConsumableItem : StoreItem, IConsumable
    {
        private string m_purchaseId;
        public bool ConsumeOnDeselect;
        public int OwnedCount { get; private set; }
        public int ConsumableAmount { get; private set; }

        public ConsumableItem(Dictionary<string, object> dict, GameObject prefab, string BuyString, bool CanBeGiven, bool consumeOnDeselect, AudioClip boughtSound): base(dict, prefab, BuyString, CanBeGiven, boughtSound)
        {
            ConsumableAmount = 1;
            ConsumeOnDeselect = consumeOnDeselect;
        }

        protected override void Init(Dictionary<string, object> dict)
        {
            base.Init(dict);

            object o;
            if (dict.TryGetValue("ConsumableAmount", out o))
                ConsumableAmount = o.ParseInt();
        }

        public void ConsumeItem()
        {
            if (string.IsNullOrEmpty(m_purchaseId)) Debug.LogWarning("ConsumeItem " + Id + " :: Invalid purchaseId");

            UserController.Instance.SendConsumeItem(m_purchaseId);
            isSelectable = OwnedCount > 1;
            TriggerChangedEvent();
        }

        internal override void UpdatePurchasedItem(PurchasedItem item)
        {
            m_purchaseId = item.purchaseId;

            OwnedCount = item.chargesAmount;
            isSelectable = OwnedCount > 0;
            if(isSelectable)
                Select();

            base.UpdatePurchasedItem(item);
        }

        protected override void OnUnselect()
        {
            if (ConsumeOnDeselect && OwnedCount > 0)
            {
                ConsumeItem();
                Selected = false;
            }
            base.OnUnselect();
        }

        internal override bool IsSelectable(int userRank)
        {
            return base.IsSelectable(userRank) && OwnedCount > 0;
        }
    }
}
