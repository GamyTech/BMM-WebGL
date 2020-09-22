using UnityEngine;
using UnityEngine.UI;
using GT.Websocket;
using System.Collections.Generic;

public class ChangeProfileWidget : Widget
{
    public Text errorText;
    public Button TakeAPicture;
    public Button BrowseButton;
    public Image pictureImage;
    public Button SubmitButton;

    public int maxTextureSize = 256;

    private bool HasChangedPicture  = false;

    public override void EnableWidget()
    {
        base.EnableWidget();

#if !UNITY_ANDROID && !UNITY_IOS
        TakeAPicture.gameObject.SetActive(false);
#if UNITY_WEBGL
        BrowseButton.RegisterCallbackOnPressedDown();
#endif
#endif

        pictureImage.sprite = UserController.Instance.gtUser.Avatar;
        SubmitButton.gameObject.SetActive(false);
    }

    public override void DisableWidget()
    {
#if UNITY_WEBGL
        BrowseButton.UnregisterCallbackOnPressedDown();
#endif
        WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.UpdateProfilePicture, SetAvatar);
        base.DisableWidget();
    }

    private void SetError(string error = null)
    {
        errorText.text = string.IsNullOrEmpty(error) ? "" : Utils.LocalizeTerm(error);
    }

    #region Events
    private void SetAvatar(object o)
    {
        LoadingController.Instance.HidePageLoading();
        WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.UpdateProfilePicture, SetAvatar);

        Dictionary<string, object> responseDict = o as Dictionary<string, object>;
        object error = null;
        if (responseDict != null && responseDict.TryGetValue("ErrorCode", out error))
        {
            Debug.LogError("failed to update new Url to server : " + error.ToString());
            SetError("failed to update your avatar.");
        }
        else
        {
            Debug.Log("On UpdateProfilePicture APIUpdate");
            PageController.Instance.ChangePage(Enums.PageId.Home);
            SetError();
        }
    }

    private void CameraCallback(CameraResult result)
    {
        Debug.Log("CameraCallback " + result);
        if (result.ResultType == CameraResultType.Success)
        {
            GTDataManagementKit.ResizeTexture(GTDataManagementKit.ScaleMode.FillCrop, maxTextureSize, result.texture);
            pictureImage.sprite = result.texture.ToSprite();
            SubmitButton.gameObject.SetActive(true);
            HasChangedPicture = true;
        }

        SetError(result.Error);
    }

    private void UploadPictureCallback(CloudResponse response)
    {
        Debug.Log("UploadPictureCallback: " + response);
        if (response.responseType == UploadResponseType.OK && !string.IsNullOrEmpty(response.uploadUrl))
        {
            WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.UpdateProfilePicture, SetAvatar);
            UserController.Instance.UpdateAvatar(response.uploadUrl);
            SetError();
        }
        else
        {
            LoadingController.Instance.HidePageLoading();
            Debug.LogError("failed to upload pic to Azure : " + response.rawResponse);
            SetError("failed to upload pic.");
        }
    }
    #endregion Events

    #region Buttons/Inputs
    public void SelectFromLibraryButton()
    {
        if (!PhotoPicker.Instance.OpenGallery(CameraCallback))
            Debug.Log("unsupported file browser");
    }

    public void TakeAPictureButton()
    {
        if (!PhotoPicker.Instance.OpenCamera(CameraCallback))
            Debug.Log("unsupported camera");
    }

    public void Submit()
    {
        Debug.Log("Submit clicked");
        if (HasChangedPicture)
        {
            LoadingController.Instance.ShowPageLoading(transform as RectTransform);
            AzureUploadKit.UploadTexture(ContentController.Instance, ContentType.ProfilePicture, pictureImage.sprite.texture, UserController.Instance.gtUser.Id, UploadPictureCallback);
        }
    }
    #endregion Buttons/Inputs
}
