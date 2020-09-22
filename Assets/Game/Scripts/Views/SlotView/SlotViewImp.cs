using UnityEngine;
using GT.Backgammon.View;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using GT.Backgammon.Logic;
using System.Collections;
using GT.Backgammon.Player;

public class SlotViewImp : MonoBehaviour, ISlotView
{

    #region Public Members

    public TouchInputHandler touchInputHandler;
    public SpriteRenderer indicator;
    public ObjectPool CheckerPool;

    public Sprite blackChecker;
    public Sprite whiteChecker;
    public Sprite blackBearOffChecker;
    public Sprite whiteBearOffChecker;

    public float checkersDistance; //regular 0.55 , bearoff 0.16
    public int index;
    public float spacingDelta = -0.55f;
    public bool isEatenSlot;
    public bool isBearOffSlot;
    public bool topSlot;
    #endregion Public Members

    #region Private Members

    private UnityAction<int> SlotClickedAction;
    private UnityAction<int> CheckerSelectedAction;
    private UnityAction<int, int, Checker> CheckerDroppedAction;

    private readonly int MaxCheckersBeforeStacking = 5;
    #endregion Private Members

    #region Props
    private int m_logicIndex;
    public int LogicIndex
    {
        get { return m_logicIndex; }
    }

    [SerializeField]
    private int m_quantity;
    public int Quantity
    {
        get { return m_quantity; }
        set
        {
            Utils.SetProperty(ref m_quantity, Mathf.Max(0, value));
        }
    }

    [SerializeField]
    private SlotColor m_slotColor;
    public SlotColor SlotColor
    {
        get { return m_slotColor; }
        set
        {
            Utils.SetProperty(ref m_slotColor, value);
        }
    }
    #endregion Props

    #region ISlot Implementation
    private void Start()
    {
        touchInputHandler.Init(GetMouseDown);
    }

    public void InitSlot(ISlotViewData slotViewData)
    {
        ImpSlotViewData data = slotViewData as ImpSlotViewData;
        this.index = data.Index;

        SlotClickedAction = data.ClickedAction;
        CheckerSelectedAction = data.CheckerSelectedAction;
        CheckerDroppedAction = data.CheckerDroppedAction;
        spacingDelta = (topSlot) ? -checkersDistance : checkersDistance;

        blackChecker = data.Checkers[0].ToSprite();
        whiteChecker = data.Checkers[1].ToSprite();
        blackBearOffChecker = data.Checkers[2].ToSprite();
        whiteBearOffChecker = data.Checkers[3].ToSprite();
    }

