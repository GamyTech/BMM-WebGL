using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GT.Websocket;
using UnityEngine.Events;

public class PaypalConnectingWidget : Widget
{
    public Image loadingImage;
    private int m_failedUpdateCount = 0;
    public GameObject IssuePanel;

    public override void EnableWidget()
    {
#if UNITY_STANDALONE
        IssuePanel.SetActive(false);
#endif
        base.EnableWidget();

        if (UserController.Instance != null && UserController.Instance.wallet != null)
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.Wallet, OnUpdateCash);
    }

    public override void DisableWidget()
    {
        if (UserController.Instance != null && UserController.Instance.wallet != null)
            WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.Wallet, OnUpdateCash); 

        StopAllCoroutines();
        UserController.Instance.GetUserVarsFromServer(APIGetVariable.Verification);

        base.DisableWidget();
    }

    void Update()
    {
        loadingImage.transform.Rotate(0.0f, 0.0f, 10.0f);
    }

    void OnUpdateCash(object o)
    {
        //Dictionary<string, object> cashDict = (Dictionary<string, object>)o;
        //if (cashDict == null) return;

        //float newtotalcash = 0.0f;
        //object cashObject;
        //if (cashDict.TryGetValue("Cash", out cashObject))
        //    newtotalcash = cashObject.ParseFloat();

        //if (cashDict.TryGetValue("BonusCash", out cashObject))
        //    newtotalcash += cashObject.ParseFloat();

        //if (UserController.Instance.gtUser.cashInData.lastBalance != newtotalcash)
        //    PageController.Instance.ChangePage(Enums.PageId.DepositSuccess);

        //else
        //    OnUpdateFailed();

    }

    void OnUpdateFailed()
    {
        PopupController.Instance.ShowSmallPopup("Balance Did Not Update", new string[] { "If you completed your transaction, Wait another minute and click Update Balance." }, new SmallPopupButton("OK"));

        ++m_failedUpdateCount;

        if (m_failedUpdateCount >= 2)
            PageController.Instance.ChangePage(Enums.PageId.PCPayPalSupport);
    }

    #region Buttons
    public void UpdateButton()
    {
        UserController.Instance.GetUserVarsFromServer(APIGetVariable.CashData);
    }

    public void BackButton()
    {
        PageController.Instance.BackPage();
    }

    public void HelpButton()
    {
        UnityAction GetEmailAction = () => { WebSocketKit.Instance.SendRequest(RequestId.SendCashinInstructions); PageController.Instance.ChangePage(Enums.PageId.Home); };
        UnityAction ContactSupportAction = () => { PageController.Instance.ChangePage(Enums.PageId.ContactSupport); };

        SmallPopupButton[] butons = new SmallPopupButton[3]
        {
            new SmallPopupButton("Get help via email", GetEmailAction),
            new SmallPopupButton("Contact Support", ContactSupportAction),
            new SmallPopupButton("Cancel")
        };

        string[] text = new string[1] { "If you are experiencing any issues with the credit card verification page while attempting to deposit funds into your account, please download the desktop version of backgammon4money on your PC/MAC and try to deposit funds on that version." };
        PopupController.Instance.ShowSmallPopup("Deposit Help", text, butons);
    }

    #endregion Buttons

}
