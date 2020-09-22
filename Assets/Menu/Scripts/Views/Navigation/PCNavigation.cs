using GT.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCNavigation : MonoBehaviour
{
    public GameObject Content;
    public Toggle Home;
    public Toggle History;
    public Toggle Cashier;
    public Toggle Balance;
    public Toggle Challenge;
    public Toggle Withdraw;
    public Toggle Store;
    public Toggle Settings;
    public Toggle Support;
    public ToggleGroup toggleGroup;

    private Toggle m_ToggleSelected;
    private Enums.NavigationCategory m_curentCat;
    private bool m_ManualChange = true;

    void Start()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        Content.SetActive(true);
        PageController.OnPageChanged += PageController_OnPageChanged;

        if(AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends)
        {
            Cashier.gameObject.SetActive(false);
            Withdraw.gameObject.SetActive(false);
        }
        else
            Challenge.gameObject.SetActive(false);
#else
        Destroy(this.gameObject);
#endif
    }

    void OnDestroy()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        PageController.OnPageChanged -= PageController_OnPageChanged;
#endif
    }

    private void PageController_OnPageChanged(Page page)
    {
        if(page.PCNav)
            PressCurrentToggle();

        Content.SetActive(page.PCNav);
    }

    private void PressCurrentToggle()
    {
        m_curentCat = PageController.Instance.CurrentPage.NavCat;


        toggleGroup.allowSwitchOff = true;
        if (m_ToggleSelected != null)
            m_ToggleSelected.isOn = false;
            m_ToggleSelected = null;
        toggleGroup.allowSwitchOff = false;

        switch (m_curentCat)
        {
            case Enums.NavigationCategory.Home:
                if(Home.gameObject.activeSelf)
                    m_ToggleSelected = Home;
                break;
            case Enums.NavigationCategory.History:
                if (History.gameObject.activeSelf)
                    m_ToggleSelected = History;
                break;
            case Enums.NavigationCategory.Cashier:
                if (Cashier.gameObject.activeSelf)
                    m_ToggleSelected = Cashier;
                break;
            case Enums.NavigationCategory.Balance:
                if (Balance.gameObject.activeSelf)
                    m_ToggleSelected = Balance;
                break;
            case Enums.NavigationCategory.Challenge:
                if (Challenge.gameObject.activeSelf)
                    m_ToggleSelected = Challenge;
                break;
            case Enums.NavigationCategory.Withdraw:
                if (Withdraw.gameObject.activeSelf)
                    m_ToggleSelected = Withdraw;
                break;
            case Enums.NavigationCategory.Store:
                if (Store.gameObject.activeSelf)
                    m_ToggleSelected = Store;
                break;
            case Enums.NavigationCategory.Settings:
                if (Settings.gameObject.activeSelf)
                    m_ToggleSelected = Settings;
                break;
            case Enums.NavigationCategory.Support:
                if (Support.gameObject.activeSelf)
                    m_ToggleSelected = Support;
                break;
        }

        if(m_ToggleSelected != null)
        {
            m_ManualChange = false;
            m_ToggleSelected.isOn = true;
            m_ManualChange = true;
        }
    }

    #region Toogles

    public void HomeToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.Home);
            m_ToggleSelected = Home;
        }
    }

    public void HistoryToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.PreviewMatchHistory);
            m_ToggleSelected = Home;
        }
    }

    public void CashierToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.Cashin);
            m_ToggleSelected = Cashier;
        }
    }

    public void BallanceToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.Balance);
            m_ToggleSelected = Balance;
        }
    }

    public void ChallengeToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.Challenge);
            m_ToggleSelected = Challenge;
        }
    }

    public void WithdrawToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.Cashout);
            m_ToggleSelected = Withdraw;
        }
    }

    public void StoreToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.LoyaltyStore);
            m_ToggleSelected = Store;
        }
    }

    public void SettingsToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.Settings);
            m_ToggleSelected = Settings;
        }
    }

    public void SupportToggle(bool On)
    {
        if (On && m_ManualChange)
        {
            PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
            m_ToggleSelected = Support;
        }
    }
    #endregion Toogles
}
