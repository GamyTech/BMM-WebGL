using UnityEngine.UI;
using System.Collections.Generic;
using GT.Store;
using UnityEngine;

public class ChatWordsWidget : Widget
{
    public ScrollRect scrollRect;
    private GridDynamicContentLayoutGroup m_content;
    public GridDynamicContentLayoutGroup content
    {
        get { return m_content ?? (m_content = FindObjectOfType<GridDynamicContentLayoutGroup>()); }
        private set { m_content = value; }
    }

    public CanvasGroup listView;
    public CanvasGroup editorView;
    public ChatEditDraggableView itemView;

    public void EnterEditorMode(bool withTransition)
    {
        SwitchView(true, withTransition);
        SetDraggedItem();
    }

    public void EnterListMode(bool withTransition)
    {
        itemView.ResetPosition();
        SwitchView(false, withTransition);
    }

    public void AnimateToSlot(GameObject slot)
    {
        itemView.SetSelectedSlot(slot);
    }

    private void SetDraggedItem()
    {
        Store store = UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.Chat);
        StoreItem item = store.selected.selectedForSwap;
        if (item != null)
        {
            itemView.Populate(item);
            itemView.SetToPosition(FindPosition(item.Id));
        }
    }

    private Vector3 FindPosition(string itemId)
    {
        foreach (IDynamicElement element in content.elementsList)
        {
            if (element.activeObject != null)
                if ((element as StoreItem).Id == itemId)
                {
                    Vector3[] corners = new Vector3[4];
                    Image image = element.activeObject.GetComponent<ChatWordView>().Image;
                    image.GetComponent<RectTransform>().GetWorldCorners(corners);
                    return ChatEditDraggableView.GetMidPositionFromCorners(corners);
                }
        }
        return new Vector3();
    }

    private void SwitchView(bool isEditor, bool withTransition)
    {
        CanvasGroup canvasHide = isEditor ? listView : editorView;
        CanvasGroup canvasShow = !isEditor ? listView : editorView;

        if (withTransition)
        {
            float fadeTime = 0.25f;
            StartCoroutine(Change.GenericChange(1f, 0, fadeTime,
                Change.Lerp, a => canvasHide.alpha = a, null));
            StartCoroutine(Change.GenericChange(0, 1f, fadeTime,
                Change.Lerp, a => canvasShow.alpha = a, FadeOutFinished));
        }
        else
        {
            canvasHide.alpha = 0;
            canvasShow.alpha = 1;
        }
    }

    private void FadeOutFinished()
    {
        itemView.AnimatePositionToZero();
    }

    public override void EnableWidget()
    {
        base.EnableWidget();

        EnterListMode(false);

        UserController.Instance.gtUser.StoresData.OnStoresUpdated += GtUser_OnStoresChanged;
        if (UserController.Instance.gtUser.StoresData != null)
            InitList(UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.Chat).items);

#if UNITY_STANDALONE || UNITY_WEBGL
        content.constraintCount = 3;
        MiddleWidgetScrollBar.Instance.SetNewScrollRect(scrollRect);
#endif
    }

    public override void DisableWidget()
    {
        UserController.Instance.gtUser.StoresData.OnStoresUpdated -= GtUser_OnStoresChanged;
        content.ClearElements();
        itemView.ResetPosition();

        base.DisableWidget();
    }

    public void InitList(List<StoreItem> items)
    {
        List<IDynamicElement> notSelectedItems = new List<IDynamicElement>();
        int size = items.Count;
        for (int i = 0; i < size; i++)
        {
            if (!items[i].Selected) notSelectedItems.Add(items[i]);
        }
        content.SetElements(notSelectedItems);
    }

    void SetItems(List<IDynamicElement> items)
    {
        content.SetElements(items);
    }

    internal void UpdateList()
    {
        InitList(UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.Chat).items);
    }

    #region Events
    private void GtUser_OnStoresChanged(Stores newValue)
    {
        UpdateList();
    }
    #endregion Events
}
