using UnityEngine;
using System.Collections.Generic;
using GT.Websocket;
using System;
using GT.Store;

namespace GT.User
{
    public class GTUser
    {
        #region Delegates And Events
        public delegate void Changed<T>(T newValue);

        public event Changed<bool> OnVerifiedChanged;
        public event Changed<bool> OnPhoneVerifiedChanged;
        public event Changed<bool> OnCashOutPendingChanged;
        public event Changed<Sprite> OnAvatarChanged;
        public event Changed<Sprite> OnLuckyItemChanged;
        public event Changed<DepositData> OnDepositInfoChanged;
        public event Changed<TimelyBonus> OnTimelyBonusChanged;
        public event Changed<HistoryMatches> OnMatchHistoryChanged;
        public event Changed<TransactionsHistory> OnTransactionHistoryChanged;
        public event Changed<List<SavedCreditCard>> OnCreditCardListChanged;
        //public event Changed<GTMails> OnGTMailsChanged;
        #endregion Delegates And Events

        public string Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string Country { get; private set; }
        public string CountryISO { get; private set; }
        public Sprite CountryFlag { get; private set; }
        public bool MustBeVerified { get; protected set; }
        public float AccumulatedDeposit { get; protected set; }
        public float MaxCashinForStage { get; protected set; }
        public HistoryMatches MatchHistoryData { get; protected set; }
        public TransactionsHistory TransactionHistoryData;
        public GTMails MailsData { get; protected set; }
        public Enums.VerificationRank Verification { get; protected set; }
        public Wallet wallet { get; protected set; }
        public Rank rank { get; protected set; }

        public CashInData cashInData;
        public CashOutData cashOutData;
        public UserVirificationData verificationData;

        private Store.Store LuckyItemStore;

        private bool m_isVerified;
        public bool Verified
        {
            get { return m_isVerified; }
            protected set
            {
                if (Utils.SetProperty(ref m_isVerified, value) && OnVerifiedChanged != null)
                    OnVerifiedChanged(m_isVerified);
            }
        }
        
        public int TotalMatchesCount;

        public bool NeedsVerification { get; protected set; }

        private bool m_isPhoneVerified;
        public bool IsPhoneVerified
        {
            get { return m_isPhoneVerified; }
            set
            {
                if (Utils.SetProperty(ref m_isPhoneVerified, value) && OnPhoneVerifiedChanged != null)
                    OnPhoneVerifiedChanged(m_isPhoneVerified);
            }
        }

        private bool m_cashoutPending;
        public bool CashOutPending
        {
            get { return m_cashoutPending; }
            set
            {
                if (Utils.SetProperty(ref m_cashoutPending, value) && OnCashOutPendingChanged != null)
                    OnCashOutPendingChanged(m_cashoutPending);
            }
        }

        private Sprite m_avatar = Texture2D.whiteTexture.ToSprite();
        public Sprite Avatar
        {
            get { return m_avatar; }
            protected set
            {
                m_avatar = value;
                if (OnAvatarChanged != null)
                    OnAvatarChanged(m_avatar);
            }
        }

        private DepositData m_depositInfo;
        public DepositData DepositInfo
        {
            get { return m_depositInfo; }
            protected set
            {
                if (Utils.SetProperty(ref m_depositInfo, value) && OnDepositInfoChanged != null)
                    OnDepositInfoChanged(m_depositInfo);
            }
        }

        private TimelyBonus m_timelyBonus;

        public TimelyBonus TimelyBonusData
        {
            get { return m_timelyBonus; }
            protected set
            {
                if (Utils.SetProperty(ref m_timelyBonus, value) && OnTimelyBonusChanged != null)
                    OnTimelyBonusChanged(m_timelyBonus);
            }
        }

        private Stores m_stores;
        public Stores StoresData
        {
            get { return m_stores; }
            protected set
            {
                if (Utils.SetProperty(ref m_stores, value))
                {
                    if (LuckyItemStore != null)
                        LuckyItemStore.selected.OnSelectedListChanged -= SetLuckyItem;
                    LuckyItemStore = m_stores.GetStore(Enums.StoreType.LuckyItems);
                    LuckyItemStore.selected.OnSelectedListChanged += SetLuckyItem;
                    SetLuckyItem(new List<StoreItem>() { LuckyItemStore.selected.GetFirstSelected() });
                    m_stores.SetPurchasedItems(m_purchasedItems);
                    InAppPurchase.Purchaser.Instance.InitProducts(m_stores.GetIAPItemIds());
                }
            }
        }

