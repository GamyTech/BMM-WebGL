using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using GT.User;

public class MobileNavigation : MonoBehaviour
{
    public bool isOpen { get; private set; }
    public delegate void NavTrigger(bool open);
    public event NavTrigger OnNavOpen;

    public Text LoyaltyAmountText;
    public Text CashAmountText;
    public Text LevelText;
    public Slider ExperienceSlider;
    public Text ExperienceProgressText;

    public GameObject Content;
    public Image CloseButtonImage;
    public SmoothMove smoothMove;
    public Text UserName;
    public Text Email;
    public Text CountryName;
    public Image CountryFlag;
    public Image UserPicture;
    public Image UserLuckyItem;

    public Toggle ProfileToggle;
    public Toggle WithdrawToggle;
    public Toggle BalanceToggle;
    public Toggle FriendsToggle;
    public Toggle AccountToggle;
    public Toggle SupportToggle;
    public Toggle SettingsToggle;
    public ToggleGroup toggleGroup;
    public FadingElement fadingElement;

    private UnityAction UnregisterUserAction;
    private Toggle m_ToggleSelected;
    private Enums.NavigationCategory m_curentCat;
    private bool m_ManualChange = true;
    private float PanelWidth = 0.0f;

    void Awake()
    {
#if UNITY_IOS || UNITY_ANDROID
        Content.SetActive(true);

        PageController.OnPageChanged += PageController_OnPageChanged;
        UserController.OnGTUserChanged += UserController_OnGTUserChanged;

        if (AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends)
            WithdrawToggle.gameObject.SetActive(false);

        RectTransform PanelRect = smoothMove.transform as RectTransform;
        PanelWidth = PanelRect.rect.size.x;

        if (UserController.Instance != null && UserController.Instance.gtUser != null)
            InitUser(UserController.Instance.gtUser);

        isOpen = true;
        CloseMenu();
#else
        Destroy(this.gameObject);
#endif
    }

    private void OnDestroy()
    {
#if UNITY_IOS || UNITY_ANDROID
        UserController.OnGTUserChanged -= UserController_OnGTUserChanged;
        PageController.OnPageChanged -= PageController_OnPageChanged;
        Unregister();
#endif
    }


    private void InitUser(GTUser user)
    {
        UserName.text = user.UserName;
        Email.text = user.Email;
        UserPicture.sprite = user.Avatar;
        CountryName.text = user.Country;
        CountryFlag.sprite = user.CountryFlag;

        User_OnLuckyItemChanged(user.LuckyItemSprite);
        Wallet_OnLoyaltyChanged(user.wallet.LoyaltyPoints);
        Wallet_OnTotalCashChanged(user.wallet.TotalCash);
        Rank_OnXPChanged(user.rank.XP);
        Register(user);
    }

    private void Register(GTUser user)
    {
        user.wallet.OnTotalCashChanged += Wallet_OnTotalCashChanged;
        user.wallet.OnLoyaltyChanged += Wallet_OnLoyaltyChanged;
        user.OnLuckyItemChanged += User_OnLuckyItemChanged;
        user.OnAvatarChanged += GtUser_OnAvatarChanged;
        user.rank.OnXPChanged += Rank_OnXPChanged;

        UnregisterUserAction = () =>
        {
            user.wallet.OnTotalCashChanged -= Wallet_OnTotalCashChanged;
            user.wallet.OnLoyaltyChanged -= Wallet_OnLoyaltyChanged;
            user.OnLuckyItemChanged -= User_OnLuckyItemChanged;
            user.OnAvatarChanged -= GtUser_OnAvatarChanged;
            user.rank.OnXPChanged -= Rank_OnXPChanged;
        };
    }

    private void Unregister()
    {
        if (UnregisterUserAction != null)
            UnregisterUserAction();
        UnregisterUserAction = null;
    }

    private void Wallet_OnLoyaltyChanged(int newPoints)
    {
        LoyaltyAmountText.text = Wallet.AmountToString(newPoints, 1);
    }

    private void Wallet_OnTotalCashChanged(float newMoney)
    {
        CashAmountText.text = !MobileHeader.HideCurrency ? Wallet.AmountToString(newMoney, 2) : "******";
    }

