using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ContentTest : MonoBehaviour
{

    [Range(0,1)]
    public float testFloat;

    public int maxHeight = 100;

    List<LayoutElement> elements = new List<LayoutElement>();

    private RectTransform m_rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (m_rectTransform == null)
                m_rectTransform = transform as RectTransform;
            return m_rectTransform;
        }
    }

    void OnEnable()
    {
        Rebuild();
    }

    void LateUpdate()
    {
        float totalHeight = rectTransform.sizeDelta.y;
        float remainingHeight = totalHeight - maxHeight;
        if (elements.Count != transform.childCount)
        {
            Rebuild();
        }
        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].preferredHeight = CalcElementSize(i, elements.Count, testFloat, totalHeight, remainingHeight, maxHeight);
        }
    }

    private void Rebuild()
    {
        elements.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            elements.Add(transform.GetChild(i).GetComponent<LayoutElement>());
        }
    }

    private float CalcElementSize(int index, int count, float pos, float total, float remaining, float max)
    {
        float size = 0;
        float distance = Mathf.Abs(index - pos * (count - 1));
        size = max - distance * distance * (max/14);
        return size;
    }



}
