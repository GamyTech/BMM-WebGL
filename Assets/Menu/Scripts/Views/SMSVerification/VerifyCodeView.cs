using UnityEngine;
using UnityEngine.UI;

public class VerifyCodeView : MonoBehaviour
{
    public delegate void GoBack();
    public GoBack OnGoBack = () => { };

    public Text PhoneNumber;
    public InputField Code;
    public Button VerifyButton;

    public void SetPhoneNumber(string number)
    {
        PhoneNumber.text = number;

        Code.onValueChanged.AddListener(ChangeButtonState);
        VerifyButton.interactable = false;

        Code.keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
    }

    private void ChangeButtonState(string value)
    {
        VerifyButton.interactable = value.Length > 0;
    }

    private void BackToPhoneInput()
    {
        OnGoBack();
    }

    private void ResendSMS()
    {
        UserController.Instance.GetSmsValidation(Utils.StripPhoneNumber(PhoneNumber.text));
    }

    #region Input
    public void DidntReceive()
    {
        PopupController.Instance.ShowSmallPopup("Didn't receive SMS?",
            new string[] { "SMS can take a few minutes to deliver" }, 2,
            new SmallPopupButton("Re-send SMS", ResendSMS),
            new SmallPopupButton("Edit phone number", BackToPhoneInput),
            new SmallPopupButton("Cancel"));
    }

    public void Verify()
    {
        UserController.Instance.ValidateSmsByUser(Code.text);
    }
    #endregion Input
}
