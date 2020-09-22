using System;
using UnityEngine;
using System.Runtime.InteropServices;
using SFB;
using UnityToolbag;
using System.Collections;

public class PhotoPicker
{

#if UNITY_ANDROID
    AndroidJavaObject photoPickerObject;
    AndroidJavaObject unityObject;
#elif UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _SelectPicture(string filename, string callbackFuncName, string objectName);
    [DllImport("__Internal")]
    private static extern void _TakePicture(string filename, string callbackFuncName, string objectName);
#elif UNITY_WEBGL
   /* [DllImport("__Internal")]
    private static extern void _SelectPicture(string objectName, string callbackFuncName);*/
#endif

    private static PhotoPicker m_instance;
    public static PhotoPicker Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new PhotoPicker();
            return m_instance;
        }
    }

    Action<CameraResult> Callback;

    private PhotoPicker()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass photoPickerClass = new AndroidJavaClass("com.GamytechPlugin.photopicker.PhotoPicker");
        photoPickerObject = photoPickerClass.GetStatic<AndroidJavaObject>("instance");
#endif
    }

    public bool OpenCamera(Action<CameraResult> callback)
    {
        Callback = callback;
#if UNITY_EDITOR
        return false;
#elif UNITY_ANDROID
        photoPickerObject.Call("launchCamera", unityObject);
        return true;
#elif UNITY_IOS
        _TakePicture("picture.png", "PhotoPicker_ImageCallback","GTPluginBridge");
        return true;
#else
        return false;
#endif
    }

    public bool OpenGallery(Action<CameraResult> callback)
    {
        Callback = callback;
#if UNITY_STANDALONE
        ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg") };
        StandaloneFileBrowser.OpenFilePanelAsync("Select Your Picture", "", extensions, false, s => { HandlePluginCallback(s.Length > 0 ? s[0] : ""); });
        return true;
#elif UNITY_ANDROID
        photoPickerObject.Call("launchGallery", unityObject);
        return true;
#elif UNITY_IOS
        _SelectPicture("picture.png", "PhotoPicker_ImageCallback","GTPluginBridge");
        return true;
#elif UNITY_WEBGL
        /*_SelectPicture("GTPluginBridge","PhotoPicker_ImageCallback");*/
        return true;
#else
        return false;
#endif
    }

    public void HandlePluginCallback(string data)
    {
        Debug.Log("raw photopicker data :" + data);
        if(Callback != null)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            CreatePictureFromURL(string.IsNullOrEmpty(data) ? null : new Uri(data).AbsoluteUri);
#elif UNITY_ANDROID
            CreatePictureFromData(data, photoPickerObject.Call<byte[]>("getPluginData"));
            cleanUp();
#elif UNITY_IOS
            CreatePictureFromURL("file://" + data);
#elif UNITY_WEBGL
            CreatePictureFromURL(data);
#endif
        }
    }

    private void CreatePictureFromURL(string url)
    {
        Debug.Log("OnImageSaved by URL " + url);

        if (string.IsNullOrEmpty(url))
            SendResult(CameraResultType.Cancel, null, null);
        else
        {
            Action<Texture2D> successCallback = t => { SendResult(CameraResultType.Success, null, t); };
            Action<string> errorCallback = e => { SendResult(CameraResultType.Error, e, null); };
            ContentController.Instance.DownloadPicOrError(url, successCallback, errorCallback);
        }
    }

    private void CreatePictureFromData(string error, byte[] imageData)
    {
        Debug.Log(error + " OnImageLoad by byte[] " + imageData);

        CameraResultType result = CameraResultType.Error;
        Texture2D texture = new Texture2D(1,1);

        if (string.IsNullOrEmpty(error))
        {
            if (imageData == null)
                result = CameraResultType.Cancel;
            else if (imageData.Length == 0)
                error = "imageData is empty";
            else if (!texture.LoadImage(imageData))
                error = "imageData is Unreadable";
            else
                result = CameraResultType.Success;
        }

        SendResult(result, error, texture);
    }

    private void SendResult(CameraResultType result, string error = null, Texture2D texture = null)
    {
        CameraResult camResult = new CameraResult(result, error, texture);
        Debug.Log(camResult.ToString());
        if (Callback != null)
            Callback(camResult);
    }

    public void cleanUp()
    {
#if UNITY_ANDROID
        photoPickerObject.Call("cleanUp");
#endif
    }
}

public enum CameraResultType
{
    Success,
    Error,
    Cancel,
}

public class CameraResult
{
    public CameraResultType ResultType { get; private set; }
    public string Error { get; private set; }
    public Texture2D texture { get; private set; }

    public CameraResult(CameraResultType result, string error, Texture2D texture = null)
    {
        this.ResultType = result;
        this.Error = error;
        this.texture = texture;
    }
    public override string ToString()
    {
        return "[" + ResultType + "]" + (string.IsNullOrEmpty(Error) ? string.Empty : " Error:" + Error) + (texture == null ? " No Texture" : " texture X: " + texture.width);
    }
}