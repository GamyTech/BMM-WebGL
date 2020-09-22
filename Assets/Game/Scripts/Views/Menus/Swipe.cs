using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Swipe : Selectable
{
    public Camera Cam;
    public BeforeRollingButtonsView beforeRollingButtonsView;
    Vector3 start;
    Vector3 end;

    public override void OnPointerDown(PointerEventData eventData)
    {
        start = gameObject.transform.position - Cam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10.0f));
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        end = gameObject.transform.position - Cam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10.0f));
        float delta = end.y - start.y;

        if (Mathf.Abs(delta) > 0.5)
            beforeRollingButtonsView.SetActive(delta < 0);
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }
}