        private Sprite m_luckyItemSprite;
        public Sprite LuckyItemSprite
        {
            get { return m_luckyItemSprite ?? (m_luckyItemSprite = AssetController.Instance.EmptySprite); }
            protected set
            {
                if(Utils.SetProperty(ref m_luckyItemSprite, value?? AssetController.Instance.EmptySprite) && OnLuckyItemChanged != null)
                    OnLuckyItemChanged(m_luckyItemSprite);
            }
        }

        private PurchasedItems m_purchasedItems;
        private PurchasedItems purchasedItems
        {
            get { return m_purchasedItems; }
            set
            {
                if (Utils.SetProperty(ref m_purchasedItems, value) && m_stores != null)
                {
                    for (int x = 0; x < m_purchasedItems.items.Count; ++x)
                        if (m_purchasedItems.items[x].storeType == Enums.StoreType.Boards)
                            AssetController.Instance.GetBoardAsset(m_purchasedItems.items[x].itemId, null);
                    m_stores.SetPurchasedItems(m_purchasedItems);
                }
            }
        }

        public struct SavedCreditCard
        {
            public string id;
            public string lastDigits;
            public Enums.CreditCardType cardType;
        }

        private List<SavedCreditCard> m_savedCreditCards;
        public List<SavedCreditCard> savedCreditCards
        {
            get { return m_savedCreditCards; }
            set
            {
                if (Utils.SetProperty(ref m_savedCreditCards, value) && OnCreditCardListChanged != null)
                    OnCreditCardListChanged(m_savedCreditCards);
            }
        }

        #region Constractors
        public GTUser ( Dictionary<string, object> userDetailsDict, Dictionary<string, object> wallet, Dictionary<string, object> verificationDict,
            Dictionary<string, object> dailyBonus)
        {
            RegisterDBEvents();
            cashInData = new CashInData();
            cashOutData = new CashOutData();
            MatchHistoryData = new HistoryMatches();
            TransactionHistoryData = new TransactionsHistory();
            MailsData = new GTMails();

            this.wallet = new Wallet(wallet);
            Verification = Enums.VerificationRank.EmailVerified;
            Email = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.LastGTUserEmail);

            SetVerification(verificationDict);
            SetDetails(userDetailsDict);
            SetDailyBonus(dailyBonus);
            MailsData.InitMails();
        }
        #endregion Constractors

        #region Destructor
        ~GTUser()
        {
            UnregisterDBEvents();
            if(LuckyItemStore != null)
                LuckyItemStore.selected.OnSelectedListChanged -= SetLuckyItem;
        }
        #endregion Destructor

