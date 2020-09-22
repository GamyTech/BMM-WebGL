using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Widget : UIBehaviour, IResizable
{
    public event ActiveStateAndSizeChanged OnActiveStateAndSizeChanged;

    public Selectable FirstSelectable;

    public string widgetId;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (OnActiveStateAndSizeChanged != null)
            OnActiveStateAndSizeChanged();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (OnActiveStateAndSizeChanged != null)
            OnActiveStateAndSizeChanged();
    }

    /// <summary>
    /// Called by widget controller when widget is enabled
    /// </summary>
    public virtual void EnableWidget()
    {
        gameObject.SetActive(true);
        if (OnActiveStateAndSizeChanged != null)
            OnActiveStateAndSizeChanged();

        gameObject.ResetGameObject();

#if UNITY_STANDALONE
        if (FirstSelectable != null)
            FirstSelectable.Select();
#endif
    }

    /// <summary>
    /// Called by widget controller when widget is disabled
    /// </summary>
    public virtual void DisableWidget()
    {
        FreeResources();
        if (OnActiveStateAndSizeChanged != null)
            OnActiveStateAndSizeChanged();
    }

    /// <summary>
    /// Called by widget controller when widget stays on a new page
    /// </summary>
    public virtual void RefreshWidget()
    {

    }

    /// <summary>
    /// Called by widget controller when widget about to be disabled or destroyed
    /// </summary>
    protected virtual void FreeResources()
    {

    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (OnActiveStateAndSizeChanged != null)
            OnActiveStateAndSizeChanged();
    }


    #region Overrides
    public override string ToString()
    {
        return widgetId.ToString();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(Widget w)
    {
        return widgetId.Equals(w.widgetId);
    }

    public bool Equals(Enums.WidgetId id)
    {
        return widgetId.Equals(id.ToString());
    }

    public bool Equals(string id)
    {
        return widgetId.Equals(id);
    }

    public override bool Equals(object o)
    {
        if (o is Widget)
        {
            return Equals((Widget)o);
        }
        else if(o is Enums.WidgetId)
        {
            return Equals((Enums.WidgetId)o);
        }
        else if(o is string)
        {
            return Equals(o.ToString());
        }
        else return false;
    }
    #endregion Overrides

    #region Create Default Widget
    public static Widget CreateDefaultWidget(Enums.WidgetId widgetId)
    {
        GameObject prefab = new GameObject("Missing_" + widgetId);
        DefaultWidget widget = prefab.AddComponent<DefaultWidget>();
        Text text = prefab.AddComponent<Text>();
        SetDefaultTextSettings(text);
        prefab.AddComponent<VerticalLayoutGroup>();
        widget.widgetId = widgetId.ToString();

        GameObject panel = new GameObject("Panel");
        LayoutElement childElement = panel.AddComponent<LayoutElement>();
        childElement.preferredHeight = 100;
        panel.transform.SetParent(prefab.transform);
        Image image = panel.AddComponent<Image>();
        image.color = new Color(1, 0, 0, .5f);

        RectTransform t = image.rectTransform;
        RectTransform pt = prefab.GetComponent<RectTransform>();

        Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                            t.anchorMax.y + t.offsetMax.y / pt.rect.height);

        t.anchorMin = newAnchorsMin;
        t.anchorMax = newAnchorsMax;
        t.offsetMin = t.offsetMax = new Vector2(0, 0);
        return widget;
    }

    private static void SetDefaultTextSettings(Text text)
    {
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.fontSize = 34;
        text.alignment = TextAnchor.MiddleCenter;
    }
    #endregion Create Default Widget
}
