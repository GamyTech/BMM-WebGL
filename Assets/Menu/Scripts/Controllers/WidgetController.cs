using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class WidgetController : MonoBehaviour
{
    public delegate void WidgetControllerInitialized();
    public delegate void PageWidgetsLoadedHandler(Enums.PageId pageId);
    public static event PageWidgetsLoadedHandler OnPageWidgetsLoaded;

    private static WidgetController m_instance;
    public static WidgetController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<WidgetController>()); }
        private set { m_instance = value; }
    }

    public ResizableWidgetContainer floatingTopWidgetContainer;
    public ResizableWidgetContainer topWidgetContainer;
    public ScrollableWidgetContainer middleWidgetContainer;
    public ResizableWidgetContainer bottomWidgetContainer;
    public ResizableWidgetContainer floatingBottomWidgetContainer;

    public WidgetPopupView WidgetPopupView;


    List<Widget> ActivatedFloatingTopWidgets = new List<Widget>();
    List<Widget> ActivatedTopWidgets = new List<Widget>();
    List<Widget> ActivatedMiddleWidgets = new List<Widget>();
    List<Widget> ActivatedBottomWidgets = new List<Widget>();
    List<Widget> ActivatedFloatingBottomWidgets = new List<Widget>();


    #region Unity Functions
    void Start()
    {
        floatingTopWidgetContainer.OnResize += middleWidgetContainer.OnFloatingTopResized;

        WidgetPopupView.gameObject.SetActive(true);
        WidgetPopupView.HidePopups(true);
    }

    void OnEnable()
    {
        Instance = this;
        PageController.OnPageChanged += PageController_OnPageChanged;
    }

    void OnDisable()
    {
        PageController.OnPageChanged -= PageController_OnPageChanged;
        DisableOnDestroy();
        Instance = null;
    }

    void OnDestroy()
    {
        PageController.OnPageChanged -= PageController_OnPageChanged;
        DisableOnDestroy();
        Instance = null;
    }
    #endregion Unity Functions

    #region Loading Widget Prefabs
    /// <summary>
    /// Init Prefabs from prefab folder
    /// </summary>

    #endregion Loading Widget Prefabs

    #region Enable/Disable Widgets
    private void EnableWidget(List<Widget> activeList, Widget widget, int atIndex)
    {
        activeList.Insert(atIndex, widget);
        widget.EnableWidget();
    }

    private void DisableWidget(List<Widget> activeList, Widget widget)
    {
        activeList.Remove(widget);
        widget.DisableWidget();
        FreeWidgetGameobject(widget);
    }

    private void DisableOnDestroy()
    {
        floatingTopWidgetContainer.OnResize -= middleWidgetContainer.OnFloatingTopResized;

        DisableWidgets(ActivatedFloatingTopWidgets);
        DisableWidgets(ActivatedTopWidgets);
        DisableWidgets(ActivatedMiddleWidgets);
        DisableWidgets(ActivatedBottomWidgets);

        floatingTopWidgetContainer.DestroyAllWidgets();
        topWidgetContainer.DestroyAllWidgets();
        middleWidgetContainer.DestroyAllWidgets();
        bottomWidgetContainer.DestroyAllWidgets();
        floatingBottomWidgetContainer.DestroyAllWidgets();
    }

    private void DisableWidgets(List<Widget> activatedWidgets)
    {
        for (int i = 0; i < activatedWidgets.Count; i++)
        {
            if (activatedWidgets[i] == null) continue;
            try
            {
                activatedWidgets[i].DisableWidget();
                FreeWidgetGameobject(activatedWidgets[i]);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
        }
        activatedWidgets.Clear();
    }

    private void FreeWidgetGameobject(Widget widget)
    {
        Destroy(widget.gameObject);
        //if (PooledObjectsContainer.Instance != null)
        //    widget.transform.SetParent(PooledObjectsContainer.Instance.transform);
    }
    #endregion Enable/Disable Widgets

    #region Main Methods
    private void ChangeWidgets(Enums.WidgetPosition widgetPosition, WidgetContainer container, List<Widget> activeList, List<Enums.WidgetId> widgets)
    {
        List<Widget> widgetsToRemove = new List<Widget>();
        for (int i = 0; i < activeList.Count; i++)
        {
            Enums.WidgetId widgetId;
            if (Utils.TryParseEnum(activeList[i].widgetId, out widgetId) && !widgets.Contains(widgetId))
                widgetsToRemove.Add(activeList[i]);
        }

        for (int i = 0; i < widgetsToRemove.Count; i++)
        {
            container.RemoveWidget(widgetsToRemove[i]);
            DisableWidget(activeList, widgetsToRemove[i]);
        }

        // Add widgets
        for (int i = 0; i < widgets.Count; i++)
        {
            if (activeList.Contains(widgets[i]) == false)
            {
                Widget widget = AssetController.Instance.GetWidget(widgetPosition, widgets[i]);
                container.AddWidget(widget, i);
                EnableWidget(activeList, widget, i);
            }
            else
                activeList[i].RefreshWidget();
        }
    }

    private void SetPageWidgets(
        Enums.PageId pageId,
        List<Enums.WidgetId> floatingTopWidgets,
        List<Enums.WidgetId> topWidgets,
        List<Enums.WidgetId> middleWidgets,
        List<Enums.WidgetId> bottomWidgets,
        List<Enums.WidgetId> floatingBottomWidgets,
        Enums.NavigationState navState,
        bool isScrollable,
        bool noMiddlePadding,
        bool isSizeToFitContent,
        float midWidgetsPosition)
    {
        //Debug.Log("SetPageWidgets | MiddleWidgets: " + middleWidgets.Display() + " | TopPageWidget: " + topWidgets.Display() +
        //    " | BottomPageWidget: " + bottomWidgets.Display() + " | navState: " + navState);

        // ------------------ Floating Top Widgets ------------------
        ChangeWidgets(Enums.WidgetPosition.FloatingTop, floatingTopWidgetContainer, ActivatedFloatingTopWidgets, floatingTopWidgets);
        // ------------------ Floating Top Widgets ------------------

        // ------------------ Top Widgets ------------------
        ChangeWidgets(Enums.WidgetPosition.Top, topWidgetContainer, ActivatedTopWidgets, topWidgets);
        // ------------------ Top Widgets ------------------


        // ------------------ Middle Widgets ------------------
        middleWidgetContainer.DisableTopPadding(noMiddlePadding);
        middleWidgetContainer.SetScrollable(isScrollable);
        middleWidgetContainer.SetSizeToFitContent(isSizeToFitContent);
        middleWidgetContainer.SetMidWidgetsPosition(midWidgetsPosition);

        ChangeWidgets(Enums.WidgetPosition.Middle, middleWidgetContainer, ActivatedMiddleWidgets, middleWidgets);
        // ------------------ Middle Widgets ------------------


        // ------------------ Bottom Widgets ------------------
        ChangeWidgets(Enums.WidgetPosition.Bottom, bottomWidgetContainer, ActivatedBottomWidgets, bottomWidgets);
        // ------------------ Bottom Widgets ------------------

        // ------------------ Floating Bottom Widgets ------------------
        ChangeWidgets(Enums.WidgetPosition.FloatingBottom, floatingBottomWidgetContainer, ActivatedFloatingBottomWidgets, floatingBottomWidgets);
        // ------------------ Floating Bottom Widgets ------------------

        if (OnPageWidgetsLoaded != null)
            OnPageWidgetsLoaded(pageId);
    }
    #endregion Main Methods

    #region Events Handling
    private void PageController_OnPageChanged(Page page)
    {
        Debug.Log("Change page " + page.ID);
        SetPageWidgets(page.ID, page.FloatingTopWidgets, page.TopWidgets, page.MidWidgets, page.BottomWidgets, page.FloatingBottomWidgets, page.NavState, page.Scrollable, page.NoMiddlePadding, page.IsSizeToFitContent, page.MidWidgetsPosition);
    }
    #endregion Events Handling

    #region Widget Popup Methods
    public void ShowWidgetPopup(Enums.WidgetId widgetId, TextAnchor position = TextAnchor.UpperCenter, bool clearOthers = true, object data = null, bool closable = true)
    {
        Widget widget = AssetController.Instance.GetWidget(Enums.WidgetPosition.Middle, widgetId);
        if (widget is PopupWidget)
            (widget as PopupWidget).Init(closable, data);
        WidgetPopupView.ShowPopup(widget, position, clearOthers);
    }
    public void ShowWidgetPopup(Enums.WidgetId widgetId, bool clearOthers)
    {
        ShowWidgetPopup(widgetId, TextAnchor.UpperCenter, clearOthers);
    }
    public void ShowWidgetPopup(Enums.WidgetId widgetId, object data)
    {
        ShowWidgetPopup(widgetId, TextAnchor.UpperCenter, true, data);
    }
    public void ShowWidgetPopup(Enums.WidgetId widgetId, object data, bool clearOthers)
    {
        ShowWidgetPopup(widgetId, TextAnchor.UpperCenter, clearOthers, data);
    }
    public void ShowWidgetPopup(Enums.WidgetId widgetId, object data, bool clearOthers, bool closable)
    {
        ShowWidgetPopup(widgetId, TextAnchor.UpperCenter, clearOthers, data, closable);
    }

    public void HideWidgetPopups()
    {
        WidgetPopupView.HidePopups();
    }

    public void HideWidgetPopup(Widget widget)
    {
        WidgetPopupView.HidePopup(widget);
    }
    #endregion Widget Popup Methods
}
