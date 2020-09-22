using UnityEngine;

public class CanvasManualRotaition : MonoBehaviour
{
    public Vector3 ReferenceScale = new Vector3(.1f, .1f, .1f);
    public Vector2 ReferenceResolution = new Vector2(1334, 750);

    void Start()
    {
        float refFactor = ReferenceResolution.x / ReferenceResolution.y;
        float factor = Screen.height / (float)Screen.width;
        float nFactor = refFactor / factor;

        float newHeight = nFactor * ReferenceResolution.y;

        RectTransform rect = transform as RectTransform;
        Vector2 size = rect.sizeDelta;
        size.y = newHeight;
        rect.sizeDelta = size;
    }
}
