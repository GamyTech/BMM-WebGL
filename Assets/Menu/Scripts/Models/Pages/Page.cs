using System.Collections.Generic;

public class Page
{
    public Enums.PageId ID { get; protected set; }
    public Enums.StoreType StoreType { get; protected set; }
    public string Title { get; protected set; }
    public List<Enums.WidgetId> MidWidgets { get; protected set; }
    public Enums.NavigationState NavState { get; protected set; }
    public Enums.NavigationCategory NavCat { get; protected set; }
    public List<Enums.WidgetId> FloatingTopWidgets { get; protected set; }
    public List<Enums.WidgetId> TopWidgets { get; protected set; }
    public List<Enums.WidgetId> BottomWidgets { get; protected set; }
    public List<Enums.WidgetId> FloatingBottomWidgets { get; protected set; }
    public Enums.PageId BackTransition { get; protected set; }
    public bool Scrollable { get; protected set; }
    public bool NoMiddlePadding { get; protected set; }
    public bool PCNav { get; protected set; }
    public bool EnablePadding { get; protected set; }
    public float MidWidgetsPosition { get; protected set; }
    public bool IsSizeToFitContent { get; protected set; }

    public Page ( Enums.PageId id, Enums.StoreType storeType, string title, List<Enums.WidgetId> floatingTopWidgets, List<Enums.WidgetId> topWidgets, 
        List<Enums.WidgetId> widgets, List<Enums.WidgetId> bottomWidgets, List<Enums.WidgetId> floatingBottomWidgets, Enums.NavigationState navState, 
        Enums.NavigationCategory navCat, Enums.PageId backTransition, bool isScrollable, bool noMiddlePadding,
        bool AcessPCQuickNavigation, bool enablePadding, bool isSizeToFitContent, float midWidgetsPosition )
    {
        ID = id;
        StoreType = storeType;
        Title = title;
        FloatingTopWidgets = floatingTopWidgets;
        TopWidgets = topWidgets;
        MidWidgets = widgets;
        BottomWidgets = bottomWidgets;
        FloatingBottomWidgets = floatingBottomWidgets;
        NavState = navState;
        NavCat = navCat;
        BackTransition = backTransition;
        Scrollable = isScrollable;
        NoMiddlePadding = noMiddlePadding;
        EnablePadding = enablePadding;
        PCNav = AcessPCQuickNavigation;
        IsSizeToFitContent = isSizeToFitContent;
        MidWidgetsPosition = midWidgetsPosition;
    }

    public override string ToString()
    {
        return ID.ToString();
    }
}
