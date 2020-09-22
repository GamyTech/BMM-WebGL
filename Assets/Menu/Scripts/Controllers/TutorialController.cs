using UnityEngine;
using System.Collections.Generic;
using System;
using GT.Websocket;
using UnityEngine.Events;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    private static TutorialController m_instance;
    public static TutorialController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<TutorialController>()); }
        private set { m_instance = value; }
    }

    private static BaseTutorialView m_tutorialView;
    public BaseTutorialView tutorialView
    {
        get
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            return m_tutorialView ?? (m_tutorialView = FindObjectOfType<PCTutorialView>());
#else
            return m_tutorialView ?? (m_tutorialView = FindObjectOfType<MobileTutorialView>());
#endif
        }
    }

    private List<TutorialStep> m_currentSteps;
    private int currentStepIndex = -1;
    private int TargetStepIndex = 0;
    public bool inTutorial { get; private set; }

    private void Start()
    {
        WidgetController.OnPageWidgetsLoaded += PageController_OnPageWidgetsLoaded;
    }

    private void OnDestroy()
    {
        Instance = null;
        m_tutorialView = null;
        WidgetController.OnPageWidgetsLoaded -= PageController_OnPageWidgetsLoaded;
    }

    public bool StartTutorial()
    {
        inTutorial = false;
        m_currentSteps = AssetController.Instance.WelcomeTutorialSteps;

        int progress = GTDataManagementKit.GetIntFromPrefs(Enums.PlayerPrefsVariable.TutoStep);
        Enums.TutorialProgress progressStep;
        if (Utils.TryParseEnum(progress, out progressStep))
        {
            for (int x = 0; x < m_currentSteps.Count; ++x)
            {
                if ((int)m_currentSteps[x].SavedProgress == progress)
                {
                    inTutorial = true;
                    currentStepIndex = -1;
                    TargetStepIndex = x;
                    SwitchToTargetStep();
                    return true;
                }
            }
        }
        return false;
    }

    private void PageController_OnPageWidgetsLoaded(Enums.PageId pageID)
    {
        if (UserController.Instance.gtUser != null && inTutorial)
            SwitchToTargetStep();
    }

    public void NextStep()
    {
        ++TargetStepIndex;
        if (TargetStepIndex < m_currentSteps.Count)
            SwitchToTargetStep();
        else
        {
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TutoStep, (int)Enums.TutorialProgress.Finished);
            StopTutorial();
        }
    }

    private void SwitchToTargetStep()
    {
        if (currentStepIndex == TargetStepIndex)
            return;

        Enums.PageId page;
        if (PageController.Instance.CurrentPage != null && m_currentSteps[TargetStepIndex].pageId == PageController.Instance.CurrentPage.ID.ToString())
        {
            currentStepIndex = TargetStepIndex;
            Debug.Log("Show tutorial Step Step, SavedProgress = " + m_currentSteps[currentStepIndex].SavedProgress.ToString());
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TutoStep, (int)m_currentSteps[currentStepIndex].SavedProgress);
            tutorialView.ShowNewStep(m_currentSteps[currentStepIndex]);
        }
        else if (Utils.TryParseEnum(m_currentSteps[TargetStepIndex].pageId, out page))
            PageController.Instance.ChangePage(page);
        else
            Debug.LogError(m_currentSteps[TargetStepIndex].pageId + " is noyt parsable into enum");
    }

    private void StopTutorial()
    {
        inTutorial = false;
        tutorialView.CloseTutorial();
    }

    public void ResetTutorialProgress()
    {
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TutoStep, (int)AssetController.Instance.WelcomeTutorialSteps[0].SavedProgress);
    }

    public void SaveTutorialProgressAsFinished()
    {
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TutoStep, (int)Enums.TutorialProgress.Finished);
    }

    #region Ienumerator for steps
    private static IEnumerator WaitLoyaltyChange()
    {
#if UNITY_ANDROID || UNITY_IOS
        Instance.tutorialView.ShowInstruction(false);
#else
        PCTutorialView view = Instance.tutorialView as PCTutorialView;
        view.ShowSecondHole(FindObjectOfType<PCHeaderWidget>().CashAmountText.transform.parent.parent);
#endif
        LoadingController.Instance.ShowPageLoading();
        bool eventCame = false;
        UserController.Instance.wallet.OnLoyaltyChanged += i => { eventCame = true; };
        float time = 0.0f;
        bool popped = false;
        bool GoToSupport = false;
        while (!eventCame && !GoToSupport)
        {
            if (!popped)
            {
                time += Time.deltaTime;

                if (time >= 10.0f)
                {
                    popped = true;
                    PopupController.Instance.ShowSmallPopup("Connection to server failed",
                    new SmallPopupButton("Try Again", () => { popped = false; time = 0.0f; }),
                    new SmallPopupButton("Contact Support", () => {
                        Instance.StopTutorial();
                        LoadingController.Instance.HidePageLoading();
                        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TutoStep, (int)Enums.TutorialProgress.Finished);
                        PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
                        GoToSupport = true;
                    }));
                }
            }
            yield return null;
        }

        UserController.Instance.wallet.OnLoyaltyChanged -= i => { eventCame = true; };
        if (!GoToSupport)
        {
            LoadingController.Instance.HidePageLoading();
            yield return new WaitForSeconds(3.0f);
#if UNITY_ANDROID || UNITY_IOS
            Instance.tutorialView.ShowInstruction(true);
#else
            view.HideSecondHole();
#endif
        }
    }

    private static IEnumerator WaitParticulesBonus()
    {

        Instance.tutorialView.ShowInstruction(false);
        bool eventCame = false;
        WebSocketKit.Instance.AckEvents[RequestId.CollectTimelyBonus] += i => { eventCame = true; };
        float time = 0.0f;
        bool popped = false;
        bool GoToSupport = false;
        while (!eventCame && !GoToSupport)
        {
            if (!popped)
            {
                time += Time.deltaTime;

                if (time >= 10.0f)
                {
                    popped = true;
                    PopupController.Instance.ShowSmallPopup("Connection to server failed",
                    new SmallPopupButton("Try Again", () => { popped = false; time = 0.0f; }),
                    new SmallPopupButton("Contact Support", () => {
                        Instance.StopTutorial();
                        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TutoStep, (int)Enums.TutorialProgress.Finished);
                        PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
                        GoToSupport = true;
                    }));
                }
            }
            yield return null;
        }

        WebSocketKit.Instance.AckEvents[RequestId.CollectTimelyBonus] -= i => { eventCame = true; };
        if (!GoToSupport)
        {
            yield return new WaitForSeconds(4.0f);
            Instance.tutorialView.ShowInstruction(true);
        }
    }

    private static IEnumerator GlowCashinToggle()
    {
        QuickAccessBarWidget bar = null;
        yield return new WaitUntil(() => { return (bar = FindObjectOfType<QuickAccessBarWidget>()) != null; });
        bar.SetGlowingAnim(bar.MoreCurrencyToogle, true);
    }

    private static IEnumerator GlowLobbyToggle()
    {
        QuickAccessBarWidget bar = null;
        yield return new WaitUntil(() => { bar = FindObjectOfType<QuickAccessBarWidget>(); return bar != null; });
        bar.SetGlowingAnim(bar.LobbyToogle, true);
    }

    private static IEnumerator GlowStoreToggle()
    {
        QuickAccessBarWidget bar = null;
        yield return new WaitUntil(() => { bar = FindObjectOfType<QuickAccessBarWidget>(); return bar != null; });
        bar.SetGlowingAnim(bar.StoreToogle, true);
    }

    private static IEnumerator ScrollToDepositItems()
    {
        DepositWidget depositWidget = null;
        yield return new WaitUntil(() => { return (depositWidget = FindObjectOfType<DepositWidget>()) != null; });
        yield return new WaitUntil(() => { return depositWidget.DepositAmountPool.transform.childCount > 4; });
        for (int x = 0; x < depositWidget.DepositAmountPool.transform.childCount; ++x)
            depositWidget.DepositAmountPool.transform.GetChild(x).gameObject.SetActive(x < 4);
    }


    private static IEnumerator ScrollToBonusButton()
    {
        EarnMoreBonusWidget bonus = null;
        yield return new WaitUntil(() => { return (bonus = FindObjectOfType<EarnMoreBonusWidget>()) != null; });
        yield return new WaitUntil(() => { return bonus.button.interactable; });
        bonus.ScrollToPosition();
    }

    private static IEnumerator WaitForStoreItemsInstalled()
    {
        GeneralStore store = null;
        yield return new WaitUntil(() => { return (store = FindObjectOfType<GeneralStore>()) != null; });
        GeneralStoreItemView item = null;
        yield return new WaitUntil(() => { item = store.content.transform.GetComponentInChildren<GeneralStoreItemView>(); return item != null && item.Id == "7"; });
        yield return null;
    }
    #endregion

