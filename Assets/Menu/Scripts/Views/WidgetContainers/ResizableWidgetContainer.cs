using UnityEngine;

public class ResizableWidgetContainer : WidgetContainer
{
    public RectTransform ContentTransform;

    public delegate void Resize(float height);
    public event Resize OnResize = delegate { };

    private float height = 0;

    private bool sizeChanged = false;

    public override RectTransform contentRectTransform
    {
        get
        {
            return ContentTransform;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateSize();
    }

    void LateUpdate()
    {
        if (sizeChanged)
        {
            sizeChanged = false;
            UpdateSize();
        }
    }

    public override void AddWidget(Widget widget, int siblingIndex)
    {
        base.AddWidget(widget, siblingIndex);
        widget.OnActiveStateAndSizeChanged += OnSizeChanged;
        SetDirty();
    }

    public override void RemoveWidget(Widget widget)
    {
        widget.OnActiveStateAndSizeChanged -= OnSizeChanged;
        base.RemoveWidget(widget);
        SetDirty();
    }

    public override void DestroyAllWidgets()
    {
        for (int i = 0; i < ActiveWidgets.Count; i++)
            ActiveWidgets[i].OnActiveStateAndSizeChanged -= OnSizeChanged;

        base.DestroyAllWidgets();
        SetDirty();
    }

    private void UpdateSize()
    {
        height = 0;
        for (int i = 0; i < contentRectTransform.childCount; i++)
        {
            RectTransform child = contentRectTransform.GetChild(i) as RectTransform;
            if (child != null && child.gameObject.activeSelf)
                height += child.sizeDelta.y;
        }
        layoutElement.minHeight = height;

        if (OnResize != null)
            OnResize(height);
    }

    protected void SetDirty()
    {
        sizeChanged = true;
    }

    public void OnSizeChanged()
    {
        SetDirty();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
