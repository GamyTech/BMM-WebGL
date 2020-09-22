using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LayoutElement)), ExecuteInEditMode, DisallowMultipleComponent]
public abstract class WidgetContainer : UIBehaviour
{
    protected List<Widget> ActiveWidgets = new List<Widget>();

    [NonSerialized]
    private LayoutElement m_layoutElement;
    [NonSerialized]
    private RectTransform m_contentRectTransform;
    [NonSerialized]
    private ContentSizeFitter m_sizeContentFitter;

    protected LayoutElement layoutElement
    {
        get
        {
            if (m_layoutElement == null)
                m_layoutElement = GetComponent<LayoutElement>();
            return m_layoutElement;
        }
    }

    public abstract RectTransform contentRectTransform
    {
        get;
    }

    protected ContentSizeFitter sizeContententFitter
    {
        get
        {
            if (m_sizeContentFitter == null)
                m_sizeContentFitter = contentRectTransform.GetComponent<ContentSizeFitter>();
            return m_sizeContentFitter;
        }
    }

    public virtual void AddWidget(Widget widget)
    {
        ActiveWidgets.Add(widget);
        widget.gameObject.InitGameObjectAfterInstantiation(contentRectTransform);
    }

    public virtual void AddWidget(Widget widget, int siblingIndex)
    {
        AddWidget(widget);
        widget.transform.SetSiblingIndex(siblingIndex);
    }

    public virtual void RemoveWidget(Widget widget)
    {
        ActiveWidgets.Remove(widget);
    }

    public virtual void DestroyWidget(Widget widget)
    {
        widget.DisableWidget();
        Destroy(widget.gameObject);
        ActiveWidgets.Remove(widget);
    }

    public virtual void DestroyAllWidgets()
    {
        for (int i = 0; i < ActiveWidgets.Count; i++)
        {
            ActiveWidgets[i].DisableWidget();
            Destroy(ActiveWidgets[i].gameObject);
            ActiveWidgets.Remove(ActiveWidgets[i]);
        }
    }

    public void SetSizeToFitContent(bool isSizeToFitContent)
    {
        contentRectTransform.anchorMin = new Vector2(0, 0);
        contentRectTransform.anchorMax = new Vector2(1, 1);
        contentRectTransform.offsetMin = contentRectTransform.offsetMax = new Vector2(0, 0);
        sizeContententFitter.enabled = isSizeToFitContent;
    }
}
