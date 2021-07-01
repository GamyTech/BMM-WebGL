using UnityEngine;
//using GT.PayPal;
public class GTPluginBridge : MonoBehaviour {

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            #region PushWoosh
            //PushNotificationKit.GetLaunchNotification();
            #endregion PushWoosh
        }
    }

    #region photopicker
    void PhotoPicker_ImageCallback(string param)
    {
        PhotoPicker.Instance.HandlePluginCallback(param);
    }
    #endregion photopicker

    #region paypal
    public void PayPal_Callback(string msg)
    {
        //PayPalKit.HandlePluginCallback(msg);
    }
    #endregion paypal

}
