using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GT.Store;
using System;

public class ChatWordsBarWidget: Widget
{
    public RectTransform ExitEditModeButton;

    private Canvas m_canvas;
    protected Canvas Canvas
    {
        get
        {
            if (m_canvas == null)
                m_canvas = GetComponentInParent<Canvas>();
            return m_canvas;
        }
    }
    private ChatWordsWidget m_midPart;
    protected ChatWordsWidget MidPart
    {
        get
        {
            if (m_midPart == null)
                m_midPart = FindObjectOfType<ChatWordsWidget>();
            return m_midPart;
        }
    }

    public ObjectPool Pool;
    private List<GameObject> activeObjects = new List<GameObject>();
    private Selected selected;

    public override void EnableWidget()
    {
        base.EnableWidget();

        UserController.Instance.gtUser.StoresData.OnStoresUpdated += GtUser_OnStoresChanged;
        if (UserController.Instance.gtUser.StoresData != null)
        {
            Store store = UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.Chat);
            selected = store.selected;
            AddSelectedListeners();

            InitUsed(selected.selectedItems);
        }

        PutExitEditModeButtonOnTop();
        EnableExitEditModeButton(false);
    }

    internal GameObject GetItemUnder(Rect place)
    {
        float maxIntersection = 0;
        GameObject mostIntersected = null;
        Vector3[] corners = new Vector3[4];
        foreach (GameObject obj in activeObjects)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.GetWorldCorners(corners);
            Rect rect = new Rect(corners[0], (corners[2] - corners[0]));

            float intersection = Intersection(rect, place);
            if (intersection > maxIntersection)
            {
                maxIntersection = intersection;
                mostIntersected = obj;
            }
        }
        return mostIntersected;
    }

    private float Intersection(Rect rect1, Rect rect2)
    {
        float xOverlap = Mathf.Max(0, Mathf.Min(rect1.xMax, rect2.xMax) - Mathf.Max(rect1.xMin, rect2.xMin));
        float yOverlap = Mathf.Max(0, Mathf.Min(rect1.yMax, rect2.yMax) - Mathf.Max(rect1.yMin, rect2.yMin));
        return xOverlap * yOverlap;
    }

    private void PutExitEditModeButtonOnTop()
    {
        SettingsController.OnScreenSizeChanged += SettingsController_OnScreenSizeChanged;
        ExitEditModeButton.sizeDelta = new Vector2(Screen.width / Canvas.scaleFactor, Screen.height / Canvas.scaleFactor);
        ExitEditModeButton.transform.SetParent(WidgetController.Instance.middleWidgetContainer.transform.parent.parent);
    }

    private void EnableExitEditModeButton(bool isEnabled)
    {
        ExitEditModeButton.gameObject.SetActive(isEnabled);
    }

    public override void DisableWidget()
    {
        ExitEditModeButton.transform.SetParent(transform);
        SettingsController.OnScreenSizeChanged -= SettingsController_OnScreenSizeChanged;

        ClearItems();

        RemoveSelectedListeners();

        RemoveEditModeListeners();

        base.DisableWidget();
    }

    private void InitUsed(List<StoreItem> list)
    {
        ClearItems();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
                continue;
            GameObject go = Pool.GetObjectFromPool();
            go.InitGameObjectAfterInstantiation(Pool.transform);
            activeObjects.Add(go);
            ChatWordBarView view = go.GetComponent<ChatWordBarView>();
            view.Populate(list[i]);
            view.OnButtonTriggered += ChatWordBarView_OnButtonTriggered;
        }
        EnableExitEditModeButton(false);
    }

    private void ClearItems()
    {
        RemoveAllItemsListeners();
        Pool.PoolObjects(activeObjects);
        activeObjects.Clear();
    }

    private void SetSelected(bool isSelected)
    {
        for (int i = 0; i < activeObjects.Count; i++)
        {
            activeObjects[i].GetComponent<ChatWordBarView>().BuyButton.interactable = isSelected;
        }

        EnableExitEditModeButton(isSelected);
        GetComponent<LayoutElement>().ignoreLayout = isSelected;

        ShakeWords(isSelected);

        if (isSelected)
        {
            transform.SetParent(WidgetController.Instance.topWidgetContainer.transform.parent.parent);
            if (MidPart != null)
            {
                MidPart.EnterEditorMode(true);
                AddEditModeListeners();
            }
        }
        else
        {
            ResetPositions();

            transform.SetParent(WidgetController.Instance.topWidgetContainer.transform.GetChild(0));
            if (MidPart != null)
            {
                RemoveEditModeListeners();
                MidPart.EnterListMode(false);
            }
        }
    }

    private void ResetPositions()
    {
        foreach (var item in activeObjects)
        {
            item.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    #region View animation
    internal void AnimateSelectionOf(GameObject selected)
    {
        foreach (GameObject obj in activeObjects)
        {
            obj.GetComponent<ChatWordBarView>().AnimateSelection(obj == selected);
        }
    }

    private void ShakeWords(bool isOn)
    {
        foreach (GameObject obj in activeObjects)
        {
            obj.GetComponent<ChatWordBarView>().AnimateShaking(isOn);
        }
    }
    #endregion

    #region Listeners
    private void RemoveAllItemsListeners()
    {
        for (int i = 0; i < activeObjects.Count; i++)
        {
            activeObjects[i].GetComponent<ChatWordBarView>().OnButtonTriggered -= ChatWordBarView_OnButtonTriggered;
        }
    }

    private void RemoveEditModeListeners()
    {
        MidPart.itemView.OnSlotSelected -= ChatEditDraggableView_OnSlotSelected;
        MidPart.itemView.OnMotionEnd -= ChatEditDraggableView_OnMotionEnd;
        MidPart.itemView.CancelItemSelection -= ChatEditDraggableView_OnSelectionCancelled;
    }

    private void AddEditModeListeners()
    {
        MidPart.itemView.OnMotionEnd += ChatEditDraggableView_OnMotionEnd;
        MidPart.itemView.OnSlotSelected += ChatEditDraggableView_OnSlotSelected;
        MidPart.itemView.CancelItemSelection += ChatEditDraggableView_OnSelectionCancelled;
    }

    private void AddSelectedListeners()
    {
        selected.OnSelectedForSwapChanged += Selected_OnSelectedForSwapChanged;
        selected.OnSelectedListChanged += Selected_OnSelectedListChanged;
    }

    private void RemoveSelectedListeners()
    {
        selected.OnSelectedForSwapChanged -= Selected_OnSelectedForSwapChanged;
        selected.OnSelectedListChanged -= Selected_OnSelectedListChanged;
    }
    #endregion

    #region Events
    private void SettingsController_OnScreenSizeChanged(int width, int height)
    {
        ExitEditModeButton.sizeDelta = new Vector2(Screen.width / Canvas.scaleFactor, Screen.height / Canvas.scaleFactor);
    }

    private void GtUser_OnStoresChanged(Stores stores)
    {
        selected = stores.GetStore(Enums.StoreType.Chat).selected;
        AddSelectedListeners();

        InitUsed(selected.selectedItems);
    }

    private void Selected_OnSelectedListChanged(List<StoreItem> items)
    {
        InitUsed(items);
    }

    private void Selected_OnSelectedForSwapChanged(StoreItem item)
    {
        SetSelected(item != null);
    }

    private void ChatEditDraggableView_OnSlotSelected(StoreItem selectedItem, StoreItem oldItem)
    {
        ChangeSelected(selectedItem, oldItem);
    }

    private void ChatEditDraggableView_OnMotionEnd(StoreItem selectedItem, StoreItem oldItem)
    {
        ChangeSelected(selectedItem, oldItem);
        MidPart.EnterListMode(false);
    }

    private void ChatEditDraggableView_OnSelectionCancelled()
    {
        CancelEditMode();
    }

    private void ChatWordBarView_OnButtonTriggered(ChatWordBarView slot)
    {
        MidPart.AnimateToSlot(slot.gameObject);
    }
    #endregion Events

    private void ChangeSelected(StoreItem selectedItem, StoreItem oldItem)
    {
        selectedItem.Select();
        selected.SelectItem(false, oldItem);
        selected.SelectItem(true, selectedItem);
        MidPart.UpdateList();
    }

    public void CancelEditMode()
    {
        selected.DeselectItemForSwap();
    }
}
