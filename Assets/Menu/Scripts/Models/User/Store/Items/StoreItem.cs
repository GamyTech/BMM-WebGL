using System.Collections.Generic;
using UnityEngine;
using GT.InAppPurchase;
using UnityEngine.Events;
using GT.Websocket;
using GT.User;
using System;

namespace GT.Store
{
    public class StoreItem : DynamicElement, ICustomPrefab, ILockable
    {
        public delegate void ChangedHandler(StoreItem newItem);
        public event ChangedHandler OnItemChanged;

        internal bool isSelectable;

        public string PurchaseString;

        public virtual string SelectedString { get { return m_selected ? "Using" : "Use"; } }
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string LoyaltyPoints { get; private set; }
        public SpriteData LocalSpriteData { get; private set; }
        public CostData Cost { get; private set; }
        public int UnlockRank { get; private set; }
        public UnityAction SelectAction;
        public GameObject Prefab { get; private set; }
        public bool CanBeGivenToOpponent { get; private set; }
        public AudioClip BoughtSound { get; private set; }

        private bool m_selected;
        public bool Selected
        {
            get { return m_selected; }
            internal set
            {
                if (value == m_selected)
                    return;

                m_selected = value;
                if (m_selected == false)
                    OnUnselect();

                TriggerChangedEvent();
            }
        }

        internal StoreItem(Dictionary<string, object> dict, GameObject prefab, string BuyString, bool CanBeGiven, AudioClip boughtSound)
        {
            BoughtSound = boughtSound;
            Prefab = prefab;
            CanBeGivenToOpponent = CanBeGiven;
            PurchaseString = BuyString;
            Init(dict);
            TriggerChangedEvent();
        }

        protected virtual void Init(Dictionary<string, object> dict)
        {
            object o;
            if (dict.TryGetValue("ItemId", out o))
                Id = o.ToString();

            if (Id == "7" || Id == "8" || Id == "9" || Id == "11" || Id == "13" || Id == "20" || Id == "24")
                BoughtSound = AssetController.Instance.BonusCashBoughtSound;

            if (dict.TryGetValue("Name", out o))
                Name = o.ToString();

            if (dict.TryGetValue("Description", out o))
                Description = o.ToString();

            // !!!!!!!!!!!!!!----------- for backgammon Royale Board ---------------- !!!!!!
            if (AppInformation.GAME_ID == Enums.GameID.BackgammonRoyale && Id == "52")
            {
                Id = "-1";
                LocalSpriteData = new SpriteData("https://gamytechstoragever2.blob.core.windows.net/boards/main_i6_loyalty_store_dices_boards_v7.png", true);
            }
            else if (dict.TryGetValue("ImageUrl", out o))
                LocalSpriteData = new SpriteData(o.ToString(), true);

            if (dict.TryGetValue("UnlockRankAmount", out o))
                UnlockRank = o.ParseInt();

            if (dict.TryGetValue("CanBeGivenToOpponent", out o))
                CanBeGivenToOpponent = o.ParseBool();

            if (dict.TryGetValue("LoyaltyPoints", out o))
                LoyaltyPoints = o.ToString();

            Cost = new CostData(this is IConsumable, dict);
        }

        internal virtual void UpdatePurchasedItem(PurchasedItem item)
        {
            TriggerChangedEvent();
        }

        protected override void Populate(RectTransform activeObject)
        {
            IStoreItemView view = activeObject.GetComponent<IStoreItemView>();
            OnItemChanged += view.Populate;
            view.Populate(this);
        }

        public override void DeactivateObject()
        {
            IStoreItemView view = activeObject.GetComponent<IStoreItemView>();
            OnItemChanged -= view.Populate;

            base.DeactivateObject();
        }

