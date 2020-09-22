using UnityEngine;
using UnityEngine.UI;

public class DepositItemView : MonoBehaviour
{
    public Sprite PopularPanelRef;
    public Sprite RegularPanelRef;
    public Image PopularImage;
    public Text SavingsText;
    public Text AmountText;
    public Text BonusText;
    public Button button;
    public bool OverrideFontSize = true;

    public void PopulateItem(DepositAmount depositAmount)
    {

#if UNITY_STANDALONE || UNITY_WEBGL
        if (OverrideFontSize)
        {
            SavingsText.fontSize = 20;
            AmountText.fontSize = 50;
            BonusText.fontSize = 28;
        }
#endif

        AmountText.text = Wallet.CashPostfix + depositAmount.amount.ToString();
        SetBonus(depositAmount);
        SetPopular(depositAmount.popular);
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => DepositClick(depositAmount));
        }
    }

    public void SetPopular(bool isPopular)
    {
        PopularImage.sprite = isPopular ? RegularPanelRef : PopularPanelRef;
        if (isPopular)
        {
            SavingsText.text = Utils.LocalizeTerm("Most Popular").ToUpper();
#if UNITY_STANDALONE || UNITY_WEBGL
            if (OverrideFontSize)
                SavingsText.fontSize = 18;
#endif
        }
    }

    private void SetBonus(DepositAmount depositAmount)
    {
        SavingsText.text = depositAmount.savingsPercent.ToString("###0.##") + Wallet.PercentPostfix + " " + Utils.LocalizeTerm("More").ToLower();
        BonusText.text = "+ " + Wallet.CashPostfix + depositAmount.bonusCash.ToString("###0.##") + " " + Utils.LocalizeTerm("Bonus").ToUpper();
    }

    #region Button
    private void DepositClick(DepositAmount depositAmount)
    {
        Enums.PageId currPage = PageController.Instance.CurrentPageId;
        Debug.Log("DepositClick " + currPage);

        GT.User.GTUser user = UserController.Instance.gtUser;
        user.cashInData.depositAmount = depositAmount;
        //if (user.MustBeVerified || (user.AccumulatedDeposit + depositAmount.amount) >= user.MaxCashinForStage)
        //    UserController.Instance.GetVerification(user.VerificationRank + 1);
        //else
        if (!UserController.Instance.CheckAndShowUserVerification())
            PageController.Instance.CustomBackChangePage(Enums.PageId.DepositInfo, currPage);

    }
    #endregion Button
}
