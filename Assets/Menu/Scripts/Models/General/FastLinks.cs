using UnityEngine;

public class FastLinks : MonoBehaviour {
#if UNITY_EDITOR
    public Transform FloatingTopWidget;
    public Transform TopWidget;
    public Transform MiddleWidget;
    public Transform BottomWidget;
    public Transform FloatingBottomWidget;
    public Transform WidgetPopup;

    private Transform FloatingTopContainer;
    private Transform TopContainer;
    private Transform MiddleContainer;
    private Transform BottomContainer;
    private Transform FloatingBottomContainer;
    private Transform WidgetPopupContainer;

    private void OnEnable()
    {
        FloatingTopContainer = FloatingTopWidget;
        TopContainer = TopWidget;
        MiddleContainer = MiddleWidget;
        BottomContainer = BottomWidget;
        FloatingBottomContainer = FloatingBottomWidget;
        WidgetPopupContainer = WidgetPopup;

        WidgetController.OnPageWidgetsLoaded += WidgetController_OnPageWidgetsLoaded;
    }

    private void OnDisable()
    {
        WidgetController.OnPageWidgetsLoaded -= WidgetController_OnPageWidgetsLoaded;
    }

    private void WidgetController_OnPageWidgetsLoaded(Enums.PageId pageId)
    {
        FloatingTopWidget = SelectTargetFrom(FloatingTopContainer);
        TopWidget = SelectTargetFrom(TopContainer);
        MiddleWidget = SelectTargetFrom(MiddleContainer);
        BottomWidget = SelectTargetFrom(BottomContainer);
        FloatingBottomWidget = SelectTargetFrom(FloatingBottomContainer);
        WidgetPopup = SelectTargetFrom(WidgetPopupContainer);
    }

    private Transform SelectTargetFrom(Transform container)
    {
        return container.childCount > 0 ? container.GetChild(0) : container;
    }

#endif
}
