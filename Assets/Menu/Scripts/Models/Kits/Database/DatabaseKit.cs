using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using MiniJSON;
using GT.Encryption;

namespace GT.Database
{
    internal enum ServerRequestKey
    {
        Service,
        GameId,
        Versions,
    }

    public enum VersioningKeys
    {
        C1001, //Bets
        C1002, //Fees
        C1003, //Loyatly
        C1004, //Category
        C1005, //CashInAmounts
        C1006, //CashOutAmount
        C1008, //Stores
        C1009, //Ads
        C1010, //Localization
        C1011, //Maintenance
        C1012, //AppFlyer
        C1013, //AppFlyerDevKit
        C1014, //PushWoosh
        C1015, //GameWebSite
        C1017, //GcmProjectNumber
        C1018, //IosAppSite
        C1019, //IosAppRateSite
        C1020, //AndroidAppSite
        C1021, //AssetBundleURL
        C1022, //StorageAccount
        C1023, //StorageKey
        C1024, // Terms and conditions
        C1025, // Privacy Policy
        C1026, //AppVersion
    }


    public static class DatabaseKit
    {
        #region Cert
#if UNITY_ANDROID && !UNITY_EDITOR
        private static string _cert = @"-----BEGIN CERTIFICATE-----
            MIIGnzCCBYegAwIBAgIHB4uB/cc2DDANBgkqhkiG9w0BAQsFADCBjDELMAkGA1UE
            BhMCSUwxFjAUBgNVBAoTDVN0YXJ0Q29tIEx0ZC4xKzApBgNVBAsTIlNlY3VyZSBE
            aWdpdGFsIENlcnRpZmljYXRlIFNpZ25pbmcxODA2BgNVBAMTL1N0YXJ0Q29tIENs
            YXNzIDIgUHJpbWFyeSBJbnRlcm1lZGlhdGUgU2VydmVyIENBMB4XDTE1MDYyMjE5
            NTcxMloXDTE3MDYyMzEzMTMzNlowgZsxCzAJBgNVBAYTAk1UMQ4wDAYDVQQIEwVN
            YWx0YTEbMBkGA1UEBx4SAFQAQSAZACAAWABCAEkARQBYMRwwGgYDVQQKExNHVCBI
            b2xkaW5ncyBMaW1pdGVkMRswGQYDVQQDFBIqLmdhbXl0ZWNoYXBpcy5jb20xJDAi
            BgkqhkiG9w0BCQEWFWpvbmF0aGFuQGdhbXl0ZWNoLmNvbTCCASIwDQYJKoZIhvcN
            AQEBBQADggEPADCCAQoCggEBAJGn3JVypUGtmjT+YsTESsH6lSMMmolEP0kI15BL
            1S4gMyLU8vttzb7B9yUdowN95v2fNx1Q3BRWyIoRsyV4I21LmsCWWPWv6nlQGHQQ
            XOYScbdju3wIw526ZLM35SFfHLZovqziiX2By99qFrLg8Med9nfgWtt5IO9aqEj3
            EmgsgaoQasbAEs5G75Hw5d228env8e2VNzsDDUHa3Moz5K+uwxHON8AGiJJrVyUC
            Fm7wgOTDvew1IxMAllvE2zf5MzdWQsFtq+yfzFaf+bqi9DN48WNUCZMO2TzQijjl
            9GuLxAPiffWX5GVwyuemeoFWVV4cTGfnEXRpdJyOXd7pP28CAwEAAaOCAvMwggLv
            MAkGA1UdEwQCMAAwCwYDVR0PBAQDAgOoMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggr
            BgEFBQcDATAdBgNVHQ4EFgQUBenHlS6JKlvLuuzosczekdW9UrYwHwYDVR0jBBgw
            FoAUEdsjRf1UzGpxb4SKA9e+9wEvJoYwLwYDVR0RBCgwJoISKi5nYW15dGVjaGFw
            aXMuY29tghBnYW15dGVjaGFwaXMuY29tMIIBVgYDVR0gBIIBTTCCAUkwCAYGZ4EM
            AQICMIIBOwYLKwYBBAGBtTcBAgMwggEqMC4GCCsGAQUFBwIBFiJodHRwOi8vd3d3
            LnN0YXJ0c3NsLmNvbS9wb2xpY3kucGRmMIH3BggrBgEFBQcCAjCB6jAnFiBTdGFy
            dENvbSBDZXJ0aWZpY2F0aW9uIEF1dGhvcml0eTADAgEBGoG+VGhpcyBjZXJ0aWZp
            Y2F0ZSB3YXMgaXNzdWVkIGFjY29yZGluZyB0byB0aGUgQ2xhc3MgMiBWYWxpZGF0
            aW9uIHJlcXVpcmVtZW50cyBvZiB0aGUgU3RhcnRDb20gQ0EgcG9saWN5LCByZWxp
            YW5jZSBvbmx5IGZvciB0aGUgaW50ZW5kZWQgcHVycG9zZSBpbiBjb21wbGlhbmNl
            IG9mIHRoZSByZWx5aW5nIHBhcnR5IG9ibGlnYXRpb25zLjA1BgNVHR8ELjAsMCqg
            KKAmhiRodHRwOi8vY3JsLnN0YXJ0c3NsLmNvbS9jcnQyLWNybC5jcmwwgY4GCCsG
            AQUFBwEBBIGBMH8wOQYIKwYBBQUHMAGGLWh0dHA6Ly9vY3NwLnN0YXJ0c3NsLmNv
            bS9zdWIvY2xhc3MyL3NlcnZlci9jYTBCBggrBgEFBQcwAoY2aHR0cDovL2FpYS5z
            dGFydHNzbC5jb20vY2VydHMvc3ViLmNsYXNzMi5zZXJ2ZXIuY2EuY3J0MCMGA1Ud
            EgQcMBqGGGh0dHA6Ly93d3cuc3RhcnRzc2wuY29tLzANBgkqhkiG9w0BAQsFAAOC
            AQEAfaamY58AKIj1UX6UgHcMDzGeCcmTtCKAuX/1K69rM0S5AQ1P1gq9oi8e8ggv
            3qwVU5gHOl+EH9S1XmiEpIrLIBnVXG+0KonPSWAsiYk/e90MfrD/RrXx/LpteHB6
            tlRaIL4Gv1GFfUA3l0GBuEv2dZ0huBVxul3fAK6UQoNy8NCiQ1Mp35Vis3M64Zqq
            Upzo7eRyxp8Rp1ejqB7NVcvADBIg/iy3sxBAXvaBZ1QN4siWbm0MNiHAy2ZwGfZL
            vLfgME2YSO7FA9KHFQDwUzkRDmD0qicYhEnnezhfN7TPX8mUqt6TEKLSi4hBTSpb
            +n5H8pDA/PQFAXZZHXTf0goAng==
            -----END CERTIFICATE-----";
        private static bool androidHttpsHelperInitialized = false;
#endif
        #endregion Cert
        public static Dictionary<VersioningKeys, Enums.PlayerPrefsVariable> detailsToPlayersPrefs = new Dictionary<VersioningKeys, Enums.PlayerPrefsVariable>()
        {
            { VersioningKeys.C1001, Enums.PlayerPrefsVariable.Bets },
            { VersioningKeys.C1002, Enums.PlayerPrefsVariable.Fees },
            { VersioningKeys.C1003, Enums.PlayerPrefsVariable.Loyatly },
            { VersioningKeys.C1004, Enums.PlayerPrefsVariable.Category },
            { VersioningKeys.C1005, Enums.PlayerPrefsVariable.CashInAmounts },
            { VersioningKeys.C1006, Enums.PlayerPrefsVariable.MinCashout },
            { VersioningKeys.C1008, Enums.PlayerPrefsVariable.Stores },
            { VersioningKeys.C1009, Enums.PlayerPrefsVariable.Ads },
            { VersioningKeys.C1010, Enums.PlayerPrefsVariable.LocalizationVer },
            { VersioningKeys.C1011, Enums.PlayerPrefsVariable.IsMaintenance },
            { VersioningKeys.C1012, Enums.PlayerPrefsVariable.AppsFlyerIOSID },
            { VersioningKeys.C1013, Enums.PlayerPrefsVariable.AppFlyerDevKit },
            { VersioningKeys.C1014, Enums.PlayerPrefsVariable.PushwooshAppCode },
            { VersioningKeys.C1015, Enums.PlayerPrefsVariable.GameWebSite },
            { VersioningKeys.C1017, Enums.PlayerPrefsVariable.GcmProjectNumber },
            { VersioningKeys.C1018, Enums.PlayerPrefsVariable.IosAppSite },
            { VersioningKeys.C1019, Enums.PlayerPrefsVariable.IosAppRateSite },
            { VersioningKeys.C1020, Enums.PlayerPrefsVariable.AndroidAppSite },
            { VersioningKeys.C1021, Enums.PlayerPrefsVariable.AssetBundleURL },
            { VersioningKeys.C1022, Enums.PlayerPrefsVariable.StorageAccount },
            { VersioningKeys.C1023, Enums.PlayerPrefsVariable.StorageKey },
            { VersioningKeys.C1024, Enums.PlayerPrefsVariable.TermsAndConditionsLink },
            { VersioningKeys.C1025, Enums.PlayerPrefsVariable.PrivacyPolicyLink },
        };

