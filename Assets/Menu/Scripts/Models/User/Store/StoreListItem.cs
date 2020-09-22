using GT.Store;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoreListItem : DynamicElement
{
    private int m_index;
    private ToggleGroup m_group;
    private Store m_store;
    private UnityAction<int> OnSelectAction;
    public StoreBarView View { get; private set; }

    public StoreListItem(int index, ToggleGroup group, Store store, UnityAction<int> onSelectAction)
    {
        m_index = index;
        m_group = group;
        m_store = store;
        OnSelectAction = onSelectAction;
    }

    protected override void Populate(RectTransform activeObject)
    {
        View = activeObject.GetComponent<StoreBarView>();
        View.Populate(m_group, m_store, TogglePressed);
        View.CheckStoreTypeChange(PageController.Instance.CurrentPage.StoreType);
    }

    private void TogglePressed()
    {
        PageController.Instance.ChangePage(m_store.pageId);
        if (OnSelectAction != null)
            OnSelectAction(m_index);
    }

    public override void DeactivateObject()
    {
        base.DeactivateObject();
        View.DisableToggle();
    }
}
