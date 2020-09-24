using UnityEngine;
using System.Collections;
using GT.Database;

public class MainSceneController : MonoBehaviour
{
    private static MainSceneController m_instance;
    public static MainSceneController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<MainSceneController>()); }
        private set { m_instance = value; }
    }
    #region Unity Methods

    public Camera MainCamera;

    void Awake()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    void Start()
    {
        Application.targetFrameRate = 30;
        Utils.ChangeOrientation(true);
        Application.runInBackground = true;

        NetworkController.Instance.OnLinksLoaded += NetworkController_OnLinksLoaded;
        NetworkController.Instance.GetGlobalServerLinkAsync();
    }
    #endregion Unity Methods

    #region Private Methods
    private void StartWebSocketConection()
    {
        LoadingController.Instance.SetCurrentSceneLoadingProgress(.4f, "Connecting To Server");
        NetworkController.Instance.Connect();
    }
    #endregion Private Methods

    #region Events
    private void NetworkController_OnLinksLoaded()
    {
        StartCoroutine(DatabaseKit.GetGlobalServerDetails(StartWebSocketConection));
    }
    #endregion Events
}
