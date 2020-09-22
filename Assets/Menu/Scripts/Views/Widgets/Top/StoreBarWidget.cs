using UnityEngine.UI;
using System.Collections.Generic;
using GT.Store;
using UnityEngine.EventSystems;
using UnityEngine;

public class StoreBarWidget : Widget
{
    #region Movement
    public float smoothTime = 0.1f;
    private float velocity = 0;
    private bool isDragging = false;
    #endregion Movement

    public ScrollRect scrollRect;
    public LoopingHorizontalDynamicContentLayoutGroup content;
    public ToggleGroup group;

    private int defaultIndex = 0;
    private int currentIndex = -1;
    private const Enums.StoreType defaultType = Enums.StoreType.Loyalty;
    private Dictionary<Enums.StoreType, int> IndexOfStoreTabs;

    public override void EnableWidget()
    {
        UserController.Instance.gtUser.StoresData.OnStoresUpdated += GtUser_OnStoresChanged;
        PageController.OnPageChanged += PageController_OnPageChanged;

        base.EnableWidget();

        if (UserController.Instance.gtUser.StoresData == null)
        {
            LoadingController.Instance.ShowPageLoading();
            gameObject.SetActive(false);
        }
        else
        {
            RefreshItems(UserController.Instance.gtUser.StoresData);
            if (content.IsScrollable())
                MoveToPage(defaultIndex);
        }
    }

    public override void DisableWidget()
    {
        content.ClearElements();

        UserController.Instance.gtUser.StoresData.OnStoresUpdated -= GtUser_OnStoresChanged;
        PageController.OnPageChanged -= PageController_OnPageChanged;

        base.DisableWidget();
    }

    private void RefreshItems(Stores stores)
    {
        if (stores == null)
            return;

        IndexOfStoreTabs = new Dictionary<Enums.StoreType, int>();
        List<Enums.StoreType> storeTypes = stores.GetExistingStoresTypes();
        for (int i = 0; i < storeTypes.Count; i++)
        {
            content.AddElement(new StoreListItem(i, group, stores.GetStore(storeTypes[i]), OnToggleChanged));
            IndexOfStoreTabs.Add(storeTypes[i], i);
            if (storeTypes[i] == defaultType)
                defaultIndex = i;
        }
    }

    private void OnToggleChanged(int index)
    {
        currentIndex = index;
    }

    #region Events
    private void GtUser_OnStoresChanged(Stores stores)
    {
        LoadingController.Instance.HidePageLoading();
        RefreshItems(stores);
    }

    private void PageController_OnPageChanged(Page page)
    {
        int pageIndex;
        if (content.IsScrollable() && IndexOfStoreTabs.TryGetValue(page.StoreType, out pageIndex))
            MoveToPage(pageIndex);
    }
    #endregion Events

    #region Scroll
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (content.IsScrollable() && !isDragging)
            MoveToPage(currentIndex);
    }

    void MoveToPage(int page)
    {
        currentIndex = page;

        Vector3 pos = scrollRect.content.transform.localPosition;

        float targetPosition = -content.GetPagePosition(page, 1).x;
        if (Utils.Approximately(targetPosition, pos.x, .0001f))
            return;

        pos.x = Mathf.SmoothDamp(pos.x, targetPosition, ref velocity, smoothTime);
        scrollRect.content.transform.localPosition = pos;
        (content.elementsList[currentIndex] as StoreListItem).View.GetComponent<GTToggle>().isOn = true;
    }

    public void OnEndDrag(BaseEventData eventData)
    {
        isDragging = false;

        if (content.IsScrollable() && content.GetMidPageIndex() != currentIndex)
        {
            currentIndex = content.GetMidPageIndex();
            (content.elementsList[currentIndex] as StoreListItem).View.GetComponent<GTToggle>().isOn = true;
        }
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
        isDragging = true;
    }
    #endregion
}