        const string DevGlobalServer = "https://prodenvironmentserver03v2.gamytechapis.com/ApiRequestDev";
        const string DevLocalServer = "http://10.0.0.95:8080/GamyTechServer2.2B/RequestHandler";

        #region User Functions
        public static IEnumerator GetGlobalServerDetails(UnityAction callback)
        {
            Dictionary<string, object> formDict = new Dictionary<string, object>();
            formDict.Add(ServerRequestKey.Service.ToString(), "GetServerDetails");
            formDict.Add(ServerRequestKey.GameId.ToString(), AppInformation.GAME_ID.ToString());

            string localVers = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Versions);
            Dictionary<string, object>  VersDict = string.IsNullOrEmpty(localVers) ? new Dictionary<string, object>() : Json.Deserialize(localVers) as Dictionary<string, object>;
            VersDict.AddOrOverrideValue(VersioningKeys.C1026.ToString(), AppInformation.RELEASED_VERSION);

            string[] keysList = Enum.GetNames(typeof(VersioningKeys));
            for (int x = 0; x < keysList.Length; ++x)
                if (!VersDict.ContainsKey(keysList[x]))
                    VersDict.Add(keysList[x], 0);

            formDict.Add(ServerRequestKey.Versions.ToString(), VersDict);

            string message = Json.Serialize(formDict);
            WWWForm form = new WWWForm();
            form.AddField("JSON", AesBase64Wrapper.Encrypt(message));

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!androidHttpsHelperInitialized)
            {
                //AndroidHttpsHelper.AddCertificate(_cert);
                androidHttpsHelperInitialized = true;
            }
#endif
            string link = NetworkController.Instance.GetGlobalServerLink();
            WWW w = new WWW(link, form);
            Debug.Log("Sending Details to " + link + " : " + message);
            AssetController.Instance.InitializeAssetBundles();

            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                Debug.Log("Server reques error:");
                Debug.Log(w.error);
                Debug.Log("Server reques response headers:");
                Debug.Log(w.responseHeaders.Display());
            }
            GlobalServerDetailsResponse response = new GlobalServerDetailsResponse(w);
            Debug.Log("<color=red>Details Response: " + response + "</color>");
            w.Dispose();

            if (response.responseCode != GSResponseCode.OK)
            {
                w = new WWW(NetworkController.Instance.SecurityGlobalServer, form);
                yield return w;
                if (!string.IsNullOrEmpty(w.error))
                {
                    Debug.Log("Second try: Server reques error:");
                    Debug.Log(w.error);
                    Debug.Log("Second try: Server reques response headers:");
                    Debug.Log(w.responseHeaders);
                }
                response = new GlobalServerDetailsResponse(w);
                Debug.Log("<color=red>Second Response Try: " + response + "</color>");
                w.Dispose();
            }

            if (response.responseCode == GSResponseCode.OK)
                SaveDetails(response, callback);
            else//Dima FIX: Responce gets ok but still goesinto here?
            {
                Debug.Log("The response from server was not 'OK': " + response.responseCode);
                Debug.Log(response);

                TrackingKit.InitTracking();
                PopupController.Instance.ShowSmallPopup("Failed to connect, Try again.", new SmallPopupButton("Try Again", SceneController.Instance.RestartApp));
            }
        }

        public static IEnumerator CheckAppsFlyer()
        {
            Dictionary<string, object> formDict = new Dictionary<string, object>();
            formDict.Add(ServerRequestKey.Service.ToString(), "CheckAppsFlyer");
            formDict.Add(ServerRequestKey.GameId.ToString(), AppInformation.GAME_ID);
            formDict.Add("DeviceId", NetworkController.DeviceId);
            formDict.Add("OS", Application.platform);
            formDict.Add("Ip", NetworkController.Instance.hostIp);
            formDict.Add("Version", AppInformation.BUNDLE_VERSION);
            formDict.Add("Device", SystemInfo.deviceModel);
            formDict.Add("Date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            string message = Json.Serialize(formDict);
            WWWForm form = new WWWForm();
            form.AddField("JSON", AesBase64Wrapper.Encrypt(message));

            string link = NetworkController.Instance.GetGlobalServerLink();
            Debug.Log("Sending CheckAppsFlyer to " + link + " : " + message);
            WWW w = new WWW(link, form);
            yield return w;
            if(!string.IsNullOrEmpty(w.error))
                Debug.LogWarning("CheckAppsFlyer error : " + w.error);
        }

        public static IEnumerator GetMatchLog(string matcId, UnityAction<GetServerMatchHistoryMoves> callback)
        {
            Dictionary<string, object> formDict = new Dictionary<string, object>();
            formDict.Add(ServerRequestKey.Service.ToString(), "GetMatchHistoryMoves");
            formDict.Add("MatchId", matcId);

            string message = Json.Serialize(formDict);
            Debug.Log("Sending GetMatchLog to global Server : " + message);
            message = AesBase64Wrapper.Encrypt(message);
            WWWForm form = new WWWForm();
            form.AddField("JSON", message);

            // TODO: change this lisnk
            WWW w = new WWW("https://prodenvironmentserver02v2.gamytechapis.com/BackOffice/MainHandler", form);
            yield return w;
            GetServerMatchHistoryMoves response = new GetServerMatchHistoryMoves(w);
            Debug.Log("<color=red>GetMatchLog Response: " + response + "</color>");
            callback(response);
            w.Dispose();
        }
        #endregion User Functions

        static void SaveDetails(GlobalServerDetailsResponse details, UnityAction callback)
        {
            string localVers = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Versions);
            Dictionary<string, object> VersDict = string.IsNullOrEmpty(localVers) ? new Dictionary<string, object>() : Json.Deserialize(localVers) as Dictionary<string, object>;
            foreach (var item in details.NewData)
            {
                VersDict.AddOrOverrideValue(item.Key.ToString(), item.Value.Version);
                Enums.PlayerPrefsVariable playerPrefKey;
                if (detailsToPlayersPrefs.TryGetValue(item.Key, out playerPrefKey) && item.Value.Data != null)
                {
                    if (item.Value.Data is ICollection)
                        GTDataManagementKit.SaveToPlayerPrefs(playerPrefKey, Json.Serialize(item.Value.Data));
                    else
                        GTDataManagementKit.SaveToPlayerPrefs(playerPrefKey, item.Value.Data.ToString());
                }
            }
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.Versions, Json.Serialize(VersDict));

            GlobalServerDetailsResponse.VersionData d;
            if (details.NewData.TryGetValue(VersioningKeys.C1026, out d))
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.AppVersion, d.Version);
            else
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.AppVersion, AppInformation.RELEASED_VERSION);

            TrackingKit.InitTracking();
            AzureUploadKit.Init();
            ContentController.Initialize();

            GlobalServerDetailsResponse.VersionData versionsData;
            if (GTDataManagementKit.GetIntFromPrefs(Enums.PlayerPrefsVariable.AppVersion) > AppInformation.RELEASED_VERSION)
            {
                PopupController.Instance.ShowPopup(Enums.PopupId.Update);
                return;
            }
                
            if (GTDataManagementKit.GetBoolFromPrefs(Enums.PlayerPrefsVariable.IsMaintenance))
            {
                PopupController.Instance.ShowPopup(Enums.PopupId.Maintenance);
                return;
            }

            if (string.IsNullOrEmpty(details.HostIp) || string.IsNullOrEmpty(details.ServerIp))
            {
                PopupController.Instance.ShowSmallPopup("Failed to connect, Try again.", new SmallPopupButton("Try Again", SceneController.Instance.RestartApp));
                return;
            }

            if (details.NewData.TryGetValue(VersioningKeys.C1010, out versionsData))
                I2.Loc.LocalizationManager.UpdateSources();

            NetworkController.Instance.hostIp = details.HostIp;
            NetworkController.Instance.serverIp = details.ServerIp;
            FacebookKit.Init();

            callback();
        }
    }
}
