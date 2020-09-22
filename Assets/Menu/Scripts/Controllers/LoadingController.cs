using UnityEngine;
using System;

public class LoadingController : MonoBehaviour
{
    private static LoadingController m_instance;
    public static LoadingController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<LoadingController>()); }
        private set { m_instance = value; }
    }

    private SceneLoadingView m_sceneLoadingView;
    protected SceneLoadingView sceneLoadingView
    {
        get
        {
            if (m_sceneLoadingView == null)
                m_sceneLoadingView = FindObjectOfType<SceneLoadingView>();
            return m_sceneLoadingView;
        }
    }

    private PageLoadingView m_pageLoadingView;
    public PageLoadingView pageLoadingView
    {
        get
        {
            if (m_pageLoadingView == null)
                m_pageLoadingView = FindObjectOfType<PageLoadingView>();
            return m_pageLoadingView;
        }
    }

    private BlackLoadingView m_blackLoadingView;
    protected BlackLoadingView blackLoadingView
    {
        get
        {
            if (m_blackLoadingView == null)
                m_blackLoadingView = FindObjectOfType<BlackLoadingView>();
            return m_blackLoadingView;
        }
    }

    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        Instance = null;
    }

    void Start()
    {
        sceneLoadingView.ShowLoading(null, true);
    }

    public void ShowPageLoading(RectTransform over = null, Action finishedCallback = null)
    {
        pageLoadingView.ShowLoading(over, finishedCallback);
    }

    public void HidePageLoading(Action finishedCallback = null)
    {
        pageLoadingView.HideLoading(finishedCallback);
    }

    public void ShowSceneLoading(Action finishedCallback = null, bool isBlack = false)
    {
        if(isBlack)
            blackLoadingView.ShowLoading(finishedCallback);
        else
            sceneLoadingView.ShowLoading(finishedCallback);
    }

    public void HideSceneLoading(Action finishedCallback = null)
    {
        sceneLoadingView.HideLoading(finishedCallback);
        blackLoadingView.HideLoading(finishedCallback);
    }

    public void ShowSpecifProgressBar()
    {
        sceneLoadingView.ShowSpecifProgressBar();
    }

    public void SetCurrentSceneLoadingProgress(float percent, string description, bool start = false)
    {
        sceneLoadingView.SetTotalProgressBar(percent, start);
        sceneLoadingView.HideSpecifProgressBar();
        sceneLoadingView.SetLoadingInfoText(description);
    }

    public void StartAssetLoading()
    {
        sceneLoadingView.StartAssetLoading();
    }

    public void StopAssetLoading()
    {
        sceneLoadingView.StopAssetLoading();
    }
}
