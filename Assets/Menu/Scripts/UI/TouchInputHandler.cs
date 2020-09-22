using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TouchInputHandler : MonoBehaviour
{
    UnityAction<Vector2> OnMouseDownCallback = null;
    UnityAction OnMouseUpCallback = null;
    UnityAction<Vector2> OnMouseDragCallback = null;

    // Use this for initialization
    public void Init (UnityAction<Vector2> onMouseDown = null, UnityAction onMousUp = null, UnityAction<Vector2> onMouseDrag = null)
    {
        tag = ExtendedInputModule.TOUCH_LISTENER_TAG;
        OnMouseDownCallback = onMouseDown;
        OnMouseUpCallback = onMousUp;
        OnMouseDragCallback = onMouseDrag;
    }

#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
    public void OnMouseDown()
    {
        GetInputDown(Input.mousePosition);
    }

    public void OnMouseUp()
    {
        GetInputUp();
    }

    public void OnMouseDrag()
    {
        GetInputDrag(Input.mousePosition);
    }
#endif

    public void GetInputDown(Vector2 inputPos)
    {
        if (OnMouseDownCallback != null)
            OnMouseDownCallback(inputPos);
    }

    public void GetInputUp()
    {
        if (OnMouseUpCallback != null)
            OnMouseUpCallback();
    }

    public void GetInputDrag(Vector2 inputPos)
    {
        if (OnMouseDragCallback != null)
            OnMouseDragCallback(inputPos);
    }
}
