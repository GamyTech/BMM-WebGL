using UnityEngine;
using UnityEngine.UI;
using GT.Websocket;
using System.Collections;
using GT.User;

public class SupportMessageWidget : Widget
{
    private string subject;

    public SmoothResize resize;
    public SmoothResize errorResize;
    public Text errorText;
    public Text PickerSubject;
    public InputField SupportInputText;
    public InputField FirstNameInputText;
    public InputField LastNameInputText;
    public InputField PhoneInputText;
    public Toggle SubjectToggle;

    public int minimumMessageLength = 20;

    public override void EnableWidget()
    {
        base.EnableWidget();

        WebSocketKit.Instance.AckEvents[RequestId.ContactGamytech] += OnContactSupport;
        errorResize.SetHeightState(0);
        SubjectToggle.isOn = true;
        SetMessageInput(true);
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.ContactGamytech] -= OnContactSupport;

        base.DisableWidget();
    }

    private void ChangeSubject(string newSubject)
    {
        PickerSubject.text = Utils.LocalizeTerm(newSubject);
        subject = newSubject;
        SubjectToggle.isOn = false;
        SetErrorText();

#if UNITY_STANDALONE
        StartCoroutine(WaitOneFrameToSelectMessageField());
#endif
    }

    private void SetMessageInput(bool isOn)
    {
        SupportInputText.gameObject.SetActive(!isOn);
        resize.targetHeight = isOn ? (resize.transform.GetChild(0).transform as RectTransform).sizeDelta.y : 0;
    }

    private void SetErrorText(string error = null)
    {
        Debug.Log("SetErrorText: " + error);
        if (string.IsNullOrEmpty(error))
            errorResize.SetHeightState(0);
        else
        {
            errorResize.SetHeightState(1, 20);
            errorText.text = Utils.LocalizeTerm(error);
        }
    }

    private bool CheckAll()
    {
        string error = null;
        if (string.IsNullOrEmpty(subject))
            SetErrorText("Please select subject");

        else if(!Utils.IsStringValid(SupportInputText.text, "message", out error, minLength: minimumMessageLength))
            SetErrorText(error);

        else if(!Utils.IsPhoneValid(PhoneInputText.text, out error))
            SetErrorText(error);

        else if (!Utils.IsStringValid(FirstNameInputText.text, "fisrt name", out error))
            SetErrorText(error);

        else if (!Utils.IsStringValid(LastNameInputText.text, "last name", out error))
            SetErrorText(error);

        else
            return true;

        return false;
    }

    #region Inputs
    public void OnOpenToggle(bool isOn)
    {
        SetMessageInput(isOn);
    }

    public void MessageTextChanged(string s)
    {
        string error = null;
        Utils.IsStringValid(s, "message", out error, minLength: minimumMessageLength);
        SetErrorText(error);
    }

    public void TelephoneEndEdit(string s)
    {
        Debug.Log("TelephoneEndEdit " + s);
        string error = null;
        Utils.IsPhoneValid(s, out error);
        SetErrorText(error);
    }

    public void FirstNameEndEdit(string s)
    {
        Debug.Log("FirstNameEndEdit " + s);
        string error = null;
        Utils.IsStringValid(s, "first Name", out error);
        SetErrorText(error);
    }

    public void LastNameEndEdit(string s)
    {
        Debug.Log("LastNameEndEdit " + s);
        string error = null;
        Utils.IsStringValid(s, "last name", out error);
        SetErrorText(error);
    }

    public void ReportBugButton()
    {
        ChangeSubject("Report Bug");
    }

    public void AccountInquiryButton()
    {
        ChangeSubject("Account inquiry");
    }

    public void BillingInquiryButton()
    {
        ChangeSubject("Billiny Inquiry");
    }

    public void ContactUsButton()
    {
        ChangeSubject("Contact Us");
    }

    public void ReportAbuseButton()
    {
        ChangeSubject("Report Abuse");
    }

    public void SubmitButton()
    {
        if (CheckAll())
        {
            LoadingController.Instance.ShowPageLoading(transform.GetChild(0).transform as RectTransform, null);
            UserController.Instance.SendContactSupport(subject, SupportInputText.text, PhoneInputText.text, FirstNameInputText.text, LastNameInputText.text); 
        }
    }

    #endregion Inputs

    private IEnumerator WaitOneFrameToSelectMessageField()
    {
        yield return null;
        SupportInputText.Select();
    }

    #region Events
    private void OnContactSupport(Ack ack)
    {
        LoadingController.Instance.HidePageLoading();
        switch (ack.Code)
        {
            case WSResponseCode.OK:
                PopupController.Instance.ShowSmallPopup("Support Message", new string[] { "Message sent" });
                PageController.Instance.ChangePage(Enums.PageId.Home);
                break;
            default:
                SetErrorText("Error, Try Again");
                break;
        }
    }
    #endregion Events
}