    private void Rank_OnXPChanged(int XP)
    {
        LevelText.text = UserController.Instance.gtUser.rank.Level.ToString();
        ExperienceProgressText.text = XP + "/" + UserController.Instance.gtUser.rank.PointsForNextLevel;
        ExperienceSlider.value = 0.08f + (UserController.Instance.gtUser.rank.LevelProgress * 0.92f);
    }

    private void UserController_OnGTUserChanged(GTUser newValue)
    {
        Unregister();
        if (newValue != null)
            InitUser(newValue);
    }

    private void User_OnLuckyItemChanged(Sprite sprite)
    {
        UserLuckyItem.sprite = sprite;
    }

    private void GtUser_OnAvatarChanged(Sprite newValue)
    {
        if (newValue != null)
            UserPicture.sprite = newValue;
    }

    private void PageController_OnPageChanged(Page page)
    {
        CloseMenu();

        m_curentCat = page.NavCat;

        switch (m_curentCat)
        {
            case Enums.NavigationCategory.Profile:
                m_ToggleSelected = ProfileToggle;
                break;
            case Enums.NavigationCategory.Balance:
                m_ToggleSelected = BalanceToggle;
                break;
            case Enums.NavigationCategory.Withdraw:
                m_ToggleSelected = WithdrawToggle;
                break;
            case Enums.NavigationCategory.Challenge:
                m_ToggleSelected = FriendsToggle;
                break;
            case Enums.NavigationCategory.Support:
                m_ToggleSelected = SupportToggle;
                break;
            case Enums.NavigationCategory.Settings:
                m_ToggleSelected = SettingsToggle;
                break;

            default:
                if (m_ToggleSelected != null)
                {
                    toggleGroup.allowSwitchOff = true;
                    m_ToggleSelected.isOn = false;
                }
                m_ToggleSelected = null;
                return;
        }

        toggleGroup.allowSwitchOff = false;
        m_ManualChange = false;
        m_ToggleSelected.isOn = true;
        m_ManualChange = true;
    }

    public void TriggerOpen(bool open)
    {
        if (isOpen == open)
            return;
        
        isOpen = open;
        CloseButtonImage.raycastTarget = isOpen;
        smoothMove.SetPosistionX(isOpen ? PanelWidth : 0.0f);
        if (isOpen)
        {
            fadingElement.FadeIn();
            if (!MobileHeader.HideCurrency)
                CashAmountText.text = Wallet.AmountToString(UserController.Instance.wallet.TotalCash, 2);
            else
                CashAmountText.text = "******";
        }
        else
            fadingElement.FadeOut();

        if (OnNavOpen != null)
            OnNavOpen(isOpen);
    }

    public void ToggleOpen()
    {
        TriggerOpen(!isOpen);
    }

    public void OpenMenu()
    {
        TriggerOpen(true);
    }

    public void CloseMenu()
    {
        TriggerOpen(false);
    }

    private void OnToggle(bool isOn, Enums.PageId pageId, Toggle toggle)
    {
        if (isOn && m_ManualChange)
        {
            PageController.Instance.ChangePage(pageId);
            m_ToggleSelected = toggle;
        }
        CloseMenu();
    }

    #region Input
    public void OnWithdrawToggle(bool isOn)
    {
        OnToggle(isOn, Enums.PageId.Cashout, WithdrawToggle);
    }

    public void OnBalanceToggle(bool isOn)
    {
        OnToggle(isOn, Enums.PageId.Balance, BalanceToggle);
    }

    public void OnFriendsToggle(bool isOn)
    {
        OnToggle(isOn, Enums.PageId.Challenge, FriendsToggle);
    }

    public void OnProfileToggle(bool isOn)
    {
        OnToggle(isOn, Enums.PageId.ChangeProfile, ProfileToggle);
    }

    public void OnSettingsToggle(bool isOn)
    {
        OnToggle(isOn, Enums.PageId.Settings, SettingsToggle);
    }

    public void OnSupportToggle(bool isOn)
    {
        OnToggle(isOn, Enums.PageId.ContactSupport, SupportToggle);
    }
    #endregion Input
}
