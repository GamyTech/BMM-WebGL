using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class BalanceWidget : Widget
{
    public GameObject CashContent;
    public Text CashAvailable;
    public Text CashBonus;
    public Text cashInPlay;
    public Text CashTotal;

    public GameObject CoinsContent;
    public Text CoinsAvailable;
    public Text CoinsInPlay;
    public Text CoinsTotal;

    public Text Loyalties;

    public override void EnableWidget()
    {
        base.EnableWidget();
        switch(AppInformation.MATCH_KIND)
        {
            case Enums.MatchKind.Cash:
                InitCashFields();
                CoinsContent.SetActive(false);
                break;
            case Enums.MatchKind.Virtual:
                InitVirtualCoinsFields();
                CashContent.SetActive(false);
                break;
        }

        Loyalties.text = Wallet.LoyaltyPointsPostfix + UserController.Instance.wallet.LoyaltyPoints.ToString("#,##0");
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
    }

    private void InitCashFields()
    {

            float available = UserController.Instance.wallet.Cash;
            float bonus = UserController.Instance.wallet.BonusCash;
            float inPlay = 0; //UserController.Instance.wallet.VirtualCurrencyInPlay;
            float total = available + bonus + inPlay;


            CashAvailable.text = Wallet.CashPostfix + available.ToString("#,##0.00");
            CashBonus.text = Wallet.CashPostfix + bonus.ToString("#,##0.00");
            cashInPlay.text = Wallet.CashPostfix + inPlay.ToString("#,##0.00");
            CashTotal.text = Wallet.CashPostfix + total.ToString("#,##0.00");
    }

    private void InitVirtualCoinsFields()
    {
        float available = UserController.Instance.wallet.VirtualCoins;
        float inPlay = 0; //UserController.Instance.wallet.VirtualCurrencyInPlay;
        float total = available + inPlay;

        string postFix = Wallet.VirtualPostfix;
        if (AppInformation.GAME_ID == Enums.GameID.BackgammonBlockChain)
        {
            postFix = Wallet.GPTPostfix;
            Wallet.SetGradientForMatchKind(Enums.MatchKind.Cash, CoinsTotal.GetComponent<UnityEngine.UI.Gradient>());
        }
        CoinsAvailable.text = postFix + available.ToString("#,##0");
        CoinsInPlay.text = postFix + inPlay.ToString("#,##0");
        CoinsTotal.text = postFix + total.ToString("#,##0");
    }


}
