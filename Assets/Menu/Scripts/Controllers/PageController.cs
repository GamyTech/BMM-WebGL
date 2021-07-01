using UnityEngine;
using System.Collections.Generic;
using GT.Assets;

public class PageController : MonoBehaviour
{
    public const string PAGES_RESOURCES_PATH = "SpecificPages/";

    public const string GROUPS_RESOURCES_PATH = "PageGroups/";
    public const string PAGE_POPUP_RESOURCES_PATH = "PagePopups/";

#region Delegates And Events
    public delegate void PageChanged(Page page);
    public static event PageChanged OnPageChanged;
#endregion Delegates And Events

    private static PageController m_instance;
    public static PageController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<PageController>()); }
        private set { m_instance = value; }
    }

    private MobileNavigation m_navigation;
    public MobileNavigation MobileNavigation { get { return m_navigation ?? (m_navigation = FindObjectOfType<MobileNavigation>()); } }

    Dictionary<Enums.PageId, Page> MenuPages;
    Dictionary<Enums.PageId, Enums.PageId> customBackDictionary = new Dictionary<Enums.PageId, Enums.PageId>();

    Dictionary<Enums.PageId, PagePopup> pagePopupDictionary = new Dictionary<Enums.PageId, PagePopup>();

    public Page CurrentPage{ get; protected set; }
    public Enums.PageId CurrentPageId { get { return CurrentPage.ID; } }

    bool changed = false;


#region Init
    void Awake()
    {
        MenuPages = InitPages();
        pagePopupDictionary = InitPagePopups();
    }

    void OnDestroy()
    {
        Instance = null;
    }
    #endregion Init

    void Update()
    {
        if(changed)
        {
            changed = false;
            if (OnPageChanged != null)
                OnPageChanged(CurrentPage);

            FirstTimePopup(CurrentPage.ID);
        }
    }

#region User Methods
    /// <summary>
    /// Custom change page that will the page to custom back transitions.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="backPage"></param>
    public void CustomBackChangePage(Enums.PageId page, Enums.PageId backPage)
    {
        customBackDictionary.AddOrOverrideValue(page, backPage);
        ChangePage(page);
    }

    /// <summary>
    /// Unconditionally change page
    /// </summary>
    /// <param name="page"></param>
    public void ChangePage(Enums.PageId page)
    {
        Debug.Log("<color=blue> ChangePage</color> " + page);
        if (CurrentPage != null && CurrentPage.ID == page)
        {
            Debug.LogWarning("State is already " + CurrentPage.ID);
            return;
        }
        WidgetController.Instance.HideWidgetPopups();

        if (!MenuPages.ContainsKey(page))
        {
            Debug.LogError("PageId " + page + " is missing from pages dictionary");
            return;
        }

        CurrentPage = MenuPages[page];
        changed = true;
    }

    public void ForceReloading()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(CurrentPage);
            changed = false;
        }
    }

    /// <summary>
    /// Change to previous page
    /// </summary>
    public void BackPage()
    {
        Enums.PageId backpage;
        if (customBackDictionary.TryGetValue(CurrentPage.ID, out backpage))
            ChangePage(backpage);
        else
            ChangePage(CurrentPage.BackTransition);
    }
#endregion User Methods

#region Private Methods
    public void FirstTimePopup(Enums.PageId page)
    {
        PagePopup popup;
        if (pagePopupDictionary.TryGetValue(page, out popup))
        {
            string data = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.FirstTimeOnPageData);
            List<object> savedDict;
            if (string.IsNullOrEmpty(data))
                savedDict = new List<object>();
            else
                savedDict = (List<object>)MiniJSON.Json.Deserialize(data);

            if (!savedDict.Contains(page.ToString()))
                PopupController.Instance.ShowSmallPopup(popup.headline, popup.content.ToArray());

            savedDict.Add(page.ToString());
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.FirstTimeOnPageData, MiniJSON.Json.Serialize(savedDict));
        }
    }
#endregion Private Methods

