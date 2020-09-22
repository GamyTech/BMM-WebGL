using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(ILayoutGroup))]
public class AdaptPagePadding : MonoBehaviour
{
    public bool RespectPageEnablePadding = false;
    public float PCTargetWidth;
    public int MobilePadding;
#if UNITY_STANDALONE || UNITY_WEBGL
    private bool Adaptable = true;
    private RectTransform m_rectTransform;
#endif
    ILayoutGroup m_layoutGroup;

    private void OnEnable()
    {
        m_layoutGroup = this.GetComponent<ILayoutGroup>();
        WidgetController.OnPageWidgetsLoaded += WidgetController_OnPageWidgetsLoaded;
#if UNITY_STANDALONE || UNITY_WEBGL
        m_rectTransform = transform.GetComponent<RectTransform>();
        AdjustLayoutToTargetWidth();

        SettingsController.OnScreenSizeChanged += SettingsController_OnScreenSizeChanged;
#endif
    }


    private void OnDisable()
    {
        WidgetController.OnPageWidgetsLoaded -= WidgetController_OnPageWidgetsLoaded;
#if UNITY_STANDALONE || UNITY_WEBGL
        SettingsController.OnScreenSizeChanged -= SettingsController_OnScreenSizeChanged;
#endif
    }

    public void SetTargetwidth(float Width)
    {
        PCTargetWidth = Width;
        AdjustLayoutToTargetWidth();
    }

    void Update()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        if (Adaptable && PCTargetWidth != (int)m_rectTransform.rect.width)
            AdjustLayoutToTargetWidth();
#endif
    }

    private void AdjustLayoutToTargetWidth()
    {
        float m_BorderSize;
#if UNITY_STANDALONE || UNITY_WEBGL
        if (m_rectTransform.rect.width > PCTargetWidth)
            m_BorderSize = (m_rectTransform.rect.width - PCTargetWidth) / 2.0f;

        else
            m_BorderSize = 0;
#else
        m_BorderSize = MobilePadding;
#endif


        if (m_layoutGroup is DynamicContentLayoutGroup)
        {
            (m_layoutGroup as DynamicContentLayoutGroup).padding.left = (int)(m_BorderSize);
            (m_layoutGroup as DynamicContentLayoutGroup).padding.right = (int)(m_BorderSize);
        }

        else if (m_layoutGroup is LayoutGroup)
        {
            (m_layoutGroup as LayoutGroup).padding.left = (int)(m_BorderSize);
            (m_layoutGroup as LayoutGroup).padding.right = (int)(m_BorderSize);
        }
    }

    private void ResetLayoutPadding()
    {
        if (m_layoutGroup is DynamicContentLayoutGroup)
        {
            (m_layoutGroup as DynamicContentLayoutGroup).padding.left = 0;
            (m_layoutGroup as DynamicContentLayoutGroup).padding.right = 0;
        }

        else if (m_layoutGroup is LayoutGroup)
        {
            (m_layoutGroup as LayoutGroup).padding.left = 0;
            (m_layoutGroup as LayoutGroup).padding.right = 0;
        }
    }

    private void WidgetController_OnPageWidgetsLoaded(Enums.PageId page)
    {
        if (RespectPageEnablePadding && !PageController.Instance.CurrentPage.EnablePadding)
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            Adaptable = false;
#endif
            ResetLayoutPadding();
        }
        else
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            Adaptable = true;
#endif
            AdjustLayoutToTargetWidth();
        }
    }

    private void SettingsController_OnScreenSizeChanged(int width, int height)
    {
        if (RespectPageEnablePadding)
        {
            if (PageController.Instance.CurrentPage.EnablePadding)
                AdjustLayoutToTargetWidth();

            else
                ResetLayoutPadding();
        }
    }

}
