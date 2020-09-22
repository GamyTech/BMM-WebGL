using UnityEngine;
using UnityEngine.UI;

public class VerificationFormView : MonoBehaviour
{
    public delegate void ImageUploadEvent();
    public ImageUploadEvent OnImageReady = () => {};

    public IndicatorsView IndicatorsView;
    public GameObject[] pages;
    public SmoothResize[] errorPanels;

    public Button SelectButton;
    public Button takeAPictureButton;
    public Button retakePictureButton;
    public Button uploadButton;
    public RectTransform stepTwoPanel;
    public Image pictureImage;
    
    private int currentPage;
    private Text ErrorText;
    private SmoothResize errorResizer;

    #region User parameters
    public InputField FirstName;
    public InputField LastName;
    public InputField Telephone;

    public InputField Street;
    public InputField StreetNumber;
    public InputField Country;
    public InputField City;
    public InputField PostalCode;

    private string firstName = string.Empty;
    private string lastName = string.Empty;
    private string telephone = string.Empty;
    private string street = string.Empty;
    private string streetNumber = string.Empty;
    private string country = string.Empty;
    private string city = string.Empty;
    private string postalCode = string.Empty;
    #endregion User parameters

    private UserInfoData userData;

    void Start()
    {
        if (errorResizer != null)
            errorResizer.ResetHeight();

        IndicatorsView.Init(pages.Length, currentPage);
        HideAllPages();
        ChangePage(0);
    }

    public void InitFields(UserInfoData data)
    {
        userData = data;

        FirstName.text = data.FirstName;
        firstName = data.FirstName;

        LastName.text = data.LastName;
        lastName = data.LastName;

        Telephone.text = data.PhoneNumber;
        telephone = data.PhoneNumber;

        Street.text = data.Street;
        street = data.Street;

        StreetNumber.text = data.Number;
        streetNumber = data.Number;

        Country.text = data.Country;
        country = data.Country;

        City.text = data.City;
        city = data.City;

        PostalCode.text = data.PostalCode;
        postalCode = data.PostalCode;
    }

    private void HideAllPages()
    {
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(false);
    }

    private void SetUpPictureForm()
    {
        ChangeToStepOne();
        uploadButton.interactable = false;
        retakePictureButton.gameObject.SetActive(false);

#if !UNITY_IOS && !UNITY_ANDROID
        takeAPictureButton.gameObject.SetActive(false);
        retakePictureButton.gameObject.SetActive(false);
#if UNITY_WEBGL
        SelectButton.RegisterCallbackOnPressedDown();
#endif
#endif
    }

    private void ChangeToStepOne()
    {
        stepTwoPanel.gameObject.SetActive(false);
        takeAPictureButton.gameObject.SetActive(true);
    }

    private void ChangeToStepTwo()
    {
        stepTwoPanel.gameObject.SetActive(true);
        takeAPictureButton.gameObject.SetActive(false);
    }
    #region Callbacks
    private void CameraCallback(CameraResult result)
    {
        if (result.ResultType == CameraResultType.Success)
        {
            uploadButton.interactable = true;

#if UNITY_IOS || UNITY_ANDROID
            retakePictureButton.gameObject.SetActive(true);
#endif
            pictureImage.sprite = result.texture.ToSprite();
            ChangeToStepTwo();
        }
    }

    public void UploadPictureCallback(CloudResponse response)
    {
        if (response.responseType == UploadResponseType.OK)
        {
            userData.IdImageLink = response.uploadUrl;
            OnImageReady();
        }
        else
        {
            LoadingController.Instance.HidePageLoading();
            SetErrorText("Failed To Upload Picture﻿");
        }
    }
    #endregion Callbacks

    private void ActivatePageIndication(int page)
    {
        IndicatorsView.SelectIndex(page);
    }

    #region Public
    public int GetCurrentPage()
    {
        return currentPage;
    }

    public void NextPage()
    {
        ChangePage(currentPage + 1);
    }
    
    public void ChangePage(int page)
    {
        if (page < pages.Length)
        {
            currentPage = page;

            if (page > 0)
            {
                pages[page - 1].SetActive(false);
                errorPanels[page - 1].ResetHeight();
            }
            pages[page].SetActive(true);
            ActivatePageIndication(page);

            errorResizer = errorPanels[page];
            ErrorText = errorPanels[page].transform.GetChild(0).GetComponent<Text>();
            errorPanels[page].ResetHeight();

            if (page == pages.Length - 1)
                SetUpPictureForm();
        }
    }

