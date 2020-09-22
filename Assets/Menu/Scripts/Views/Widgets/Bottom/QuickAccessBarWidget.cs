using GT.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickAccessBarWidget : Widget
{
    public Toggle MoreToogle;
    public Toggle HistoryToogle;
    public Toggle LobbyToogle;
    public Toggle StoreToogle;
    public Toggle MoreCurrencyToogle;
    public ToggleGroup toggleGroup;
    public Text LobbyIcon;

    private Toggle m_ToggleSelected;
    private Enums.NavigationCategory m_curentCat;
    private bool m_ManualChange = true;

    public override void EnableWidget()
    {
        base.EnableWidget();
        PageController.OnPageChanged += PageController_OnPageChanged;
        if (PageController.Instance != null && PageController.Instance.MobileNavigation != null)
            PageController.Instance.MobileNavigation.OnNavOpen += MobileNavigation_OnNavOpen;
        PressCurrentToggle();

        if (AppInformation.GAME_ID == Enums.GameID.BackgammonRoyale)
            LobbyIcon.text = "W";
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
        PageController.OnPageChanged -= PageController_OnPageChanged;
        if (PageController.Instance != null && PageController.Instance.MobileNavigation != null)
            PageController.Instance.MobileNavigation.OnNavOpen -= MobileNavigation_OnNavOpen;
    }

    private void MobileNavigation_OnNavOpen(bool open)
    {
        m_ManualChange = false;
        MoreToogle.isOn = open;
        m_ManualChange = true;
    }

    private void PageController_OnPageChanged(Page page)
    {
        PressCurrentToggle();
    }

    private void PressCurrentToggle()
    {
        m_curentCat = PageController.Instance.CurrentPage.NavCat;
        switch (m_curentCat)
        {
            case Enums.NavigationCategory.History:
                m_ToggleSelected = HistoryToogle;
                break;
            case Enums.NavigationCategory.Home:
                m_ToggleSelected = LobbyToogle;
                break;
            case Enums.NavigationCategory.Store:
                m_ToggleSelected = StoreToogle;
                break;
            case Enums.NavigationCategory.Cashier:
                m_ToggleSelected = MoreCurrencyToogle;
                break;

            default:
                if (m_ToggleSelected != null)
                {
                    toggleGroup.allowSwitchOff = true;
                    m_ToggleSelected.isOn = false;
                    toggleGroup.allowSwitchOff = false;
                }
                m_ToggleSelected = null;
                return;
        }

        m_ManualChange = false;
        m_ToggleSelected.isOn = true;
        m_ManualChange = true;
    }

    #region Toogles

    public void MoreToggleClicked(bool On)
    {
        if (m_ManualChange)
            PageController.Instance.MobileNavigation.TriggerOpen(On);
    }

    public void HistoryToggleClicked(bool On)
    {
        ToggleClickAction(HistoryToogle, Enums.PageId.PreviewMatchHistory, On);
    }

    public void LobbyToggleClicked(bool On)
    {
        ToggleClickAction(LobbyToogle, Enums.PageId.Home, On);
    }

    public void StoreToggleClicked(bool On)
    {
        ToggleClickAction(StoreToogle, Enums.PageId.LoyaltyStore, On);
    }

    public void CashinToggleClicked(bool On)
    {
        Enums.PageId page = AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends ? Enums.PageId.InAppPurchase : Enums.PageId.Cashin;
        ToggleClickAction(MoreCurrencyToogle, page, On);
    }

    public void ToggleClickAction(Toggle toggle, Enums.PageId page, bool on)
    {
        toggle.animator.SetBool("Glow", false);
        if (on && m_ManualChange)
        {
            PageController.Instance.ChangePage(page);
            m_ToggleSelected = toggle;
        }
        if (PageController.Instance != null && PageController.Instance.MobileNavigation != null)
            PageController.Instance.MobileNavigation.CloseMenu();
    }

    public void SetGlowingAnim(Toggle toggle, bool glowing)
    {
        toggle.animator.SetBool("Glow", glowing);
    }
    #endregion Toogles
}
