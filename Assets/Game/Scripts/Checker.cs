using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]

public class Checker : MonoBehaviour
{
    public TouchInputHandler touchInputHandler;
    public SpriteRenderer spriteRenderer;
    public BoxCollider boxCollider;

    private int targetSlotIndex;
    private int originSlotIndex;

    private Vector3 offset;
    private int defaultLayer;
    private  Vector3 initPosition;
    private int dragCount = 0;
    private int dragDelta = 5;

    public bool isCheckeHeld = false;
    public bool isDraged = false;

    private UnityAction<int> selectAction;
    private UnityAction<int,int,Checker> dropAction;

    internal bool isMoving;
    private void Start()
    {
        touchInputHandler.Init(GetMouseDown, GetMouseUp, GetMouseDrag);
    }

    public void Init(UnityAction<int> onSelect, UnityAction<int, int, Checker> onDropped)
    {
        selectAction = onSelect;
        dropAction = onDropped;
    }

    void GetMouseDown(Vector2 mousePos)
    {
        if (EventSystem.current.IsPointerOverGameObject() || Utils.IsPointerOverUIObject())
            return;

        isCheckeHeld = true;
        defaultLayer = spriteRenderer.sortingOrder;
        originSlotIndex = targetSlotIndex;
        initPosition = transform.position;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10.0f));

        selectAction(originSlotIndex);
    }

    void GetMouseUp()
    {

        if (!isCheckeHeld) return;

        isCheckeHeld = false;
        spriteRenderer.sortingOrder = defaultLayer;
        transform.localScale = new Vector3(1f, 1f, 1f);
        dragCount = 0;
        if (isDraged && targetSlotIndex == originSlotIndex)
            targetSlotIndex = -1;

        isDraged = false;

        dropAction(originSlotIndex, targetSlotIndex, this);
    }

    void GetMouseDrag(Vector2 mousePos)
    {
        if (!isCheckeHeld) return;

        dragCount++;
        if (dragCount > dragDelta)
        {
            boxCollider.center = new Vector3(0, 0, 0);
            boxCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            spriteRenderer.sortingOrder = 200;
            Vector3 newPosition = new Vector3(mousePos.x, mousePos.y, 10.0f);
            transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;

            if (Mathf.Abs(initPosition.x - transform.position.x) > 0.1f || Mathf.Abs(initPosition.y - transform.position.y) > 0.1f)
                isDraged = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        SlotViewImp s = other.gameObject.GetComponent<SlotViewImp>();
        if (s != null)
            targetSlotIndex = s.index;
    }

    void OnTriggerExit(Collider other)
    {
        SlotViewImp s = other.gameObject.GetComponent<SlotViewImp>();
        if (s != null && targetSlotIndex == s.index)
            targetSlotIndex = -1;
    }
}