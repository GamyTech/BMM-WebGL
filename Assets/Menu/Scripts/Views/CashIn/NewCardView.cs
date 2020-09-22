using UnityEngine;
using UnityEngine.UI;
using GT.PayPal;
using System.Text;

public class NewCardView : MonoBehaviour
{
    public delegate void FormValidated(bool isValid);
    public FormValidated OnFormValidated = (i) => { };

    public Text AmountText;
    public GameObject CardScannerPanel;
    public Image CardTypeImage;
    public InputField CardNumInputField;
    public InputField ExpirationInputField;
    public InputField SecurityInputField;
    public InputField NameInputField;

    public Color RightValueColor;
    public Color InputColor;
    public Color WrongValueColor;

    private bool ignoreInputFieldChanged = false;
    private NewCreditCard newCardData = new NewCreditCard();

    private void Start()
    {
        CardScannerPanel.SetActive(PayPalKit.IsScannerSupported());

        CardNumInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        ExpirationInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        SecurityInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        NameInputField.keyboardType = TouchScreenKeyboardType.NamePhonePad;
        
        AmountText.text = Wallet.CashPostfix + UserController.Instance.gtUser.cashInData.depositAmount.amount.ToString();
    }

    public void ResetCard()
    {
        CardNumInputField.text = string.Empty;
        ExpirationInputField.text = string.Empty;
        SecurityInputField.text = string.Empty;
        NameInputField.text = string.Empty;
        CardTypeImage.sprite = null;
        CardTypeImage.color = new Color(255, 255, 255, 0);
        newCardData.Reset();
        OnFormValidated(false);
    }

    public NewCreditCard GetCardData()
    {
        return newCardData;
    }

    private void SetCardType(string num)
    {
        newCardData.setCardType(num);
        CardNumInputField.characterLimit = newCardData.DigitLenghtRequired + Mathf.CeilToInt((float)(newCardData.DigitLenghtRequired) / 4f) - 1;
        SecurityInputField.characterLimit = newCardData.CVVLenghtRequired;

        Sprite sprite = null;
        AssetController.Instance.CreditCard.TryGetValue(newCardData.CardType, out sprite);
        CardTypeImage.sprite = sprite;
        CardTypeImage.color = sprite != null ? Color.white : new Color(255, 255, 255, 0);
    }

    private bool isAllDoneAndValid()
    {
        return newCardData.IsValidCard(); 
    }

    private void SelectNexField(InputField input)
    {
        if (input.text == "")
            input.ActivateInputField();
    }

    #region Buttons
    public void ScanCreditCardButton()
    {
        PayPalKit.OpenScanner(ScannerCallback);
    }

