using UnityEngine;
using GT.Websocket;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using System.Collections;

public class NetworkController : MonoBehaviour
{
    public enum ServerType {
        GlobalServer,
        Local,
        Development,
        Production,
        WebGL,
    }

    private static NetworkController m_instance;
    public static NetworkController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<NetworkController>()); } }

    [HideInInspector]
    public string hostIp;
    [HideInInspector]
    public string serverIp;

    public string GlobalServer;
    public string DevelopmentGlobalServer;
    public string SecurityGlobalServer;

    public ServerType Connection = ServerType.Development;
    public string DevelopmentURL;
    public string ProductionURL;
    public string LocalURL;
    public string WebGLURL;

    public string overrideDeviceId;

    private bool isReconnecting;

    private static string m_deviceID;
    public static string DeviceId
    {
        get
        {
            if (string.IsNullOrEmpty(m_deviceID))
            {
                if (!string.IsNullOrEmpty(Instance.overrideDeviceId))
                    m_deviceID = Instance.overrideDeviceId;
                else
                {
#if UNITY_WEBGL
                    m_deviceID = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.WebGLDeviceId);
                    if (string.IsNullOrEmpty(m_deviceID))
                    {
                        m_deviceID = Guid.NewGuid().ToString();
                        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.WebGLDeviceId, m_deviceID);
                    }
#else
                    m_deviceID = SystemInfo.deviceUniqueIdentifier;
#endif
                }
            }
            return m_deviceID;
        }
    }

    void OnEnable()
    {
        WebSocketKit.Instance.OnOpenEvent += WebSocketKit_OnOpenEvent;
        WebSocketKit.Instance.OnCloseEvent += WebSocketKit_OnCloseEvent;
        WebSocketKit.Instance.OnErrorEvent += WebSocketKit_OnError;   
    }

    void OnDisable()
    {
        WebSocketKit.Instance.OnOpenEvent -= WebSocketKit_OnOpenEvent;
        WebSocketKit.Instance.OnCloseEvent -= WebSocketKit_OnCloseEvent;
        WebSocketKit.Instance.OnErrorEvent -= WebSocketKit_OnError;
    }

    void Start()
    {
        m_instance = this;
    }


    void OnDestroy()
    {
        m_instance = null;
        StopKeepAlive();
    }

    #region Connection Events
    private void WebSocketKit_OnOpenEvent()
    {
        isReconnecting = false;
        StartKeepAlive();
    }

    private void WebSocketKit_OnCloseEvent(ushort code, string message)
    {
        WSResponseCode errorCode;
        Utils.TryParseEnum(code, out errorCode);
        Debug.Log("WebSocket connection was closed. (That might be not an error) Error code (if any): " + errorCode + ", message (if any): " + message);
        switch (errorCode)
        {
            case WSResponseCode.Disconnected:
                if (!message.Equals("Restarting") && !isReconnecting)
                    WebsoketDisconnectPopup();
                else
                    Debug.Log("Connection Closed Normally");
                break;
            case WSResponseCode.MultipleConnection:
                isReconnecting = false;
                Debug.Log("Multiple device connection");
                break;
            default:
                isReconnecting = false;
                WebsoketDisconnectPopup();
                break;
        }
        StopKeepAlive();
    }

    private void WebSocketKit_OnError(string error)
    {
        StartCoroutine(Utils.Wait(1.0f, () =>
        {
            if (WebSocketKit.Instance.IsOpen())
            {
                Debug.Log("Websocket open -- Ignoring error");
                return;
            }

            Debug.Log("WebSocket error: " + error);

            WebsoketDisconnectPopup();
            StopKeepAlive();
        }));
    }
    #endregion Connection Events

    #region Public Game Methods

    public string GetGlobalServerLink()
    {
        return Connection == ServerType.Development ?
                DevelopmentGlobalServer :
                GlobalServer;
    }

    public delegate void LinkLoadingCallback();
    public event LinkLoadingCallback OnLinksLoaded;
    public void GetGlobalServerLinkAsync()
    {
        if (OnLinksLoaded != null)
        {
            OnLinksLoaded();
        }
    }

    public void Connect()
    { 
        if (WidgetController.Instance != null)
            WidgetController.Instance.WidgetPopupView.HidePopups();

        string webSocketURL = null;
        switch(Connection)
        {
            case ServerType.Development:
                webSocketURL = DevelopmentURL;
                break;
            case ServerType.Local:
                webSocketURL = LocalURL;
                break;
            case ServerType.Production:
                webSocketURL = ProductionURL;
                break;
            case ServerType.GlobalServer:
                webSocketURL = serverIp;
                break;
            case ServerType.WebGL:
                webSocketURL = WebGLURL;
                break;
        }
        Debug.Log("WS link to connect: " + webSocketURL);
        WebSocketKit.Instance.OpenConnection(webSocketURL, hostIp);
    }

    public void Reconnect()
    {
        isReconnecting = true;
        WebSocketKit.Instance.webSocket.Close();
        Connect();
    }
    #endregion Public API Methods

    #region Private Methods
    private void WebsoketDisconnectPopup()
    {
        PopupController.Instance.ShowSmallPopup("Disconnected", new string[] { "Press try again to retry" }, new SmallPopupButton("Try Again", Connect));
        LoadingController.Instance.ShowPageLoading();
    }

    private void Logout()
    {
        Connect();
    }

    private void StartKeepAlive()
    {
        StopKeepAlive();

#if UNITY_WEBGL || UNITY_EDITOR
        m_keepAliveCo = KeepAliveCo();
        StartCoroutine(m_keepAliveCo);
#else
        thread = new Thread(KeepAliveThread);
        thread.Start();
#endif
    }

    private void StopKeepAlive()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        if(m_keepAliveCo != null)
            StopCoroutine(m_keepAliveCo);
        m_keepAliveCo = null;
#else
        if (thread != null)
            thread.Abort();
        thread = null;
#endif
    }

#if UNITY_WEBGL || UNITY_EDITOR
    System.Collections.IEnumerator m_keepAliveCo;
    private System.Collections.IEnumerator KeepAliveCo()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("DeviceId", DeviceId);
        dic.Add("Time", "");
        while (true)
        {
            dic["Time"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            WebSocketKit.Instance.SendRequest(RequestId.IsAlive, dic);
            yield return new WaitForSeconds(5);
        }
    }
#else
    Thread thread;
    private void KeepAliveThread()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("DeviceId", DeviceId);
        dic.Add("Time", "");

        while (true)
        {
            dic["Time"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            WebSocketKit.Instance.SendRequest(RequestId.IsAlive, dic);
            Thread.Sleep(5000);
        }
    }
#endif

    #endregion Private Methods
}
