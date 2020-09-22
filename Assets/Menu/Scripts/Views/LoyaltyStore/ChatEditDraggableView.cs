using System.Collections;
using GT.Store;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatEditDraggableView : ChatWordBarView
{
    public delegate void SlotSelected(StoreItem selectedItem, StoreItem oldItem);
    public event SlotSelected OnSlotSelected;
    public event SlotSelected OnMotionEnd;

    public delegate void CancelSelection();
    public event CancelSelection CancelItemSelection;

    private Camera m_camera;
    protected Camera Camera
    {
        get
        {
            if (m_camera == null)
                m_camera = FindObjectOfType<Camera>();
            return m_camera;
        }
    }

    private Transform itemParent;
    private ChatWordsBarWidget selectedBar;
    private EventTrigger dragTrigger;

    private GameObject itemSlot;
    private RectTransform dragged;

    private Vector3[] corners;
    private Vector3 dragPosition;
    private Vector3 dragOffset;

    private bool startPositionSaved = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Awake()
    {
        corners = new Vector3[4];
        dragPosition = new Vector3();
        dragged = GetComponent<RectTransform>();
        dragTrigger = GetComponent<EventTrigger>();
    }

    public override void Populate(StoreItem item)
    {
        Item = item;
        FillInfo(item);

        selectedBar = FindObjectOfType<ChatWordsBarWidget>();
        dragTrigger.enabled = true;

        MoveOutside();
    }

    internal void SetToPosition(Vector3 globalPosition)
    {
        startPosition = transform.position;

        transform.position += globalPosition - GetMidPositionFromCorners(corners);
    }

    internal void AnimatePositionToZero()
    {
        dragTrigger.enabled = false;

        IEnumerator routine = Change.GenericChange(transform.position, startPosition, 0.25f, 
            Change.Lerp, position => transform.position = position, OnStartPosition);
        StartCoroutine(routine);
    }

    void OnStartPosition()
    {
        dragTrigger.enabled = true;
    }

    public void SetSelectedSlot(GameObject slot)
    {
        itemSlot = slot;
        MoveToSelectedSlot();
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
        Vector3 worldPosition = Camera.ScreenToWorldPoint(((PointerEventData)eventData).position);
        dragPosition.Set(worldPosition.x, worldPosition.y, transform.position.z);
        dragOffset = dragPosition - transform.position;
    }

    public void OnDrag(BaseEventData eventData)
    {
        dragged.GetWorldCorners(corners);

        Rect rect = new Rect(corners[0], (corners[2] - corners[0]));

        Vector3 worldPosition = Camera.ScreenToWorldPoint(((PointerEventData)eventData).position);
        dragPosition.Set(worldPosition.x - dragOffset.x, worldPosition.y - dragOffset.y, transform.position.z);
        transform.position = dragPosition;

        GameObject oldSlot = itemSlot;
        itemSlot = selectedBar.GetItemUnder(rect);
        if (oldSlot != itemSlot) selectedBar.AnimateSelectionOf(itemSlot);
    }

    public void OnEndDrag(BaseEventData eventData)
    {
        if (itemSlot != null)
        {
            dragged.GetWorldCorners(corners);
            Vector3[] itemCorners = new Vector3[4];
            itemSlot.GetComponent<RectTransform>().GetWorldCorners(itemCorners);

            targetPosition = transform.position + itemCorners[0] - corners[0];

            StartCoroutine(MoveItemToSlot(OnSlotSelected));
        }
        else
        {
            if (CancelItemSelection != null)
                CancelItemSelection();
        }
    }

    private void MoveToSelectedSlot()
    {
        dragTrigger.enabled = false;

        Vector3[] draggedCorners = new Vector3[4];
        dragged.GetWorldCorners(draggedCorners);
        itemSlot.GetComponent<RectTransform>().GetWorldCorners(corners);

        targetPosition = transform.position + corners[0] - draggedCorners[0];

        StartCoroutine(MoveItemToSlot(OnMotionEnd));
    }

    private IEnumerator MoveItemToSlot(SlotSelected OnMotionFinished)
    {
        float duration = 1f;
        float counter = 0;
        while (counter < duration)
        {
            transform.position = Change.EaseInOutQuad(transform.position, targetPosition, counter / duration);
            yield return null;
            counter += 0.1f;
        }

        if (OnMotionFinished != null)
            OnMotionFinished(Item, itemSlot.GetComponent<ChatWordBarView>().Item);
    }

    #region Button positioning
    public void MoveOutside()
    {
        Transform newParent = WidgetController.Instance.middleWidgetContainer.transform.parent.parent;
        if (transform.parent != newParent)
        {
            GetComponent<RectTransform>().GetWorldCorners(corners);
            if (!startPositionSaved)
            {
                startPositionSaved = true;
                startPosition = GetTopPositionFromCorners(corners);
            }

            itemParent = transform.parent;
            transform.SetParent(newParent);
        }
    }

    public void ResetPosition()
    {
        if (itemParent != null)
        {
            transform.SetParent(itemParent, false);
            transform.localPosition = new Vector3();
        }
    }
    #endregion

    protected override void SetButton(StoreItem item) { }

    public static Vector3 GetTopPositionFromCorners(Vector3[] corners)
    {
        return (corners[1] + corners[2]) * 0.5f;
    }
    public static Vector3 GetMidPositionFromCorners(Vector3[] corners)
    {
        return (corners[1] + corners[2] + corners[0] + corners[3]) * 0.25f;
    }
}