    public IEnumerator SlotOrganize()
    {
        if (transform.childCount == 0 || isBearOffSlot)
            yield break;

        spacingDelta = topSlot ? -checkersDistance : checkersDistance;
        float spacingPresentage = 0.025f;

        if (transform.childCount > MaxCheckersBeforeStacking)
            spacingDelta += (topSlot ? 1 : -1) * (spacingPresentage * transform.childCount);

        float duration = 1f;
        float counter = 0;
        while (counter < duration)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 v = GetIndex(i);
                Transform child = topSlot ? transform.GetChild(i) : transform.GetChild(transform.childCount - 1 - i);
                Checker checker = child.GetComponent<Checker>();
                if (checker != null && !checker.isMoving)
                    child.position = Change.Lerp(child.position, v, counter / duration);
            }
            yield return null;
            counter += 0.1f;
        }
    }

    public void SetSlotView(int logicIndex, SlotColor color, int quantity)
    {
        ResetSlotView();
        m_logicIndex = logicIndex;
        SlotColor = color;
        SetSlotCheckersQuantity(quantity);
        DisableCheckersCollider();
        DisableSlotCollider();

        if (quantity > MaxCheckersBeforeStacking)
        {
            StartCoroutine(SlotOrganize());
        }
    }

    public void SetSlotColor(PlayerColor color)
    {
        SlotColor = color == PlayerColor.Black ? SlotColor.Black : SlotColor.White;
    }

    public void SetSlotColorEmpty()
    {
        if (Quantity == 0)
            SlotColor = SlotColor.Empty;
    }

    public void AddChecker(SlotColor color)
    {
        Transform checker = CheckerPool.GetObjectFromPool().transform;

        checker.GetComponent<Checker>().Init(CheckerSelectedAction, CheckerDroppedAction);
        checker.gameObject.InitGameObjectAfterInstantiation(transform);
        checker.localPosition = new Vector3(0f, spacingDelta * Quantity, 0f);

        if (topSlot)
            checker.GetComponent<SpriteRenderer>().sortingOrder = checker.GetSiblingIndex();
        else
            checker.SetAsFirstSibling();

        SetSortingOrder();
        SetSlotCheckersColor(color);
        Quantity++;
        SetCheckersCollider();
    }

    public void UpdateCheckersSprite(Texture2D[] checkers)
    {
        blackChecker = checkers[0].ToSprite();
        whiteChecker = checkers[1].ToSprite();
        blackBearOffChecker = checkers[2].ToSprite();
        whiteBearOffChecker = checkers[3].ToSprite();

        if (Quantity == 0)
            return;

        Sprite checkerColor;

        if (isBearOffSlot)
            checkerColor = (SlotColor == SlotColor.Black) ? blackBearOffChecker : whiteBearOffChecker;
        else
            checkerColor = (SlotColor == SlotColor.Black) ? blackChecker : whiteChecker;

        for (int i = 0; i < Quantity; i++)
            transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = checkerColor;
        SetSortingOrder();
    }

    public Vector3 GetIndex()
    {
        return transform.TransformPoint(Vector3.up * spacingDelta * (Quantity - 1));
    }

    public Vector3 GetIndex(int i)
    {
        return transform.TransformPoint(Vector3.up * spacingDelta * i);
    }

    public void SetSortingOrder()
    {
        foreach (Transform item in transform)
            item.GetComponent<SpriteRenderer>().sortingOrder = isEatenSlot ? item.transform.GetSiblingIndex() + 100 : item.transform.GetSiblingIndex();
    }

    public void SetCheckersCollider()
    {
        if (Quantity == 0)
            return;

        ActivateCheckersColliders(false);

        if (!isBearOffSlot)
        {
            BoxCollider box = transform.GetChild(topSlot ? Quantity - 1 : 0).GetComponent<BoxCollider>();
            box.enabled = true;
            box.size = new Vector3(0.6f, 0.8f, 0.6f);
        }
    }

    public void ActivateCheckersColliders(bool b)
    {
        for (int i = 0; i < Quantity; i++)
            transform.GetChild(i).GetComponent<BoxCollider>().enabled = b;
    }

    public void DisableCheckersCollider()
    {
        for (int i = 0; i < Quantity; i++)
        {
            BoxCollider box = transform.GetChild(i).GetComponent<BoxCollider>();
            box.size = Vector3.zero;
            box.enabled = false;
        }
    }

    public void DisableSlotCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    public void EnableSlotCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    public void RemoveChecker()
    {
        Debug.Log("RemoveChecker from :" + index + " " + Quantity);
        Quantity--;

        if (Quantity == 0)
            SlotColor = SlotColor.Empty;
        else
            Destroy(transform.GetChild(topSlot ? Quantity : 0).gameObject);

        SetCheckersCollider();
    }

    public GameObject GetChecker()
    {
        Quantity--;
        return transform.GetChild(topSlot ? Quantity : 0).gameObject;
    }

    public void AddCheckerAsChild(GameObject checker)
    {
        checker.transform.SetParent(transform);
        if (!topSlot)
            checker.transform.SetAsFirstSibling();

        Quantity++;
    }

    public void SetIndicator(Color color)
    {
        if (indicator != null)
        {
            indicator.enabled = true;
            indicator.color = color;
        }
    }

    public void ResetSlotView()
    {
        SlotColor = SlotColor.Empty;
        Quantity = 0;
        ResetIndicator();
        RemoveAllCheckers();
    }

    public void ResetIndicator()
    {
        if (indicator != null)
            indicator.enabled = false;
    }

    public void SetCheckerToBearOffSprite(PlayerColor color)
    {
        if (!isBearOffSlot)
            return;

        Sprite checkerColor = (color == PlayerColor.Black) ? blackBearOffChecker : whiteBearOffChecker;
        Transform checker = topSlot ? transform.GetChild(Quantity - 1) : transform.GetChild(0);

        checker.GetComponent<SpriteRenderer>().sprite = checkerColor;
        checker.gameObject.InitGameObjectAfterInstantiation(transform);

        foreach (Transform ckr in transform)
        {
            ckr.rotation = new Quaternion(0, 0, 0, 0);
            ckr.Rotate(new Vector3(0, 0, -6));
        }
    }

    public void SetCheckerToBoardSprite(PlayerColor color)
    {
        Sprite checkerColor = (color == PlayerColor.Black) ? blackChecker : whiteChecker;
        foreach (Transform checker in transform)
        {
            checker.GetComponent<SpriteRenderer>().sprite = checkerColor;
            checker.gameObject.InitGameObjectAfterInstantiation(transform);
        }
    }

    private void RemoveAllCheckers()
    {
        foreach (Transform checker in transform)
            Destroy(checker.gameObject);
    }

    private void SetSlotCheckersColor(SlotColor color)
    {
        Sprite checkerColor;
        if (isBearOffSlot)
            checkerColor = color == SlotColor.Black ? blackBearOffChecker : whiteBearOffChecker;
        else
            checkerColor = color == SlotColor.Black ? blackChecker : whiteChecker;

        foreach (Transform checker in transform)
            checker.GetComponent<SpriteRenderer>().sprite = checkerColor;
    }

    private void SetSlotCheckersQuantity(int quantity)
    {
        for (int i = 0; i < quantity; i++)
            AddChecker(SlotColor);
    }

    #endregion ISlot Implementation

    #region Input
    public void GetMouseDown(Vector2 mousePos)
    {
        if (SlotClickedAction != null && !(EventSystem.current.IsPointerOverGameObject() || Utils.IsPointerOverUIObject()))
            SlotClickedAction(index);
    }
    #endregion Input


}
