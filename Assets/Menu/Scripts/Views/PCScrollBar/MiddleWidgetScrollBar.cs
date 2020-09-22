using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MiddleWidgetScrollBar : MonoBehaviour
{
    public static MiddleWidgetScrollBar Instance;
    public Scrollbar m_ScrollBar;
    public ScrollRect InitialScrollRect;
    private ScrollRect m_currentScrollRect;


    void Start()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        Instance = this;
        m_currentScrollRect = InitialScrollRect;
#else
        Destroy(this.gameObject);
#endif

    }

    public void OnEnable()
    {
        WidgetController.OnPageWidgetsLoaded += PageController_OnPageWidgetsLoaded;
    }

    public void OnDisable()
    {
        WidgetController.OnPageWidgetsLoaded -= PageController_OnPageWidgetsLoaded;
    }

    public void SetNewScrollRect(ScrollRect newScrollRect)
    {
        if (m_currentScrollRect != null)
            m_currentScrollRect.verticalScrollbar = null;

        m_currentScrollRect = newScrollRect;
        StartCoroutine(RefreshNecessary());
    }

    private IEnumerator RefreshNecessary()
    {
        m_ScrollBar.value = 1.0f;
        bool necessary = true;

        if (PageController.Instance.CurrentPage.Scrollable)
        {
            Canvas.ForceUpdateCanvases();
            yield return null;
            if (m_currentScrollRect != null)
                necessary = m_currentScrollRect.content.rect.height > m_currentScrollRect.viewport.rect.height;
        }

        else
            necessary = false;

        if (m_currentScrollRect != null)
            m_currentScrollRect.verticalScrollbar = necessary ? m_ScrollBar : null;
        m_ScrollBar.gameObject.SetActive(necessary);

    }

    private void PageController_OnPageWidgetsLoaded(Enums.PageId pageID)
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        if (PageController.Instance.CurrentPage.Scrollable && PageController.Instance.CurrentPage.EnablePadding)
                SetNewScrollRect(InitialScrollRect);
        StartCoroutine(RefreshNecessary());
#endif

    }
}
