using UnityEngine;
using UnityEngine.UI;
using GT.Websocket;
using System.IO;
using System.Collections;

public class IdentityProofWidget : Widget
{
    public Button takeAPictureButton;
    public Button uploadButton;
    public RectTransform dynamicTextPanel;
    public Image pictureImage;

    public override void EnableWidget()
    {
        base.EnableWidget();
        WebSocketKit.Instance.AckEvents[RequestId.AddPendingVerification] += OnVerifyRequest;
        uploadButton.interactable = false;

#if !UNITY_IOS && !UNITY_ANDROID
        takeAPictureButton.gameObject.SetActive(false);
#endif
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.AddPendingVerification] -= OnVerifyRequest;
        base.DisableWidget();
    }

    private void UpdateServer(string picURL)
    {
        UserController.Instance.SendIdentityProofToServer(Enums.VerificationRank.IdentityVerified, picURL);
    }

    #region Callbacks
    private void CameraCallback(CameraResult result)
    {
        Debug.Log("CameraCallback " + result);
        if (result.ResultType == CameraResultType.Success)
        {
            uploadButton.interactable = true;
            pictureImage.sprite = result.texture.ToSprite();
        }
    }

    public void UploadPictureCallback(CloudResponse response)
    {
        Debug.Log("UploadPictureCallback: " + response);
        if (response.responseType == UploadResponseType.OK)
            UpdateServer(response.uploadUrl);
        else
        {
            LoadingController.Instance.HidePageLoading();
            PopupController.Instance.ShowSmallPopup("Failed To Upload Picture﻿", new SmallPopupButton("OK"));
        }
    }

    private void OnVerifyRequest(Ack ack)
    {
        AddPendingVerificationAck response = ack as AddPendingVerificationAck;

        Debug.Log(ack.RawData);
        LoadingController.Instance.HidePageLoading();
        if(response.Code == WSResponseCode.OK)
        {
            PopupController.Instance.ShowSmallPopup("Request Sent", new string[] { "Your identity is going to be verified." },
                new SmallPopupButton("Ok", () => PageController.Instance.ChangePage(Enums.PageId.Home)));
        }
    }

    #endregion Callbacks

    #region Buttons
    public void TakeAPictureButton()
    {
        Debug.Log("TakeAPictureButton");
        if (!PhotoPicker.Instance.OpenCamera(CameraCallback))
        {
#if UNITY_EDITOR
            CameraCallback(new CameraResult(CameraResultType.Success, "", AppInformation.LOGO.texture));
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
        AzureUploadKit.UploadTexture(ContentController.Instance, ContentType.IdentityProof, pictureImage.sprite.texture, UserController.Instance.gtUser.Id, UploadPictureCallback);
    }
    #endregion Buttons
}

