using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using GT.Websocket;

public class DepositWidget : Widget
{

    public GameObject PromoPanelObject;
    public GameObject EnterPromoCodeObject;
    public SmoothResize promoCodeInputResizer;
    public SmoothResize errorResizer;
    public Text ErrorText;
    public InputField PromoCodeInputField;
    public Transform DepositPanel;
    public GridLayoutGroup Grid;

    private List<DepositItemView> depositItems = new List<DepositItemView>();
    private string tempPromoCode;

    public SpecialOfferView specialOfferView;
    public ObjectPool DepositAmountPool;

    public override void EnableWidget()
    {
        base.EnableWidget();

#if UNITY_STANDALONE || UNITY_WEBGL
        Grid.constraintCount = 3;
        Grid.cellSize = new Vector2(240.0f, 162.0f);
#endif

        UserController.Instance.gtUser.OnDepositInfoChanged += GtUser_OnDepositInfoChanged;

        tempPromoCode = string.Empty;
        UserController.Instance.gtUser.cashInData = new CashInData();

        APIGetVariable varsToUpdate = APIGetVariable.None;

        if (UserController.Instance.gtUser.DepositInfo == null ||
            UserController.Instance.gtUser.DepositInfo.DepositAmounts == null)
        {
            LoadingController.Instance.ShowPageLoading();
            varsToUpdate |= APIGetVariable.DepositInfo;
            UserController.Instance.gtUser.GetDepositInfoFromPlayErPrefs();
        }
        if (UserController.Instance.gtUser.TimelyBonusData == null)
            varsToUpdate |= APIGetVariable.TimelyBonus;
        if (UserController.Instance.gtUser.savedCreditCards == null)
            varsToUpdate |= APIGetVariable.GetCards;

        if (varsToUpdate != APIGetVariable.None)
            UserController.Instance.GetUserVarsFromServer(varsToUpdate);

        if (UserController.Instance.gtUser.DepositInfo != null &&
             UserController.Instance.gtUser.DepositInfo.DepositAmounts != null)
            SetData(UserController.Instance.gtUser.DepositInfo);

        PromoPanelObject.SetActive(false);
        EnterPromoCodeObject.SetActive(false);
    }

    public override void DisableWidget()
    {
        UserController.Instance.gtUser.OnDepositInfoChanged -= GtUser_OnDepositInfoChanged;
        base.DisableWidget();
    }

#region Aid Functions
    private void SetData(DepositData data)
    {
        if (data == null)
            return;

        SetDepositAmounts(data.DepositAmounts);
        SetSpecialOffer(data.SpecialOffer);
    }

    private void SetSpecialOffer(SpecialDepositOffer offer)
    {
        if (offer == null)
            return;
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.SpecialOfferID, offer.Id);
        NotificationSystemController.Instance.ForceUpdate(Enums.NotificationId.Cashin);
        specialOfferView.resizer.SetHeightState(1);
        specialOfferView.PopulateSpecialOffer(offer);
    }

    private void SetDepositAmounts(List<DepositAmount> amounts)
    {
        ClearAmountDataList();
        if (amounts == null || amounts.Count == 0)
        {
            Debug.LogError("Deposit Amounts is null or empty");
            return;
        }

        for (int i = 0; i < amounts.Count; i++)
        {
            GameObject go = DepositAmountPool.GetObjectFromPool();
            go.InitGameObjectAfterInstantiation(DepositAmountPool.transform);
            DepositItemView depositItem = go.GetComponent<DepositItemView>();
            depositItems.Add(depositItem);
            depositItem.PopulateItem(amounts[i]);
        }
    }

    private void SetErrorText(string error, bool verified = false)
    {
        Debug.Log("SetErrorText: " + error);
        if (string.IsNullOrEmpty(error))
            errorResizer.SetHeightState(0);
        else
        {
            errorResizer.SetHeightState(1, 20);
            ErrorText.color = verified ? Color.green : Color.red;
            ErrorText.text = Utils.LocalizeTerm(error);
        }
    }

    private bool VerifyPromoCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            SetErrorText("Please enter promo code");
        else if (code.Length < 1)
            SetErrorText("Promo code too short");
        else if (code.Length > 20)
            SetErrorText("Promo code too long");
        else
            return true;

        return false;
    }

    private void SendPromoCode()
    {
        LoadingController.Instance.ShowPageLoading(PromoPanelObject.transform as RectTransform);
        //UserController.Instance.ValidatePromoCode(tempPromoCode);
        SetErrorText("Invalid Promo Code");
    }
#endregion Aid Functions

#region Deposit Items List Functions
    private void ClearAmountDataList()
    {
        for (int i = 0; i < depositItems.Count; i++)
            DepositAmountPool.PoolObject(depositItems[i].gameObject);
        depositItems.Clear();
    }
#endregion Deposit Items List Functions

#region Buttons
    public void EnterPromoCode()
    {
        EnterPromoCodeObject.SetActive(false);
        PromoPanelObject.SetActive(true);
        promoCodeInputResizer.ResetHeight();
        promoCodeInputResizer.SetHeightState(1, 20);
    }

    public void VerifyPromoCode()
    {
        if(string.IsNullOrEmpty(tempPromoCode) == false)
            SendPromoCode();
        else
            SetErrorText("Please enter promo code");
    }

    public void PromoCodeEndEdit(string str)
    {
        if(VerifyPromoCode(str))
            tempPromoCode = str;
    }
#endregion Buttons

#region Events
    private void GtUser_OnDepositInfoChanged(DepositData newValue)
    {
        LoadingController.Instance.HidePageLoading();
        SetData(newValue);

        EarnMoreBonusWidget bonusWidget = FindObjectOfType<EarnMoreBonusWidget>();
        if (bonusWidget != null && bonusWidget.button.IsInteractable())
            bonusWidget.ScrollToPosition();
    }
    #endregion Events
}