    public void OnInfoButton()
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.CvvHintPopup, TextAnchor.UpperCenter, false);
    }
    #endregion Buttons

    #region Inputs
    public void CardNumberInputFieldEndEdit(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;

        string card = str.Replace(" ", string.Empty);
        string error = null;
        if (newCardData.SetCardNumbers(card, out error))
        {
            SelectNexField(ExpirationInputField);
            CardNumInputField.textComponent.color = RightValueColor;
        }
        else
            CardNumInputField.textComponent.color = WrongValueColor;
        
        OnFormValidated(isAllDoneAndValid());
    }

    public void ExpirationDateInputFieldEndEdit(string str)
    {
        string error = null;
        if (newCardData.SetExpirationDate(str, out error))
        {
            SelectNexField(SecurityInputField);
            ExpirationInputField.textComponent.color = RightValueColor;
        }
        else
            ExpirationInputField.textComponent.color = WrongValueColor;
        OnFormValidated(isAllDoneAndValid());
    }

    public void SecurityCodeInputFieldEndEdit(string str)
    {
        string error = null;
        if (newCardData.SetSecurityCode(str, out error))
        {
            SelectNexField(NameInputField);
            SecurityInputField.textComponent.color = RightValueColor;
        }
        else
            SecurityInputField.textComponent.color = WrongValueColor;
        OnFormValidated(isAllDoneAndValid());
    }

    public void NameInputFieldEndEdit(string str)
    {
        if (newCardData.SetOwnerName(str))
            NameInputField.textComponent.color = RightValueColor;
        else
            NameInputField.textComponent.color = WrongValueColor;

        OnFormValidated(isAllDoneAndValid());
    }


    public void CardNumberInputFieldChanged(string str)
    {
        if (ignoreInputFieldChanged)
        {
            ignoreInputFieldChanged = false;
            CardNumInputField.MoveTextEnd(false);
            return;
        }

        string numbers = str.Replace(" ", string.Empty);
        string cardStr = NewCreditCard.CardNumToString(numbers);
        if (!str.Equals(cardStr))
        {
            ignoreInputFieldChanged = true;
            CardNumInputField.text = cardStr;
        }
        
        CardNumInputField.textComponent.color = InputColor;
        SetCardType(numbers);

        OnFormValidated(isAllDoneAndValid());
    }

    public void SecurityCodeInputFieldChanged(string str)
    {
        if (ignoreInputFieldChanged)
        {
            ignoreInputFieldChanged = false;
            SecurityInputField.MoveTextEnd(false);
            return;
        }
        SecurityInputField.textComponent.color = InputColor;

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            int num;
            if (int.TryParse(str[i].ToString(), out num))
                builder.Append(str[i]);
        }
        if (!SecurityInputField.text.Equals(builder.ToString()))
        {
            ignoreInputFieldChanged = true;
            SecurityInputField.text = builder.ToString();
        }

        OnFormValidated(isAllDoneAndValid());
    }

    public void ExpirationDateInputFieldChanged(string str)
    {
        if (ignoreInputFieldChanged)
        {
            ignoreInputFieldChanged = false;
            ExpirationInputField.MoveTextEnd(false);
            return;
        }
        ExpirationInputField.textComponent.color = InputColor;

        string exp = str.Replace(" ", string.Empty).Replace("/", string.Empty);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < exp.Length; i++)
        {
            int num;
            if (int.TryParse(exp[i].ToString(), out num))
            {
                builder.Append(exp[i]);
                if ((i + 1) >= exp.Length)
                    continue;
                if ((i + 1) == 2)
                    builder.Append('/');
            }
        }
        if (!ExpirationInputField.text.Equals(builder.ToString()))
        {
            ignoreInputFieldChanged = true;
            ExpirationInputField.text = builder.ToString();
        }

        OnFormValidated(isAllDoneAndValid());
    }

    public void NameInputFieldChanged(string str)
    {
        NameInputField.textComponent.color = InputColor;
        newCardData.SetOwnerName(str);
        OnFormValidated(isAllDoneAndValid());
    }
    #endregion Inputs

    #region Scanner Callback
    private void ScannerCallback(ScannerResponse response)
    {
        Debug.Log("ScannerCallback " + response);
        switch (response.responseType)
        {
            case ScannerResponseType.OK:
                if (response.cardNumber != null && response.cardNumber.Length > 4)
                    PasteScanData(response.cardNumber, response.expireMonth, response.expireYear, response.cvv);
                else
                {
                    ResetCard();
                    // card unreadable
                }
                break;
            case ScannerResponseType.Cancel:
                ResetCard();
                // Scan Canceled
                break;
            case ScannerResponseType.NotSupported:
                break;
        }
    }

    private void PasteScanData(string num, string month, string year, string cvv)
    {
        string error = null;

        if (!newCardData.SetCardNumbers(num, out error) || !newCardData.SetExpirationDate(month + year, out error) || !newCardData.SetSecurityCode(cvv, out error))
        {
            return;
        }

        SetCardType(num);

        CardNumInputField.text = num;
        ignoreInputFieldChanged = true;
        ExpirationInputField.text = newCardData.expirationMonth + "/" + newCardData.expirationYear;
        SecurityInputField.text = cvv;
        ignoreInputFieldChanged = false;

        SelectNexField(NameInputField);
    }
    #endregion Scanner Callback
}
