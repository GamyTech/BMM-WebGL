using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class MobileHeader : Widget {

    const float changeTime = 2f;

    public Text LevelText;
    public Slider ExperienceSlider;
    public Text ExperienceProgressText;
    public Text LoyaltyAmountText;
    public Text CashAmountText;
    public Text BubbleBalanceText;

    public RectTransform BubbleContainer;
    public GameObject PointsBubble;
    public GameObject LoyaltyBubble;
    public GameObject CurrencyBubble;

    private bool ClickOnSelectable;
    private bool ClickSomewhere;
    UnityAction Unregister;

    public static bool HideCurrency { get; private set; }
    static float currentCashCurrency = 0;
    static int currentLoyaltyPoints = 0;
    static int currentExperiencePoints = 0;
    static bool firstTime = true;

    public override void EnableWidget()
    {
        base.EnableWidget();

        Wallet wallet = UserController.Instance.wallet;
        Rank rank = UserController.Instance.gtUser.rank;
        ExtendedInputModule.OnScreenTouchEnd += ExtendedInputModule_OnScreenTouched;
        wallet.OnTotalCashChanged += UpdateCash;
        wallet.OnLoyaltyChanged += UpdateLoyalty;
        rank.OnXPChanged += UpdateXP;

        Unregister = () =>
        {
            wallet.OnTotalCashChanged -= UpdateCash;
            wallet.OnLoyaltyChanged -= UpdateLoyalty;
        };

        if (firstTime)
        {
            SetTotalCash(wallet.TotalCash);
            SetLoyalty(wallet.LoyaltyPoints);
            SetExperience(rank.XP);
            firstTime = false;
        }
        else
        {
            UpdateCash(wallet.TotalCash);
            UpdateLoyalty(wallet.LoyaltyPoints);
            UpdateXP(rank.XP);
        }

        HideBubbles();

        BubbleContainer.SetParent(WidgetController.Instance.floatingTopWidgetContainer.transform.parent, false);
        BubbleContainer.anchoredPosition = Vector3.zero;
    }

    public override void DisableWidget()
    {
        BubbleContainer.transform.SetParent(transform);

        Unregister();
        ExtendedInputModule.OnScreenTouchEnd -= ExtendedInputModule_OnScreenTouched;
        StopAllCoroutines();

        base.DisableWidget();
    }

    protected override void FreeResources()
    {
        Unregister();
        ExtendedInputModule.OnScreenTouchEnd -= ExtendedInputModule_OnScreenTouched;

        StopAllCoroutines();
        base.FreeResources();
    }

    private void Update()
    {
        if (ClickSomewhere && !ClickOnSelectable)
            HideBubbles();

        ClickSomewhere = false;
        ClickOnSelectable = false;
    }

    #region Events
    void UpdateCash(float cashCurrency)
    {
        StartCoroutine(Change.GenericChange(currentCashCurrency, cashCurrency, changeTime, Change.EaseOutQuad, a => SetTotalCash(a), () => SetTotalCash(cashCurrency)));
        SetBalanceBubble();
    }

    void UpdateLoyalty(int loyaltyPoints)
    {
        StartCoroutine(Change.GenericChange((float)currentLoyaltyPoints, loyaltyPoints, changeTime, Change.EaseOutQuad, a => SetLoyalty(a), () => SetLoyalty(loyaltyPoints)));
    }

    void UpdateXP(int xp)
    {
        StartCoroutine(Change.GenericChange((float)currentExperiencePoints, xp, changeTime, Change.EaseOutQuad, a => SetExperience(a), () => SetExperience(xp)));
    }

    private void ExtendedInputModule_OnScreenTouched()
    {
        ClickSomewhere = true;
    }
    #endregion Events

    #region Inputs
    public void ExperienceButton()
    {
        ClickOnSelectable = true;
        PointsBubble.SetActive(!PointsBubble.activeSelf);
        LoyaltyBubble.SetActive(false);
        CurrencyBubble.SetActive(false);
    }

    public void LoyaltyButton()
    {
        ClickOnSelectable = true;
        PointsBubble.SetActive(false);
        LoyaltyBubble.SetActive(!LoyaltyBubble.activeSelf);
        CurrencyBubble.SetActive(false);
    }

    public void MoneyButton()
    {
        ClickOnSelectable = true;
        PointsBubble.SetActive(false);
        LoyaltyBubble.SetActive(false);
        CurrencyBubble.SetActive(!CurrencyBubble.activeSelf);
        if(CurrencyBubble.activeSelf)
            SetBalanceBubble();
    }

    public void HideCurrencyToggle(bool isOn)
    {
        ClickOnSelectable = true;
        HideCurrency = !isOn;
        SetTotalCash(UserController.Instance.wallet.TotalCash);
    }
    #endregion Inputs

    #region Aid Functions
    private void HideBubbles()
    {
        PointsBubble.SetActive(false);
        LoyaltyBubble.SetActive(false);
        CurrencyBubble.SetActive(false);
    }

    private void SetTotalCash(float amount)
    {
        currentCashCurrency = amount;
        CashAmountText.text = !HideCurrency? Wallet.AmountToString(currentCashCurrency, 2) : "******";
        CashAmountText.alignByGeometry = HideCurrency;
    }

    private void SetBalanceBubble()
    {
        string dollarSymbol = ": <color=#388E3C>" + Wallet.CashPostfix;
        BubbleBalanceText.text =Utils.LocalizeTerm("Balance") + dollarSymbol + UserController.Instance.wallet.TotalCash + "</color>\n"
            + Utils.LocalizeTerm("Real Money") + dollarSymbol + UserController.Instance.wallet.Cash + " </color>\n"
            + Utils.LocalizeTerm("Bonus Cash") + dollarSymbol + UserController.Instance.wallet.BonusCash + "</color>";
    }

    private void SetLoyalty(float amount, bool animating = false)
    {
        currentLoyaltyPoints = (int)amount;
        LoyaltyAmountText.text = Wallet.AmountToString(currentLoyaltyPoints, 1);
    }

    private void SetExperience(float XP, bool animating = false)
    {
        currentExperiencePoints = (int)XP;
        LevelText.text = UserController.Instance.gtUser.rank.Level.ToString();
        ExperienceProgressText.text = currentExperiencePoints + "/" + UserController.Instance.gtUser.rank.PointsForNextLevel;
        ExperienceSlider.value = 0.08f + (UserController.Instance.gtUser.rank.LevelProgress * 0.92f);
    }
    #endregion Aid Functions
}
