using System.Collections;
using SP.Dto.ProcessBreezeRequests;
using UnityEngine;
using UnityEngine.UI;

public class PhoneInputView : MonoBehaviour
{
    public delegate void WaitForCode(ISO3166Country country, int codeId, string number);
    public WaitForCode OnWaitForCode = (i, j, k) => { };

    public Text LoyaltyCount;
    public Text CountryText;
    public Image CorrectIcon;
    public InputField PhoneInput;
    public Button ValidateButton;

    public SelectCountryCodeView CountrySelect;

    private ISO3166Country selectedCountry;
    private int countryCode;
    
    private void OnEnable()
    {
        CorrectIcon.enabled = false;
        LoyaltyCount.text = string.Format(LoyaltyCount.text, Wallet.AmountToString(UserController.Instance.PhoneValidationBonus));

        PhoneInput.onEndEdit.AddListener(CheckPhoneNumber);
        PhoneInput.onValueChanged.AddListener(CheckPhoneNumber);
        PhoneInput.keyboardType = TouchScreenKeyboardType.PhonePad;
        string phone = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.SMSSentPhoneNumber);
        if (!string.IsNullOrEmpty(phone))
            PhoneInput.text = phone;

        SetSelectedCountry(FindCurrentCountry(), 0);

        CountrySelect.OnSelect += SetSelectedCountry;

        ValidateButton.interactable = false;
    }

    private void OnDisable()
    {
        CountrySelect.OnSelect -= SetSelectedCountry;
        PhoneInput.onEndEdit.RemoveListener(CheckPhoneNumber);
        PhoneInput.onValueChanged.RemoveListener(CheckPhoneNumber);
    }

    public void SetSelected(ISO3166Country country, int countryCode, string number)
    {
        SetSelectedCountry(country, countryCode);
        PhoneInput.text = number;
        CheckPhoneNumber(number);
    }

    private ISO3166Country FindCurrentCountry()
    {
        string savedAlpha3 = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.SMSCountryISO);
        IEnumerator ie = ISO3166.GetCollection().GetEnumerator();
        while (ie.MoveNext())
        {
            ISO3166Country item = (ISO3166Country)ie.Current;
            if ((!string.IsNullOrEmpty(savedAlpha3) && savedAlpha3 == item.Alpha3) || 
                (string.IsNullOrEmpty(savedAlpha3) && item.Alpha3 == UserController.Instance.gtUser.CountryISO))
            {
                ie.Reset();
                return item;
            }
        }
        ie.Reset();
        ie.MoveNext();
        ISO3166Country defaultItem = (ISO3166Country)ie.Current;
        ie.Reset();
        return defaultItem;
    }

    #region Events 
    private void CheckPhoneNumber(string input)
    {
        string error;
        bool isValid = Utils.IsPhoneValid(input, out error);
        CorrectIcon.enabled = isValid;
        ValidateButton.interactable = isValid;
    }

    private void SetSelectedCountry(ISO3166Country country, int codeId)
    {
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.SMSCountryISO, country.Alpha3);
        selectedCountry = country;
        countryCode = codeId;
        CountryText.text = CountryCodeListItem.GetCountryCodeName(country, codeId);
        CountrySelect.gameObject.SetActive(false);
    }
    #endregion Events

    #region Input
    public void SelectCountryCode()
    {
        CountrySelect.gameObject.SetActive(true);
    }

    public void Verify()
    {
        UserController.Instance.GetSmsValidation(selectedCountry.DialCodes[countryCode] + Utils.StripPhoneNumber(PhoneInput.text));
        OnWaitForCode(selectedCountry, countryCode, Utils.StripPhoneNumber(PhoneInput.text));
    }
    #endregion Input
}
