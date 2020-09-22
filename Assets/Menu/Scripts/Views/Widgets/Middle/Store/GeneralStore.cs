using System.Collections.Generic;
using UnityEngine.UI;
using GT.Store;
using UnityEngine;

public class GeneralStore : Widget
{
    public ScrollContentHelper scrollHelper;
    public ScrollRect scrollRect;
    public GridDynamicContentLayoutGroup content;

    public override void EnableWidget()
    {
        base.EnableWidget();

        UserController.Instance.gtUser.StoresData.OnStoresUpdated += GtUser_OnStoresChanged;
        RefreshItems(UserController.Instance.gtUser.StoresData);

#if UNITY_STANDALONE || UNITY_WEBGL
        content.constraintCount = 3;
        MiddleWidgetScrollBar.Instance.SetNewScrollRect(scrollRect);
#endif
    }

    public override void DisableWidget()
    {
        UserController.Instance.gtUser.StoresData.OnStoresUpdated -= GtUser_OnStoresChanged;

        base.DisableWidget();
    }

    public override void RefreshWidget()
    {
        base.RefreshWidget();
        RefreshItems(UserController.Instance.gtUser.StoresData);
        scrollRect.verticalNormalizedPosition = 1;
    }

    private void RefreshItems(Stores stores)
    {
        if (stores == null)
            return;

        Store store = stores.GetStoreFromPage(PageController.Instance.CurrentPage);
        if (store != null)
            content.SetElements(new List<IDynamicElement>(store.items.ToArray()));
    }

    #region Events
    private void GtUser_OnStoresChanged(Stores stores)
    {
        RefreshItems(stores);
    }
    #endregion Events
}