#if UNITY_ANDROID || UNITY_IOS
    public static Dictionary<Enums.TutorialProgress, UnityAction> NextActionPool = new Dictionary<Enums.TutorialProgress, UnityAction>()
    {
        { Enums.TutorialProgress.Deposit_Bonus, () => { FindObjectOfType<EarnMoreBonusWidget>().CollectPrice(); } },
        { Enums.TutorialProgress.Store_Redeem, () => { UserController.Instance.SendPurchaseToServer(new Dictionary<string, object>(){{PassableVariable.ItemId.ToString(), "7"}, {PassableVariable.PurchaseMethod.ToString(), "Loyalty"}}); }},
        { Enums.TutorialProgress.Home_Play, () => { FindObjectOfType<PlayWidget>().CurrentTargetCategory.Play(); } }
    };

    public static Dictionary<Enums.TutorialProgress, Func<Transform>> GetTargetPool = new Dictionary<Enums.TutorialProgress, Func<Transform>>()
    {
        { Enums.TutorialProgress.Home_Wallet, () => { return FindObjectOfType<MobileHeader>().transform; } },
        { Enums.TutorialProgress.Home_Bet, () => { return FindObjectOfType<PlayWidget>().CurrentTargetCategory.transform; } },
        { Enums.TutorialProgress.Home_Cashin, () => { return FindObjectOfType<QuickAccessBarWidget>().MoreCurrencyToogle.transform; } },
        { Enums.TutorialProgress.Deposit_Welcome, () => { return FindObjectOfType<DepositWidget>().DepositPanel; } },
        { Enums.TutorialProgress.Deposit_Bonus, () => { return FindObjectOfType<EarnMoreBonusWidget>().transform; } },
        { Enums.TutorialProgress.Deposit_Store, () => { return FindObjectOfType<QuickAccessBarWidget>().StoreToogle.transform; } },
        { Enums.TutorialProgress.Store_Redeem, () => { return FindObjectOfType<GeneralStore>().content.transform.GetChild(0); } },
        { Enums.TutorialProgress.Store_Home, () => { return FindObjectOfType<QuickAccessBarWidget>().LobbyToogle.transform; } },
        { Enums.TutorialProgress.Home_Play, () => { return FindObjectOfType<PlayWidget>().CurrentTargetCategory.PlayButton; } },
    };

    public static Dictionary<Enums.TutorialProgress, Func<IEnumerator>> ConditionForNextPool = new Dictionary<Enums.TutorialProgress, Func<IEnumerator>>()
    {
        { Enums.TutorialProgress.Store_Redeem, WaitLoyaltyChange },
        { Enums.TutorialProgress.Deposit_Bonus, WaitParticulesBonus },
    };

    public static Dictionary<Enums.TutorialProgress, Func<IEnumerator>> ConditionToStartPool = new Dictionary<Enums.TutorialProgress, Func<IEnumerator>>()
    {
        { Enums.TutorialProgress.Home_Cashin, GlowCashinToggle },
        { Enums.TutorialProgress.Deposit_Welcome, ScrollToDepositItems },
        { Enums.TutorialProgress.Deposit_Bonus, ScrollToBonusButton },
        { Enums.TutorialProgress.Deposit_Store, GlowStoreToggle },
        { Enums.TutorialProgress.Store_Redeem, WaitForStoreItemsInstalled },
        { Enums.TutorialProgress.Store_Home, GlowLobbyToggle },
    };
