using System;
using RESTClient;
using Azure.StorageServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ContentType
{
    ReplayVideo,
    ProfilePicture,
    IdPicture,
    IdentityProof,
    AddressProof,
    Logs,
}

public class AzureUploadKit
{
    private static StorageServiceClient client;
    private static BlobService blobService;

    private static Dictionary<ContentType, string> contentTypeContainerDictionary = new Dictionary<ContentType, string>()
    {
        { ContentType.ProfilePicture, "profilepictures" },
        { ContentType.IdPicture, "idcards" },
    };

    public static void Init()
    {
        string storageAccount = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.StorageAccount);
        Debug.Log("storageAccount " + storageAccount);
        string accessKey = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.StorageKey);
        Debug.Log("accessKey " + accessKey);

        client = StorageServiceClient.Create(storageAccount, accessKey);
        blobService = client.GetBlobService();
    }

    public static void UploadTexture(MonoBehaviour mono, ContentType content, Texture2D texture, string fileName, Action<CloudResponse> callback = null)
    {
        fileName = fileName.EndsWith(".png") ? fileName : fileName + ".png";
        Debug.Log("Azure Uploading " + fileName);

        string container;
        if (contentTypeContainerDictionary.TryGetValue(content, out container))
            mono.StartCoroutine(blobService.PutImageBlob(c => { PutImageCompleted(c, callback); }, texture.EncodeToPNG(), container, fileName, "image/png"));
        else
        {
            Debug.LogError(content.ToString() + " has no container set on the Azure Website");
            callback(new CloudResponse(UploadResponseType.Error));
        }
    }

    private static void PutImageCompleted(RestResponse response, Action<CloudResponse> callback)
    {
        if (response.IsError)
        {
            Debug.LogError("Azure UploadError : " + response.ErrorMessage);
            callback(new CloudResponse(UploadResponseType.Error));
        }
        else
            callback(new CloudResponse(response.Url));
    }
}

public enum UploadResponseType
{
    OK,
    Error,
}

public class CloudResponse
{
    public UploadResponseType responseType;
    public string rawResponse = "";
    public string uploadUrl = "";

    public CloudResponse(UploadResponseType type)
    {
        responseType = type;
    }

    public CloudResponse(string URL)
    {
        this.responseType = UploadResponseType.OK;
        this.uploadUrl = URL;
    }

    #region Overrides
    public override string ToString()
    {
        return "[" + responseType + "]" + rawResponse + " url: " + uploadUrl;
    }
    #endregion Overrides
}

