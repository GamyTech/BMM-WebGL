
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;

        anchorMin.y /= Screen.height;
        anchorMin.x /= Screen.width;
        anchorMax.y /= Screen.height;
        anchorMax.x /= Screen.width;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    } 
}
