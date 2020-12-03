using System.Collections;
using System.Collections.Generic;
using GT.User;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PCHeaderWidget : Widget
{

    const float balanceChangeTime = 2f;
    const char NAV_CHAR = '\\';
    const char BACK_CHAR = ']';

    public Button BackButton;
    public Button Cashier;
    public Image CashierButtonImage;
    public Sprite CashierButtonNormal;
    public Sprite CashierButtonEvent;
    public Image UserLuckyItem;


    public Text LoyaltyAmountText;
    public Text VirtualAmountText;
    public Text CashAmountText;

    static bool firstTime = true;
    public Text CashierButtonText;

    public GameTitleWidget gameTitleWidget;
    public AdvertisementWidget advertisementWidget;

    public Text UserName;
    public Image UserPicture;

    public List<RectTransform> LeftAdElements;
    public LayoutElement AdButton;

    private Canvas canvas;
    private float halfAdButtonWidth;
    public SmoothResize SizeEquilizer;
    private UnityAction UnregisterUserAction;

    static int currentCoinsAmount = 0;
    static float currentCashAmount = 0;
    static int currenctLoyaltyPoints = 0;

    private Enums.NavigationState navState = Enums.NavigationState.Blocked;
    Enums.NotificationId[] NotificationList = new Enums.NotificationId[2] { Enums.NotificationId.Bonus, Enums.NotificationId.Cashin };

    public override void EnableWidget()
    {
        base.EnableWidget();

        canvas = this.GetComponentInParent<Canvas>();
        halfAdButtonWidth = AdButton.preferredWidth / 2.0f;

        if (NetworkController.Instance.IsClientGame())
        {
            Cashier.gameObject.SetActive(false);
        }
        if (AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends)
        {
            CashierButtonText.text = "Buy Coins";
            CashAmountText.transform.parent.gameObject.SetActive(false);
        }
        else if (AppInformation.GAME_ID == Enums.GameID.BackgammonBlockChain)
        {
            CashierButtonText.text = "GET TOKENS";
            CashAmountText.transform.parent.gameObject.SetActive(false);
            Wallet.SetGradientForMatchKind(Enums.MatchKind.Cash, VirtualAmountText.GetComponent<UnityEngine.UI.Gradient>());
        }
        else
            VirtualAmountText.transform.parent.gameObject.SetActive(false);

        SettingsController.OnScreenSizeChanged += SettingsController_OnScreenSizeChanged;

        BackButton.gameObject.SetActive(false);
        gameTitleWidget.EnableWidget();
        advertisementWidget.EnableWidget();

        if (UserController.Instance.gtUser != null)
        {
            RegisterUser(UserController.Instance.gtUser);
            InitUser(UserController.Instance.gtUser);
        }
        NotificationSystemController.Instance.RegisterForNotification(NotificationList, OnBonusChanged);
        OnBonusChanged(NotificationSystemController.Instance.GetNotificationCount(NotificationList));

        Canvas.ForceUpdateCanvases();
        AdjustAdPos();
    }

    public override void DisableWidget()
    {
        SettingsController.OnScreenSizeChanged -= SettingsController_OnScreenSizeChanged;
        if (NotificationSystemController.Instance != null)
        {
            NotificationSystemController.Instance.UnregisterForNotification(NotificationList, OnBonusChanged);
        }

        if(UnregisterUserAction != null)
            UnregisterUserAction();

        gameTitleWidget.DisableWidget();
        advertisementWidget.DisableWidget();

        base.DisableWidget();
    }

    public override void RefreshWidget()
    {
        base.RefreshWidget();
        RefreshPage(PageController.Instance.CurrentPage);
    }

    private void InitUser(GTUser user)
    {
        UserName.text = user.UserName;
        if (user.Avatar != null)
            UserPicture.sprite = user.Avatar;
        UserLuckyItem.sprite = user.LuckyItemSprite;

        Wallet wallet = UserController.Instance.wallet;

        if (firstTime)
        {
            SetCoins(wallet.VirtualCoins);
            SetCash(wallet.TotalCash);
            SetLoyalty(wallet.LoyaltyPoints);
            firstTime = false;
        }
        else
        {
            UpdateCoins(wallet.VirtualCoins);
            UpdateCash(wallet.TotalCash);
            UpdateLoyalty(wallet.LoyaltyPoints);
        }
    }

    private void RegisterUser(GTUser user)
    {
        if (user == null) return;

        UnregisterUserAction = () =>
        {
            user.OnAvatarChanged -= GtUser_OnAvatarChanged;
            user.OnLuckyItemChanged -= GtUser_OnLuckyItemChanged;
            user.wallet.OnTotalCashChanged -= UpdateCash;
            user.wallet.OnVirtualChanged -= UpdateCoins;
            user.wallet.OnLoyaltyChanged -= UpdateLoyalty;
        };

        user.OnAvatarChanged += GtUser_OnAvatarChanged;
        user.OnLuckyItemChanged += GtUser_OnLuckyItemChanged;
        user.wallet.OnTotalCashChanged += UpdateCash;
        user.wallet.OnVirtualChanged += UpdateCoins;
        user.wallet.OnLoyaltyChanged += UpdateLoyalty;
    }

    private void GtUser_OnAvatarChanged(Sprite newValue)
    {
        if (UserPicture != null)
            UserPicture.sprite = newValue;
    }

    private void GtUser_OnLuckyItemChanged(Sprite newValue)
    {
        if (newValue != null)
            UserLuckyItem.sprite = newValue;
    }

    private void SettingsController_OnScreenSizeChanged(int width, int height)
    {
        AdjustAdPos();
    }

    private void AdjustAdPos()
    {
        float leftSize = 0.0f;
        for (int x = 0; x < LeftAdElements.Count; ++x)
            leftSize += LeftAdElements[x].rect.width;

        float deficit = ((float)Screen.width / (2.0f * canvas.scaleFactor)) - leftSize - halfAdButtonWidth;
        deficit = deficit > 0.0f ? deficit : 0.0f;
        SizeEquilizer.targetWidth = deficit;
    }

    private void RefreshPage(Page page)
    {
        SetNavigationState(page.NavState);
        AdjustAdPos();
    }

    void SetNavigationState(Enums.NavigationState state)
    {
        if (navState == state)
            return;

        BackButton.onClick.RemoveAllListeners();

        BackButton.gameObject.SetActive(state != Enums.NavigationState.Blocked);

        switch (state)
        {
            case Enums.NavigationState.Menu:
                BackButton.onClick.AddListener(GoHome);
                break;
            case Enums.NavigationState.Cancel:
            case Enums.NavigationState.Back:
                BackButton.onClick.AddListener(BackAction);
                break;
        }

        navState = state;
    }

    private void UpdateCoins(int coins)
    {
        StartCoroutine(Change.GenericChange((float)currentCoinsAmount, coins, balanceChangeTime, Change.EaseOutQuad, a => SetCoins((int)a, true), () => SetCoins(coins)));
    }

    private void UpdateCash(float cash)
    {
        StartCoroutine(Change.GenericChange(currentCashAmount, cash, balanceChangeTime, Change.EaseOutQuad, a => SetCash(a, true), () => SetCash(cash)));
    }

    private void UpdateLoyalty(int loyalty)
    {
        StartCoroutine(Change.GenericChange((float)currenctLoyaltyPoints, loyalty, balanceChangeTime, Change.EaseOutQuad, a => SetLoyalty((int)a, true), () => SetLoyalty(loyalty)));
    }

    private void SetCoins(int amount, bool animating = false)
    {
        currentCoinsAmount = amount;
        VirtualAmountText.text = Wallet.VirtualPostfix + Wallet.AmountToString(amount);
    }

    private void SetCash(float amount, bool animating = false)
    {
        currentCashAmount = amount;
        CashAmountText.text = Wallet.SpecialCashPostfix + " " + Wallet.AmountToString(amount, 2);
    }

    private void SetLoyalty(int amount, bool animating = false)
    {
        currenctLoyaltyPoints = amount;
        LoyaltyAmountText.text = Wallet.LoyaltyPointsPostfix + Wallet.AmountToString(amount, 1);
    }

    void OnBonusChanged(int count)
    {
        CashierButtonImage.sprite = count > 0? CashierButtonEvent : CashierButtonNormal;
    }

    #region Back Button Actions
    void GoHome()
    {
        PageController.Instance.ChangePage(Enums.PageId.Home);
    }

    void BackAction()
    {
        PageController.Instance.BackPage();
    }
    #endregion Back Button Actions

    #region Buttons
    public void ProfileButton()
    {
        PageController.Instance.ChangePage(Enums.PageId.ChangeProfile);
    }

    public void CashierButton()
    {
        if (NetworkController.Instance.IsClientGame()) return;
        if (AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends)
            PageController.Instance.ChangePage(Enums.PageId.InAppPurchase);
        else
            PageController.Instance.ChangePage(Enums.PageId.QuickAccessCashin);
    }

    public void LoyaltyButton()
    {
        PageController.Instance.ChangePage(Enums.PageId.LoyaltyStore);
    }
    #endregion Buttons
}
