using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CyclicSelector : UIBehaviour, IEventSystemHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, ICanvasElement
{
    [SerializeField]
    private bool m_Horizontal = true;
    [SerializeField]
    private bool m_Vertical = true;
    [SerializeField]
    private bool m_Inertia = true;
    [SerializeField]
    private float m_DecelerationRate = 0.135f;
    [SerializeField]
    private float m_ScrollSensitivity = 1f;
    private Vector2 m_Velocity;

#pragma warning disable 0414
    private bool m_Dragging;
    private Vector2 m_PrevPosition = new Vector2(0, 0);
#pragma warning restore 0414

    [NonSerialized]
    private RectTransform m_Rect;

    /// <summary>
    ///   <para>Should horizontal scrolling be enabled?</para>
    /// </summary>
    public bool horizontal
    {
        get
        {
            return this.m_Horizontal;
        }
        set
        {
            this.m_Horizontal = value;
        }
    }

    /// <summary>
    ///   <para>Should vertical scrolling be enabled?</para>
    /// </summary>
    public bool vertical
    {
        get
        {
            return this.m_Vertical;
        }
        set
        {
            this.m_Vertical = value;
        }
    }

    /// <summary>
    ///   <para>Should movement inertia be enabled?</para>
    /// </summary>
    public bool inertia
    {
        get
        {
            return this.m_Inertia;
        }
        set
        {
            this.m_Inertia = value;
        }
    }

    /// <summary>
    ///   <para>The rate at which movement slows down.</para>
    /// </summary>
    public float decelerationRate
    {
        get
        {
            return this.m_DecelerationRate;
        }
        set
        {
            this.m_DecelerationRate = value;
        }
    }

    /// <summary>
    ///   <para>The sensitivity to scroll wheel and track pad scroll events.</para>
    /// </summary>
    public float scrollSensitivity
    {
        get
        {
            return this.m_ScrollSensitivity;
        }
        set
        {
            this.m_ScrollSensitivity = value;
        }
    }

    /// <summary>
    ///   <para>The current velocity of the content.</para>
    /// </summary>
    public Vector2 velocity
    {
        get
        {
            return this.m_Velocity;
        }
        set
        {
            this.m_Velocity = value;
        }
    }

    private RectTransform rectTransform
    {
        get
        {
            if (this.m_Rect == null)
            {
                this.m_Rect = base.GetComponent<RectTransform>();
            }
            return this.m_Rect;
        }
    }




    protected override void OnEnable()
    {
        base.OnEnable();
        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
    }

    protected override void OnDisable()
    {
        CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
        base.OnDisable();
    }

    #region Events

    #region Movement Handling
    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        Debug.Log("OnInitializePotentialDrag " + eventData.position);

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        this.m_Velocity = new Vector2(0, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag " + eventData.position);
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        //this.UpdateBounds();
        //this.m_PointerStartLocalCursor = new Vector2(0, 0);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out this.m_PointerStartLocalCursor);
        //this.m_ContentStartPosition = this.m_Content.anchoredPosition;
        this.m_Dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag " + eventData.delta);
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag " + eventData.position);
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        this.m_Dragging = false;
    }

    public void OnScroll(PointerEventData eventData)
    {
        Debug.Log("OnScroll " + eventData.scrollDelta + " " + eventData.delta);
        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);

    }
    #endregion Movement Handling




    public void Rebuild(CanvasUpdate executing)
    {
        Debug.Log("Rebuild " + executing);

    }

    public void LayoutComplete()
    {
        Debug.Log("LayoutComplete");
    }

    public void GraphicUpdateComplete()
    {
        Debug.Log("GraphicUpdateComplete");
    }
    #endregion Events

    #region Public Functions
    /// <summary>
    ///   <para>Sets the velocity to zero on both axes so the content stops moving.</para>
    /// </summary>
    public virtual void StopMovement()
    {
        this.m_Velocity = new Vector2(0, 0);
    }
    #endregion Public Functions
}