#region Initialization
    private static Dictionary<Enums.PageId, Page> InitPages()
    {
        Dictionary<Enums.PageId, Page> menuPages = new Dictionary<Enums.PageId, Page>();

        PageData[] pages = Resources.LoadAll<PageData>(PAGES_RESOURCES_PATH);


        for (int i = 0; i < pages.Length; i++)
        {
            Page page = CreatePage(pages[i]);

            if (page == null)
                continue;

            if (menuPages.ContainsKey(page.ID))
            {
                Debug.LogWarning("InitPages :: Overriding Key " + page.ID);
                menuPages[page.ID] = page;
            }
            else
                menuPages.Add(page.ID, page);
        }

        return menuPages;
    }

    private static Dictionary<Enums.PageId, PagePopup> InitPagePopups()
    {
        Dictionary<Enums.PageId, PagePopup> dict = new Dictionary<Enums.PageId, PagePopup>();
        PagePopup[] pagePopups = Resources.LoadAll<PagePopup>(PAGE_POPUP_RESOURCES_PATH);
        for (int i = 0; i < pagePopups.Length; i++)
        {
            Enums.PageId id;
            if(Utils.TryParseEnum(pagePopups[i].pageId, out id))
                dict.AddOrOverrideValue(id, pagePopups[i]);
        }
        return dict;
    }

    private static Page CreatePage(PageData pageData)
    {
        if (pageData == null)
            return null;

        Enums.PageId pageId;
        if (Utils.TryParseEnum(pageData.PageId, out pageId) == false)
        {
            Debug.LogWarning("Failed parsing PageId " + pageData.PageId);
            return null;
        }

        Enums.StoreType storeType;
        Utils.TryParseEnum(pageData.StoreType, out storeType);

        Enums.WidgetId tempWidget;
        List<Enums.WidgetId> tempFloatingTopWidgets = new List<Enums.WidgetId>();
        for (int j = 0; j < pageData.FloatingTopWidgets.Count; j++)
            if (Utils.TryParseEnum(pageData.FloatingTopWidgets[j], out tempWidget))
                tempFloatingTopWidgets.Add(tempWidget);

        List<Enums.WidgetId> tempTopWidgets = new List<Enums.WidgetId>();
        for (int j = 0; j < pageData.TopWidgets.Count; j++)
            if (Utils.TryParseEnum(pageData.TopWidgets[j], out tempWidget))
                tempTopWidgets.Add(tempWidget);

        List<Enums.WidgetId> tempMiddleWidgets = new List<Enums.WidgetId>();
        for (int j = 0; j < pageData.MiddleWidgets.Count; j++)
            if (Utils.TryParseEnum(pageData.MiddleWidgets[j], out tempWidget))
                tempMiddleWidgets.Add(tempWidget);

        List<Enums.WidgetId> tempBottomWidgets = new List<Enums.WidgetId>();
        for (int j = 0; j < pageData.BottomWidgets.Count; j++)
            if (Utils.TryParseEnum(pageData.BottomWidgets[j], out tempWidget))
                tempBottomWidgets.Add(tempWidget);

        List<Enums.WidgetId> tempFloatingBottomWidgets = new List<Enums.WidgetId>();
        for (int j = 0; j < pageData.FloatingBottomWidgets.Count; j++)
            if (Utils.TryParseEnum(pageData.FloatingBottomWidgets[j], out tempWidget))
                tempFloatingBottomWidgets.Add(tempWidget);

        Enums.NavigationState tempNavState;
        Utils.TryParseEnum(pageData.NavigationState, out tempNavState);

        Enums.NavigationCategory tempNavCat;
        Utils.TryParseEnum(pageData.NavigationCategory, out tempNavCat);

        Enums.PageId tempBackTransition;
        Utils.TryParseEnum(pageData.BackTransitionPageId, out tempBackTransition);

        Page page = new Page (pageId, storeType, pageData.Title, tempFloatingTopWidgets, tempTopWidgets, tempMiddleWidgets, tempBottomWidgets,
            tempFloatingBottomWidgets, tempNavState, tempNavCat, tempBackTransition, pageData.IsScrollable, pageData.NoMiddlePadding, pageData.AcessPCQuickNavigation,
            pageData.EnablePadding,  pageData.IsSizeToFitContent, pageData.MidWidgetsPosition );

        return page;
    }
#endregion Initialization
}
