using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


namespace GT.PayPal
{

    public class PayPalKit : MonoBehaviour
    {
        //private static string PaypalClientId = "AX6xxyBjHbNdl57ohgo6JiG7x0LDQP_F-SlbNv-BpwUrvXTP9MVOiJ7JzeLUiJU0NnmmsXAnRA917Tbv";
#if UNITY_ANDROID
        private const string javaClass = "com.gamytech.paypal.PPController";
        private static AndroidJavaClass paypalAndroid;
#endif
        public delegate void OpenPayPalCallback(PayPalResponse response);
        private static OpenPayPalCallback paypalCallback;
        public delegate void OpenScannerCallback(ScannerResponse response);
        private static OpenScannerCallback scannerCallback;
        private static bool askForScan = false;

        public static void Initialize(string clientId)
        {
            Debug.Log("Initialize PayPal " + clientId);
#if UNITY_ANDROID
            //using (var pluginClass = new AndroidJavaClass("com.gamytech.paypal.PPController"))
            //{
            //    paypalAndroid = pluginClass;
            //}
#elif UNITY_IOS
            IOSBridge.InitPaypal(clientId);
#endif
        }

        public static bool IsPayPalSupported()
        {
            return false;
        }

        public static bool IsScannerSupported()
        {
#if UNITY_ANDROID
            return false;
#elif UNITY_IOS
            return true;
#else
            return false;
#endif
        }

        public static void OpenPayPal(OpenPayPalCallback callback, string itemName, string email, float price)
        {
            askForScan = false;
            paypalCallback = callback;
#if UNITY_ANDROID
            //paypalAndroid.CallStatic("startPayPal", itemName, price, email);
            HandelOpenPayapalResponse(new PayPalResponse(PayPalResponseType.NotSupported));
#elif UNITY_IOS
            IOSBridge.ChangePaypalViewController(itemName, (int)price, email);
#else
            HandelOpenPayapalResponse(new PayPalResponse(PayPalResponseType.NotSupported));
#endif
        }

        public static void HandelOpenPayapalResponse(PayPalResponse response)
        {
            if (paypalCallback != null)
                paypalCallback(response);
            paypalCallback = null;
        }

        public static void OpenScanner(OpenScannerCallback callback)
        {
            askForScan = true;
            scannerCallback = callback;
#if UNITY_ANDROID
            HandelScannerResponse(new ScannerResponse(ScannerResponseType.NotSupported));
#elif UNITY_IOS
            IOSBridge.ChangeCardScannerViewController();
#else
            HandelScannerResponse(new ScannerResponse(ScannerResponseType.NotSupported));
#endif
        }

        public static void HandelScannerResponse(ScannerResponse response)
        {
            if (scannerCallback != null)
                scannerCallback(response);
            scannerCallback = null;
        }

        public static void HandlePluginCallback(string msg)
        {
            if (askForScan)
            {
                Debug.Log("CardIOResultParse " + msg);
                ScannerResponse response;
                if (string.IsNullOrEmpty(msg))
                    response = new ScannerResponse(ScannerResponseType.Cancel);
                else
                {
                    string[] words = msg.Split(':');
                    response = new ScannerResponse(ScannerResponseType.OK, words[0], words[1], words[2], words[3]);
                }

                HandelScannerResponse(response);
            }
            else
            {
                Debug.Log("ObjectiveC " + msg);
                PayPalResponse response;
                if (string.IsNullOrEmpty(msg))
                    response = new PayPalResponse(PayPalResponseType.Cancel);
                else
                {
                    string[] words = msg.Split(':');
                    response = new PayPalResponse(PayPalResponseType.OK, words[0], words[1], words[2], words[3], words[4]);
                }

                HandelOpenPayapalResponse(response);
            }
        }


        //private IEnumerator TestIEnumerator()
        //{
        //    Dictionary<string, object> _formDictionary = new Dictionary<string, object>();
        //    _formDictionary.Add("Service", "CreatePayPalOrder");
        //    _formDictionary.Add("UserId", "izululock@gmail.com");
        //    _formDictionary.Add("DepositAmount", "1");
        //    _formDictionary.Add("GameId", "Backgammon4Money");
        //    var _dict = Json.Serialize(_formDictionary);
        //    _dict = GT.Encryption.Encrypt(_dict.ToString(), Encryption.CryptPass);
        //    Debug.Log(_dict);

        //    WWWForm form = new WWWForm();
        //    form.AddField("JSON", _dict);
        //    string globalAddress = "http://globalserver.azurewebsites.net/Api/GlobalServer02.ashx";
        //    Debug.Log(globalAddress);
        //    WWW w = new WWW(globalAddress, form.data);
        //    yield return w;
        //    Debug.Log(w.text);
        //    string _currentJSON = w.text;
        //    string _actuallData = DecryptNewInput(_currentJSON);
        //    if (_actuallData != string.Empty)
        //        currentJSON = actuallData;
        //}

    }
}