    public bool FullValidityCheck()
    {
        string error = null;
        if (Utils.IsStringValid(firstName, "First Name", out error) &&
            Utils.IsStringValid(lastName, "Last Name", out error) &&
            Utils.IsPhoneValid(telephone, out error) &&
            Utils.IsStringValid(street, "Street", out error) &&
            Utils.IsNumberValid(streetNumber, "Street Number", out error) &&
            Utils.IsStringValid(country, "Country", out error) &&
            Utils.IsStringValid(city, "City", out error) &&
            Utils.IsStringValid(postalCode, "Postal Code", out error))
        {
            return true;
        }

        SetErrorText(error);
        return false;
    }

    public bool ValidityCheckWithoutAddress()
    {
        string error = null;
        if (Utils.IsStringValid(firstName, "First Name", out error) &&
            Utils.IsStringValid(lastName, "Last Name", out error) &&
            Utils.IsPhoneValid(telephone, out error))
        {
            return true;
        }

        SetErrorText(error);
        return false;
    }

    public void SetErrorText(string error)
    {
        if (errorResizer != null)
        {
            if (string.IsNullOrEmpty(error))
            {
                errorResizer.SetHeightState(0);
            }
            else
            {
                errorResizer.SetHeightState(1, 20);
                if (ErrorText != null)
                    ErrorText.text = Utils.LocalizeTerm(error);
            }
        }
    }
    #endregion Public

    #region Input
    public void FirstNameEndEdit(string s)
    {
        string error = null;
        if (Utils.IsStringValid(s, "First Name", out error))
            firstName = s;
        else
            firstName = string.Empty;
        userData.FirstName = firstName;
        SetErrorText(error);
    }

    public void LastNameEndEdit(string s)
    {
        string error = null;
        if (Utils.IsStringValid(s, "Last Name", out error))
            lastName = s;
        else
            lastName = string.Empty;

        userData.LastName = lastName;
        SetErrorText(error);
    }

    public void TelephoneEndEdit(string s)
    {
        string error = null;
        if (Utils.IsPhoneValid(s, out error))
            telephone = s;
        else
            telephone = string.Empty;

        userData.PhoneNumber = telephone;
        SetErrorText(error);
    }

    public void StreetEndEdit(string s)
    {
        string error = null;
        if (Utils.IsStringValid(s, "Street", out error))
            street = s;
        else
            street = string.Empty;

        userData.Street = street;
        SetErrorText(error);
    }

    public void StreetNumberEndEdit(string s)
    {
        string error = null;
        if (Utils.IsNumberValid(s, "Street Number", out error))
            streetNumber = s;
        else
            streetNumber = string.Empty;

        userData.Number = streetNumber;
        SetErrorText(error);
    }

    public void CountryEndEdit(string s)
    {
        string error = null;
        if (Utils.IsStringValid(s, "Country", out error))
            country = s;
        else
            country = string.Empty;

        userData.Country = country;
        SetErrorText(error);
    }

    public void CityEndEdit(string s)
    {
        string error = null;
        if (Utils.IsStringValid(s, "City", out error))
            city = s;
        else
            city = string.Empty;

        userData.City = city;
        SetErrorText(error);
    }

    public void PostalCodeEndEdit(string s)
    {
        string error = null;
        if (Utils.IsStringValid(s, "Postal Code", out error))
            postalCode = s;
        else
            postalCode = string.Empty;

        userData.PostalCode = postalCode;
        SetErrorText(error);
    }

    public void CustomerServiceButton()
    {
        PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
    }

    public void TakeAPictureButton()
    {
        if (!PhotoPicker.Instance.OpenCamera(CameraCallback))
        {
#if UNITY_EDITOR
            //===== for testing only ====//
            SelectFromLibraryButton();
#else
            Debug.Log("unsupported Camera");
#endif
        }
    }

    public void SelectFromLibraryButton()
    {
        if (!PhotoPicker.Instance.OpenGallery(CameraCallback))
            Debug.Log("unsupported file browser");
    }

    public void UploadButton()
    {
        LoadingController.Instance.ShowPageLoading();
        AzureUploadKit.UploadTexture(ContentController.Instance, ContentType.IdPicture, pictureImage.sprite.texture, UserController.Instance.gtUser.Id, UploadPictureCallback);
    }
    #endregion Input
}