        public void Purchase()
        {
            if (Cost.IAPCost != null)
            {
                InitiateIAP();
                return;
            }

            int count = 0;
            for (int i = 0; i < Cost.GTCosts.Count; i++)
                if (Cost.GTCosts[i].HaveEnoughMoney())
                    count++;

            List<SmallPopupButton> buttons = new List<SmallPopupButton>();
            List<string> contentStrings = new List<string>();
            contentStrings.Add(Utils.LocalizeTerm("Purchase {0} with {1}", new object[] { Name, Cost.GetCostsString() }));
            for (int i = 0; i < Cost.GTCosts.Count; i++)
            {
                int index = i;
                if (Cost.GTCosts[index].HaveEnoughMoney())
                {
                    string buttonText = count > 1 ? Cost.GTCosts[index].GetCostString() : "OK";
                    buttons.Add(new SmallPopupButton(buttonText, () => { PurchaseItem(CreatePaymentMethodDict(Cost.GTCosts[index].PurchaseMethod)); }));
                }
                else
                    contentStrings.Add(GetNotEnoughString(Cost.GTCosts[index].CurrencyName));
            }

            buttons.Add(new SmallPopupButton(count > 0 ? "Cancel" : "OK"));

            PopupController.Instance.ShowSmallPopup("\"" + Name + "\"", contentStrings.ToArray(), buttons.ToArray());
        }

        private string GetNotEnoughString(string currency)
        {
            return Utils.LocalizeTerm("Not enough " + currency + " to purchase this item");
        }

        public void PurchaseInGame(bool OpponentHasIt = false)
        {
            if (!CanBeGivenToOpponent)
                Debug.LogWarning("Buy for Opponent an item not suppose to");

            List<SmallPopupButton> buttons = new List<SmallPopupButton>();
            List<string> contentStrings = new List<string>();
            contentStrings.Add("Purchase " + Name + " with " + Cost.GetCostsString());

            int count = Cost.IAPCost != null ? 1 : 0;
            for (int i = 0; i < Cost.GTCosts.Count; i++)
                if (Cost.GTCosts[i].HaveEnoughMoney())
                    count++;

            if (isSelectable && !m_selected)
                buttons.Add(new SmallPopupButton(SelectedString, Select));

            for (int i = 0; i < Cost.GTCosts.Count; i++)
            {
                int index = i;
                if (Cost.GTCosts[index].HaveEnoughMoney())
                {
                    if (!isSelectable)
                    {
                        string buttonText;
                        if (!OpponentHasIt)
                            buttonText = count > 1 ? string.Format("Buy for yourself {0}", Cost.GTCosts[index].GetCostString()) : "Buy for yourself";
                        else
                            buttonText = count > 1 ? Cost.GTCosts[index].GetCostString() : "OK";

                        buttons.Add(new SmallPopupButton(buttonText, () => {
                            PurchaseItem(CreatePaymentMethodDict(Cost.GTCosts[index].PurchaseMethod));
                        }));
                    }

                    if (!OpponentHasIt)
                    {
                        string OpponentButtonText = count > 1 ? string.Format("Buy for opponent {0}", Cost.GTCosts[index].GetCostString()) : "Buy for opponent";

                        buttons.Add(new SmallPopupButton(OpponentButtonText, () => {
                            Dictionary<string, object> dic = CreatePaymentMethodDict(Cost.GTCosts[index].PurchaseMethod);
                            dic.AddOrOverrideValue("ForOpponent", true);
                            PurchaseItem(dic);
                        }));
                    }
                }
                else
                    contentStrings.Add("Not enough " + Cost.GTCosts[index].CurrencyName + " to purchase this item");
            }

            //  TO DO : Create InatiateIAP ForOpponent
            //if (Cost.IAPCost != null)
            //{
            //    buttons.Add(new SmallPopupButton(string.Format(buttonText, Cost.IAPCost.GetIAPCostString()), true, InitiateIAP));

            //    if (inGame && m_canBeGiven)
            //        buttons.Add(new SmallPopupButton(string.Format(OpponentButtonText, Cost.IAPCost.GetIAPCostString()), true, InitiateIAP));
            //    else
            //    {
            //        // we dont show pop-up for IAP
            //        InitiateIAP();
            //        return;
            //    }
            //}

            buttons.Add(new SmallPopupButton(count > 0 ? "Cancel" : "OK"));

            PopupController.Instance.ShowSmallPopup("\"" + Name + "\"", contentStrings.ToArray(), buttons.ToArray());
        }

        protected virtual void PurchaseSuccessfull()
        {
            TriggerChangedEvent();
            AfterPurchaseAction();
        }

        protected virtual void AfterPurchaseAction()
        {
            PopupController.Instance.ShowSmallPopup("Purchase Successfull");
            if (BoughtSound != null)
                MenuSoundController.Instance.Play(BoughtSound);
        }

