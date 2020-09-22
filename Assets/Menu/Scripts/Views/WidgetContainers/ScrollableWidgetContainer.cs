using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollableWidgetContainer : WidgetContainer
{
    public RectTransform ContentTransform;

    [NonSerialized]
    private ScrollRect m_scrollRect;
    private int StartingTopPadding = 0;
    private bool NoTopPadding = true;
    private int FloatingTopSize;

    private VerticalLayoutGroup m_layout;
    private VerticalLayoutGroup Layout { get { return m_layout ?? (m_layout = scrollRect.content.GetComponent<VerticalLayoutGroup>()); } }

    protected ScrollRect scrollRect
    {
        get
        {
            if(m_scrollRect == null)
                m_scrollRect = GetComponent<ScrollRect>();
            return m_scrollRect;
        }
    }

    public override RectTransform contentRectTransform
    {
        get
        {
            return ContentTransform;
        }
    }

    protected override void Start()
    {
        StartingTopPadding = scrollRect.content.GetComponent<VerticalLayoutGroup>().padding.top;
    }

    public void SetScrollable(bool isScrollable)
    {
        scrollRect.enabled = isScrollable;
    }

    public void SetMidWidgetsPosition(float midWidgetsPosition)
    {
        Vector2 pivot = contentRectTransform.pivot;
        pivot.y = midWidgetsPosition;
        contentRectTransform.pivot = pivot;
    }

    public void DisableTopPadding(bool noTopPadding)
    {
        NoTopPadding = noTopPadding;
        Layout.padding.top = NoTopPadding ? 0 : StartingTopPadding + FloatingTopSize;
    }

    internal void OnFloatingTopResized(float height)
    {
        FloatingTopSize = (int)height;
        Layout.padding.top = NoTopPadding? 0 : StartingTopPadding + FloatingTopSize;
    }
}
