using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GT.Backgammon.View;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using GT.Backgammon.Logic;
using System;

public class TestSlotView : MonoBehaviour, ISlotView, IPointerClickHandler, IScrollHandler
{
    static Dictionary<SlotColor, Color> SlotColorDict = new Dictionary<SlotColor, Color>()
    {
        { SlotColor.Black, Color.black },
        { SlotColor.White, Color.white },
        { SlotColor.Empty, Color.clear },
    };

    static Dictionary<SlotColor, Color> TextColorDict = new Dictionary<SlotColor, Color>()
    {
        { SlotColor.Black, Color.white },
        { SlotColor.White, Color.black },
        { SlotColor.Empty, Color.clear },
    };

    public Image topImage;
    public Image bottomImage;

    public Text topText;
    public Text bottomText;

    private Image checkerImage;
    private Image indicatorImage;
    private Text checkerText;

    private UnityAction<BaseEventData> SlotClickedAction;
    public int index;

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
            if (Utils.SetProperty(ref m_quantity, Mathf.Max(0, value)))
            {
                checkerText.text = m_quantity > 0 ? m_quantity.ToString() : string.Empty;
            }
        }
    }

    [SerializeField]
    private SlotColor m_slotColor;
    public SlotColor SlotColor
    {
        get { return m_slotColor; }
        set
        {
            if (Utils.SetProperty(ref m_slotColor, value))
            {
                checkerImage.color = SlotColorDict[m_slotColor];
                checkerText.color = TextColorDict[m_slotColor];
            }
        }
    }

    #region ISlot Implementation
    public void InitSlot(ISlotViewData slotViewData)
    {
        TestSlotViewData data = slotViewData as TestSlotViewData;
        this.index = data.index;
        checkerText = data.top ? bottomText : topText;
        checkerImage = data.top ? bottomImage : topImage;
        indicatorImage = data.top ? topImage : bottomImage;
        SlotClickedAction = data.clickedAction;

        checkerText.text = string.Empty;
        checkerImage.color = SlotColorDict[m_slotColor];
        checkerText.color = TextColorDict[m_slotColor];

        indexText = data.top ? topText : bottomText;
    }

    Text indexText;
    public void SetSlotView(int logicIndex, SlotColor color, int quantity)
    {
        indexText.text = logicIndex.ToString();

        m_logicIndex = logicIndex;
        SlotColor = color;
        Quantity = quantity;
    }

    public void AddChecker(SlotColor color)
    {
        SlotColor = color;
        Quantity++;
    }

    public void RemoveChecker()
    {
        Quantity--;
        if (Quantity == 0)
            SlotColor = SlotColor.Empty;
    }

    public void ResetSlotView()
    {
        SlotColor = SlotColor.Empty;
        Quantity = 0;
        ResetIndicator();
    }

    public void SetIndicator(Color color)
    {
        indicatorImage.enabled = true;
        indicatorImage.color = color;
    }

    public void ResetIndicator()
    {
        indicatorImage.enabled = false;
    }
    #endregion ISlot Implementation

    #region Input
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SlotClickedAction != null)
            SlotClickedAction(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (SlotClickedAction != null)
            SlotClickedAction(eventData);
    }

    #endregion Input
}
