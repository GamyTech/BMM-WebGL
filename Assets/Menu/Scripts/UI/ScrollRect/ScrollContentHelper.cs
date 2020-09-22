using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScrollContentHelper : MonoBehaviour
{
    public float verticalReset = 1;
    public float horizontalReset = 0;

    private ScrollRect m_scrollRect;
    private RectTransform m_rectTransform;

    public ScrollRect scrollRect
    {
        get
        {
            if (m_scrollRect == null)
                m_scrollRect = GetComponent<ScrollRect>();
            return m_scrollRect;
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            if (m_rectTransform == null)
                m_rectTransform = scrollRect.content;
            return m_rectTransform;
        }
    }

    float lastWidth = 0;
    float lastHeight = 0;

    void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        scrollRect.verticalNormalizedPosition = verticalReset;
        scrollRect.horizontalNormalizedPosition = horizontalReset;
    }

    void Update()
    {
        if (rectTransform.sizeDelta.x != lastWidth)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition);
            lastWidth = rectTransform.sizeDelta.x;
        }

        if (rectTransform.sizeDelta.y != lastHeight)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            lastHeight = rectTransform.sizeDelta.y;
        }
    }
}
