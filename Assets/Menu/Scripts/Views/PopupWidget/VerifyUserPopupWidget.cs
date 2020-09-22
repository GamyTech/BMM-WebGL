using GT.Websocket;
using UnityEngine.UI;

public class VerifyUserPopupWidget : PopupWidget
{
    public VerificationFormView userForm;

    #region User parameters
    public InputField Email;    
    private string email = string.Empty;
    #endregion User parameters

    public override void EnableWidget()
    {
        base.EnableWidget();

        WebSocketKit.Instance.AckEvents[RequestId.UpdateVerificationDetails] += OnUpdateVerificationDetails;

        UserVirificationData data = new UserVirificationData();
        UserController.Instance.gtUser.verificationData = data;
        
        userForm.InitFields(data);
        userForm.OnImageReady += UpdateServer;

        Email.text = data.Email;
        email = data.Email;
    }

    public override void DisableWidget()
    {
        userForm.OnImageReady -= UpdateServer;
        base.DisableWidget();
        WebSocketKit.Instance.AckEvents[RequestId.UpdateVerificationDetails] -= OnUpdateVerificationDetails;
    }

    private void OnUpdateVerificationDetails(Ack ack)
    {
        UserController.Instance.gtUser.UpdateVerificationState(ack);
        LoadingController.Instance.HidePageLoading();
        PopupController.Instance.ShowSmallPopup("Account Verification", new string[]
                { "Your account is under a review by the support team, you can continue playing" });
        HidePopup();
    }

    private void UpdateServer()
    {
        UserController.Instance.SendVerification();
    }

    private bool ValidityCheck()
    {
        string error = null;
        int page = userForm.GetCurrentPage();
        if (page == 0)
        {
            if (userForm.ValidityCheckWithoutAddress() &&
                Utils.IsEmailValid(email, "Email", out error))
                return true;
        }
        else if (page == 1)
        {
            if (userForm.FullValidityCheck())
                return true;
        }
        if (!string.IsNullOrEmpty(error))
            userForm.SetErrorText(error);

        return false;
    }

    #region Input
    public void NextPage()
    {
        if (ValidityCheck())
            userForm.NextPage();
    }

    public void EmailEndEdit(string s)
    {
        string error = null;
        if (Utils.IsEmailValid(s, out error))
            email = s;
        else
            email = string.Empty;

        UserController.Instance.gtUser.verificationData.Email = email;
        userForm.SetErrorText(error);
    }
    #endregion Input
}
