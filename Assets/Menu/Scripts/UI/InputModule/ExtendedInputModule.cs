using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class ExtendedInputModule : StandaloneInputModule
{
    public static string TOUCH_LISTENER_TAG = "TouchListener";

    public delegate void InstantInputEvent();
    public static event InstantInputEvent OnScreenTouchEnd;
    public static event InstantInputEvent OnSelectableSubmitted;

    private TouchInputHandler touchedElement;
    public static Dictionary<int, UnityAction> OnPressedDownCallbacks = new Dictionary<int, UnityAction>();

    private bool PointerOnSelected;
    private Selectable selected = null;
    private bool shouldResetIntetractable = false;

    public void Update()
    {
#if  UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
        PointerEventData pointerData = GetLastPointerEventData(-1);
#else
        PointerEventData pointerData = GetLastPointerEventData(0);
#endif
        if(shouldResetIntetractable)
        {
            shouldResetIntetractable = false;
            selected.interactable = true;
        }
        if (OnSelectableSubmitted != null && pointerData != null)
        {
            if (pointerData.pointerPress != null)
            {
                if (selected == null)
                {
                    Selectable selectable = pointerData.pointerPress.GetComponent<Selectable>();
                    if (selectable != null && selectable.IsInteractable())
                    {
                        selected = selectable;
                        UnityAction onPressedDownAction;
                        if (OnPressedDownCallbacks.TryGetValue(selectable.GetInstanceID(), out onPressedDownAction))
                        {
                            shouldResetIntetractable = true;
                            selected.interactable = false;
                            onPressedDownAction();
                        }
                    }
                }
                else if (pointerData.pointerCurrentRaycast.gameObject != null)
                    PointerOnSelected = pointerData.pointerCurrentRaycast.gameObject.transform.IsChildOf(selected.transform);
                else
                    PointerOnSelected = false;
            }
            else
            {
                if (PointerOnSelected && !pointerData.dragging)
                    OnSelectableSubmitted();
                selected = null;
                PointerOnSelected = false;
            }
        }
        else
        {
            if (PointerOnSelected)
                OnSelectableSubmitted();
            selected = null;
            PointerOnSelected = false;
        }

        if (Camera.main == null)
        {
            Debug.LogError("No Camera");
            return;
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (OnScreenTouchEnd != null)
                OnScreenTouchEnd();
        }
        else if(Input.touches.Length > 0)
        {
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        Debug.Log("touched object :" + hitInfo.collider.name + " with tag : " + hitInfo.collider.tag);
                        if (hitInfo.collider.tag == TOUCH_LISTENER_TAG)
                        {
                            touchedElement = hitInfo.collider.GetComponent<TouchInputHandler>();
                            touchedElement.GetInputDown(Input.touches[0].position);
                        }
                    }
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    if (OnScreenTouchEnd != null)
                        OnScreenTouchEnd();
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (touchedElement != null)
                    {
                        Debug.LogError("drag touched element");
                        touchedElement.GetInputDrag(Input.touches[0].position);
                    }
                    break;
            }
        }
        else if(touchedElement != null)
        {
            Debug.LogError("Drop touched element");
            touchedElement.GetInputUp();
            touchedElement = null;
        }
    }



}