#else
    public static Dictionary<Enums.TutorialProgress, UnityAction> NextActionPool = new Dictionary<Enums.TutorialProgress, UnityAction>()
    {
        { Enums.TutorialProgress.Deposit_Bonus, () => { FindObjectOfType<EarnMoreBonusWidget>().CollectPrice(); } },
        { Enums.TutorialProgress.Store_Redeem, () => { UserController.Instance.SendPurchaseToServer(new Dictionary<string, object>(){{PassableVariable.ItemId.ToString(), "7"}, {PassableVariable.PurchaseMethod.ToString(), "Loyalty"}}); }},
        { Enums.TutorialProgress.Home_Play, () => { FindObjectOfType<PlayWidget>().CurrentTargetCategory.Play(); } }
    };

    public static Dictionary<Enums.TutorialProgress, Func<Transform>> GetTargetPool = new Dictionary<Enums.TutorialProgress, Func<Transform>>()
    {
        { Enums.TutorialProgress.Home_Bet, () => { return FindObjectOfType<PlayWidget>().CurrentTargetCategory.transform; } },
        { Enums.TutorialProgress.Home_Wallet, () => { return FindObjectOfType<PCHeaderWidget>().CashAmountText.transform.parent.parent; }},
        { Enums.TutorialProgress.Home_Cashin, () => { return FindObjectOfType<PCHeaderWidget>().CashAmountText.transform; }},
        { Enums.TutorialProgress.Deposit_Welcome, () => { return FindObjectOfType<DepositWidget>().DepositAmountPool.transform.parent; }},
        { Enums.TutorialProgress.Deposit_Bonus, () => { return FindObjectOfType<EarnMoreBonusWidget>().button.transform; }},
        { Enums.TutorialProgress.Deposit_Store, () => { return FindObjectOfType<PCNavigation>().Store.transform; }},
        { Enums.TutorialProgress.Store_Redeem, () => { return FindObjectOfType<GeneralStore>().content.transform.GetChild(0); }},
        { Enums.TutorialProgress.Store_Home, () => { return FindObjectOfType<PCNavigation>().Home.transform; }},
        { Enums.TutorialProgress.Home_Play, () => { return FindObjectOfType<PCPlayWidget>().CurrentTargetCategory.PlayButton; }}
    };

    public static Dictionary<Enums.TutorialProgress, Func<IEnumerator>> ConditionForNextPool = new Dictionary<Enums.TutorialProgress, Func<IEnumerator>>()
    {
        { Enums.TutorialProgress.Deposit_Bonus, WaitParticulesBonus },
        { Enums.TutorialProgress.Store_Redeem, WaitLoyaltyChange },
    };

    public static Dictionary<Enums.TutorialProgress, Func<IEnumerator>> ConditionToStartPool = new Dictionary<Enums.TutorialProgress, Func<IEnumerator>>()
    {
    };
#endif

}
