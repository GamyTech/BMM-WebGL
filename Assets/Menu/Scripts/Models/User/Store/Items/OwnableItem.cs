using System.Collections.Generic;
using UnityEngine;

namespace GT.Store
{
    public class OwnableItem : StoreItem, IOwnable
    {
        private bool m_owned;
        public bool Owned
        {
            get
            {
                return m_owned;
            }
        }

        public OwnableItem(Dictionary<string, object> dict, GameObject prefab, string BuyString, bool CanBeGiven, AudioClip boughtSound) : base(dict, prefab, BuyString, CanBeGiven, boughtSound) { }

        protected override void Init(Dictionary<string, object> dict)
        {
            base.Init(dict);

            if (Cost.IsFree())
            {
                m_owned = true;
            }
        }

        protected override void PurchaseSuccessfull()
        {
            Select();

            m_owned = true;
            isSelectable = true;
            base.PurchaseSuccessfull();
        }
        protected override void AfterPurchaseAction() {}

        internal override void UpdatePurchasedItem(PurchasedItem item)
        {
            m_owned = true;
            isSelectable = true;
            base.UpdatePurchasedItem(item);
        }

        internal override bool IsSelectable(int userRank)
        {
            return base.IsSelectable(userRank) && m_owned;
        }
    }
}
