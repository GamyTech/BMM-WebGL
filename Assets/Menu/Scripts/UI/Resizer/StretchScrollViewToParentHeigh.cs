using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class StretchScrollViewToParentHeigh : MonoBehaviour
{
    public ScrollRect scrollRectToStretch;

    private LayoutElement layoutToResize;
    private RectTransform scrollTransform;
    private RectTransform contentTransform;

    private LayoutGroup layoutGroup;
    private RectTransform rectTransform;
    private LayoutGroup containerLayout;
    private RectTransform containerTransform;

    private void Start()
    {
        layoutToResize = scrollRectToStretch.GetComponent<LayoutElement>();
        scrollTransform = scrollRectToStretch.GetComponent<RectTransform>();
        contentTransform = scrollRectToStretch.content.GetComponent<RectTransform>();

        layoutGroup = GetComponent<LayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (layoutToResize == null)
            Debug.LogError("StretchToParentHeight there's no LayoutElement on the given ScrollRect");
    }

    private void Update()
    {
        Resize();
    }

    private void Resize()
    {
        if (containerTransform == null)
        {
            containerLayout = transform.parent.GetComponent<LayoutGroup>();
            containerTransform = transform.parent.parent.GetComponent<RectTransform>();
            if (containerTransform == null) return;
        }
        if (containerTransform == null || layoutGroup == null || layoutToResize == null) return;
        float height = containerTransform.rect.height - rectTransform.rect.height + scrollTransform.rect.height - 
            (containerLayout != null ? containerLayout.padding.top + containerLayout.padding.bottom : 0);

        if (contentTransform.rect.height < height)
        {
            layoutToResize.preferredHeight = contentTransform.rect.height;
            if (scrollRectToStretch.vertical)
            {
                scrollRectToStretch.vertical = false;
            }
        }
        else if (layoutToResize.preferredHeight != height)
        {
            layoutToResize.preferredHeight = height;
            scrollRectToStretch.vertical = true;
        }
    }
}