        #region Initialize DB Events
        /// <summary>
        /// Register for all relevent user data coming from database
        /// </summary>
        private void RegisterDBEvents()
        {
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.UpdateUserName, SetUserName);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.UpdateProfilePicture, SetAvatar);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.Verification, SetVerification);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.IsVerified, SetIsVerified);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.UserTotalMatchesCount, SetTotalMatchesCount);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.Score, SetScore);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.CashOutPending, SetCashOutPending);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.DepositInfo, SetDepositInfo);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.TimelyBonus, SetTimelyBonus);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.HistoryMatches, AddMatchHistory);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.PurchasedItems, SetPurchasedItems);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.UpdatedItems, SetUpdatedItems);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.UserDetails, SetDetails);
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.GetCards, SetSavedCreditCards);
        }

        /// <summary>
        /// Unregister for all relevent user data coming from database
        /// </summary>
        private void UnregisterDBEvents()
        {
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.UpdateUserName, SetUserName);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.UpdateProfilePicture, SetAvatar);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.Verification, SetVerification);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.IsVerified, SetIsVerified);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.Score, SetScore);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.CashOutPending, SetCashOutPending);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.DepositInfo, SetDepositInfo);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.TimelyBonus, SetTimelyBonus);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.HistoryMatches, AddMatchHistory);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.PurchasedItems, SetPurchasedItems);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.UpdatedItems, SetUpdatedItems);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.UserDetails, SetDetails);
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.GetCards, SetSavedCreditCards);
        }
        #endregion Initialize DB Events

        #region DB Set Functions
        private void SetDailyBonus(Dictionary<string, object> dailyBonus)
        {
            if (dailyBonus == null) return;

            int day = 0;
            object o;

            if (dailyBonus.TryGetValue("BonusDay", out o))
                day = o.ParseInt();

            Enums.PopupId id = Enums.PopupId.DailyBonus1;
            switch (day)
            {
                case 1:
                    id = Enums.PopupId.DailyBonus1;
                    break;
                case 2:
                    id = Enums.PopupId.DailyBonus2;
                    break;
                case 3:
                    id = Enums.PopupId.DailyBonus3;
                    break;
                case 4:
                    id = Enums.PopupId.DailyBonus4;
                    break;
                case 5:
                    id = Enums.PopupId.DailyBonus5;
                    break;
                default:
                    return;
            }

            int bonus = 0;
            if (dailyBonus.TryGetValue("BonusAmount", out o))
                bonus = o.ParseInt();

            PopupData popup = PopupController.Instance.GetPopup(id);
            popup.textElements[1].text = Wallet.LoyaltyPointsPostfix + bonus.ToString("#,##0");
            PopupController.Instance.ShowPopup(popup);
        }

        private void SetVerification(object o)
        {
            Dictionary<string, object> verificationDict = o as Dictionary<string, object>;
            if (verificationDict == null)
                return;
            object verificationObject;
            if (verificationDict.TryGetValue("VerificationRank", out verificationObject))
                Verification = (Enums.VerificationRank)verificationObject.ParseInt();

            if (verificationDict.TryGetValue("DepositAmount", out verificationObject))
                AccumulatedDeposit = verificationObject.ParseFloat();

            if (verificationDict.TryGetValue("MaxAccumulatedSum", out verificationObject))
                MaxCashinForStage = verificationObject.ParseFloat();

            if (verificationDict.TryGetValue("MustVerified", out verificationObject))
                MustBeVerified = verificationObject.ParseBool();
        }

        private void SetDetails(object o)
        {
            Dictionary<string, object> userDetailsDict = (Dictionary<string, object>)o;
            if (userDetailsDict == null) return;
            object detailObject;
            if (userDetailsDict.TryGetValue("UserId", out detailObject))
                Id = detailObject.ToString();

            if (userDetailsDict.TryGetValue("UserName", out detailObject))
                UserName = detailObject.ToString();
            TrackingKit.SetUserNameTracker(UserName);


            if (userDetailsDict.TryGetValue("IsVerified", out detailObject))
                Verified = detailObject.ParseBool();

            if (userDetailsDict.TryGetValue("UserStatus", out detailObject))
                NeedsVerification = detailObject.ToString() == "NeedVerification";

            if (userDetailsDict.TryGetValue("SmsValidation", out detailObject))
                IsPhoneVerified = detailObject.ParseBool();

            if (userDetailsDict.TryGetValue("Score", out detailObject))
                rank = new Rank(detailObject.ParseInt());

            rank = new Rank(200);

            if (userDetailsDict.TryGetValue("Country", out detailObject))
            {
                Country = detailObject.ToString();
                Sprite flag;
                CountryFlag = AssetController.Instance.CountryFlags.TryGetValue(Country, out flag) ? flag : AssetController.Instance.EmptySprite;
            }

            if (userDetailsDict.TryGetValue("CountryISO", out detailObject))
                CountryISO = detailObject.ToString();

            if (userDetailsDict.TryGetValue("IsCashOutPending", out detailObject))
                CashOutPending = detailObject.ParseBool();

            if (userDetailsDict.TryGetValue("PictureUrl", out detailObject))
                SetAvatar(detailObject);
        }

        private void SetSavedCreditCards(object o)
        {
            List<SavedCreditCard> cards = new List<SavedCreditCard>();
            List<object> cardList = o as List<object>;
            if (cardList == null)
                return;
            object data;
            for (int i = 0; i < cardList.Count; i++)
            {
                SavedCreditCard card = new SavedCreditCard();
                Dictionary<string, object> cardData = cardList[i] as Dictionary<string, object>;
                if (cardData != null && cardData.TryGetValue("Id", out data))
                    card.id = data.ToString();
                if (cardData != null && cardData.TryGetValue("CardLastDigits", out data))
                    card.lastDigits = data.ToString();
                if (cardData != null && cardData.TryGetValue("CardType", out data))
                    if (!Utils.TryParseEnum(data.ToString(), out card.cardType))
                        card.cardType = Enums.CreditCardType.Unknown;
                cards.Add(card);
            }
            
            savedCreditCards = cards;
        }

            private void SetUserName(object o)
        {
            UserName = o.ToString();
        }

        private void SetAvatar(object o)
        {
            ContentController.Instance.DownloadPicture(o.ToString(), t => Avatar = t, AssetController.Instance.DefaultAvatar);
        }

        private void SetIsVerified(object o)
        {
            Verified = o.ParseBool();
        }

        private void SetTotalMatchesCount(object o)
        {
            TotalMatchesCount = o.ParseInt();
        }

        private void SetScore(object o)
        {
            rank.SetXP(o);
        }

        private void SetCashOutPending(object o)
        {
            CashOutPending = o.ParseBool();
        }

        private void SetDepositInfo(object o)
        {
            if (o == null) return;
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.DepositInfo, MiniJSON.Json.Serialize(o));
            DepositInfo = new DepositData((Dictionary<string, object>)o);
        }

        private void SetTimelyBonus(object o)
        {
            TimelyBonusData = new TimelyBonus((Dictionary<string, object>)o);
        }

        private void AddMatchHistory(object o)
        {
            MatchHistoryData.AddMatches(o);

            if (OnMatchHistoryChanged != null)
                OnMatchHistoryChanged(MatchHistoryData);
        }

        private void SetLuckyItem(List<StoreItem> items)
        {
            if (items != null && items.Count > 0 && items[0] != null)
                items[0].LocalSpriteData.LoadImage(UserController.Instance, s => { LuckyItemSprite = s; }, AssetController.Instance.EmptySprite);
        }

        private void SetPurchasedItems(object o)
        {
            if (o == null)
                return;
            try
            {
                purchasedItems = new PurchasedItems((List<object>)o);
            }
            catch (InvalidCastException)
            {
                Debug.Log("Invalid Purchased Items Data");
            }
            catch (Exception e)
            {
                Debug.Log("setPurchasedItems :: " + e.Message);
            }
        }

        private void SetUpdatedItems(object o)
        {
            if (StoresData != null)
            {
                List<object> list = o as List<object>;
                StoresData.UpdatePurchasedItems(new PurchasedItems(list));
            }
        }
        #endregion DB Set Functions
        
        #region Overrides
        public override string ToString()
        {
            return "id:" + Id + " , name:" + UserName + "\n" + wallet;
        }
        #endregion Overrides

        #region Public Methods

        public void InitStores()
        {
            string stores = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Stores);

            try
            {
                StoresData = new Stores(MiniJSON.Json.Deserialize(stores) as List<object>);
            }
            catch (InvalidCastException)
            {
                Debug.Log("Invalid Stores Data");
            }
            catch (Exception e)
            {
                Debug.Log("setStores :: " + e.Message);
            }

        }

        public void AddStoreListener(Enums.StoreType storeType, SelectedListChangedHandler listener)
        {
            Store.Store store = StoresData.GetStore(storeType);
            if (store != null)
                store.selected.OnSelectedListChanged += listener;
        }

        public void RemoveStoreListener(Enums.StoreType storeType, SelectedListChangedHandler listener)
        {
            Store.Store store = StoresData.GetStore(storeType);
            if (store != null)
                store.selected.OnSelectedListChanged -= listener;
        }

        public void AddTransactionHistory(object o)
        {
            TransactionHistoryData.GetNewTransactions(o);

            if (OnTransactionHistoryChanged != null)
                OnTransactionHistoryChanged(TransactionHistoryData);
        }

        public void UpdateWallet(Dictionary<string, object> wallet)
        {
            this.wallet.Reset(wallet);
        }

        public void GetDepositInfoFromPlayErPrefs()
        {
            string savedInfo = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.DepositInfo);
            if (!string.IsNullOrEmpty(savedInfo))
                SetDepositInfo(MiniJSON.Json.Deserialize(savedInfo));
        }

        public void UpdateVerificationState(Ack ack)
        {
            if (ack.Code == WSResponseCode.OK)
            {
                NeedsVerification = false;
                return;
            }
            PopupController.Instance.ShowSmallPopup("Error", new string[] { "Account verification failed" });
        }
        #endregion Public Methods
    }
}
