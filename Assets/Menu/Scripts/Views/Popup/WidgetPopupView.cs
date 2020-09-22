using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetPopupView : MonoBehaviour
{
    public FadingElement FadingElement;

    public WidgetContainer widgetContainer;
    private List<Widget> activeWidgets = new List<Widget>();
    private VerticalLayoutGroup layoutGroup;

    public void ShowPopup(Widget widget, TextAnchor position, bool clearOthers = true)
    {
        if (clearOthers) Clear();
        widgetContainer.AddWidget(widget);
        widget.EnableWidget();
        activeWidgets.Add(widget);
        if (layoutGroup == null)
            layoutGroup = widgetContainer.contentRectTransform.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = position;
#if UNITY_STANDALONE || UNITY_WEBGL
        layoutGroup.padding.top = 140;
#endif

        FadingElement.FadeIn(false, () => OnPopupsShown(widget));
    }

    private void OnPopupsShown(Widget widget)
    {
        if (widget is PopupWidget)
            (widget as PopupWidget).OnPopupWidgetShown();
    }

    public void HidePopups(bool instant)
    {
        Clear();
        FadingElement.FadeOut(instant);
    }

    public void HidePopup(Widget widget, bool instant = false)
    {
        if (widget is PopupWidget && !(widget as PopupWidget).IsClosable) return;
        widgetContainer.DestroyWidget(widget);
        activeWidgets.Remove(widget);
        if (activeWidgets.Count == 0)
            FadingElement.FadeOut(instant);
    }

    private void Clear()
    {
        activeWidgets.Clear();
        widgetContainer.DestroyAllWidgets();
    }

    public void HidePopups()
    {
        HidePopups(false);
    }

    public void HidePopup()
    {
        if (activeWidgets.Count > 0)
            HidePopup(activeWidgets[activeWidgets.Count - 1]);
    }
}