        private Dictionary<string, object> CreatePaymentMethodDict(string method)
        {
            return new Dictionary<string, object>() { { PassableVariable.PurchaseMethod.ToString(), method } };
        }

        private void InitiateIAP()
        {
            LoadingController.Instance.ShowPageLoading();
#if !UNITY_EDITOR
            if(Cost.IAPCost.PurchaseMethod == "FacebookIAP")
                FacebookPurchaser.Instance.BuyProduct(Cost.IAPCost.ItemId, InAppPurchaseCallback);
            else
#endif
            Purchaser.Instance.BuyProduct(Cost.IAPCost.ItemId, InAppPurchaseCallback);
        }

        private void PurchaseItem(Dictionary<string, object> dic)
        {
            LoadingController.Instance.ShowPageLoading();
            dic.Add(PassableVariable.ItemId.ToString(), Id);
            WebSocketKit.Instance.AckEvents[RequestId.PurchaseItem] += OnPurchaseItem;
            UserController.Instance.SendPurchaseToServer(dic);
        }

        public bool IsLocked()
        {
            return false; //m_unlockRank > userRank;
        }

        internal virtual bool IsSelectable(int userRank)
        {
            return !IsLocked() && isSelectable;
        }

        public void Select()
        {
            SelectAction();
        }

        public override string ToString()
        {
            return GetType() + " ItemID: " + Id + " Name: " + Name + " CostData: " + Cost;
        }

        protected void TriggerChangedEvent()
        {
            if (OnItemChanged != null)
                OnItemChanged(this);
        }

        protected virtual void OnUnselect()
        {
        }

        #region Events

        void InAppPurchaseCallback(InAppPurchaseResponse response)
        {
            Debug.Log("InAppPurchaseCallback " + response.ToString());
            if (response.responseType == ResponseType.OK)
            {
                Dictionary<string, object> dict = CreatePaymentMethodDict(Cost.IAPCost.PurchaseMethod);
                string transactionID = response.product != null ? response.product.transactionID : response.transactionID;
                dict.Add(PassableVariable.TransactionId.ToString(), transactionID);

                Dictionary<string, object> completedict = new Dictionary<string, object>(dict);
                completedict.Add(PassableVariable.ItemId.ToString(), Id);
                SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
                user.pendingInAppPurchase.Add(completedict);
                SavedUsers.SaveUserToFile(user);

                Debug.Log("External Payment succesfull, notice the websocket : " + dict.Display());
                PurchaseItem(dict);
            }
            else
            {
                LoadingController.Instance.HidePageLoading();
                PopupController.Instance.ShowSmallPopup("Purchase Failed", new string[] { response.failureReason });
            }
        }

        void OnPurchaseItem(Ack ack)
        {
            WebSocketKit.Instance.AckEvents[RequestId.PurchaseItem] -= OnPurchaseItem;

            Debug.Log("GTPurchaseCallback " + ack.ToString());
            LoadingController.Instance.HidePageLoading();
            switch (ack.Code)
            {
                case WSResponseCode.OK:
                    if (Cost.IAPCost != null)
                    {
                        SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
                        user.pendingInAppPurchase.Clear();
                        SavedUsers.SaveUserToFile(user);
                    }
                    PurchaseSuccessfull();
                    break;
                default:
                    PopupController.Instance.ShowSmallPopup(Utils.LocalizeTerm("Unexpected error, Try again or contact support. code: {0}", (int)ack.Code),
                        new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)));
                    break;
            }
        }
        #endregion Events

        #region Static Functions
        public static StoreItem CreateStoreItem(Dictionary<string, object> dict, GameObject prefab, string BuyString, bool CanBeGiven, bool consumeOnDeselect, AudioClip boughtSound)
        {

            object o;
            if (dict.TryGetValue("ItemType", out o))
            {
                if (o.ToString().Equals("Ownable"))
                    return new OwnableItem(dict, prefab, BuyString, CanBeGiven, boughtSound);
                if (o.ToString().Equals("Consumable"))
                    return new ConsumableItem(dict, prefab, BuyString, CanBeGiven, consumeOnDeselect, boughtSound);
            }

            return new StoreItem(dict, prefab, BuyString, CanBeGiven, boughtSound);
        }
        #endregion Static Functions
    }
}
