using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class TextResizer : UIBehaviour
{
    public Vector2 initialSize;
    private Vector2 currSize;

    private RectTransform m_rectTransform;
    private Text m_text;

    public RectTransform rectTransform
    {
        get
        {
            if (m_rectTransform == null)
                m_rectTransform = transform as RectTransform;
            return m_rectTransform;
        }
    }

    public Text text
    {
        get
        {
            if (m_text == null)
                m_text = GetComponentInChildren<Text>();
            return m_text;
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        float widthPercent = currSize.x / initialSize.x;
        float height = currSize.y / initialSize.y;
        if (currSize.x != rectTransform.sizeDelta.x)
        {
            widthPercent = rectTransform.sizeDelta.x / initialSize.x;
        }
        if (currSize.y != rectTransform.sizeDelta.y)
        {
            height = rectTransform.sizeDelta.y / initialSize.y;
        }

        text.transform.localScale = new Vector3(widthPercent.IsValidNum() ? widthPercent : 1, height.IsValidNum() ? height : 1, 1);
        currSize = rectTransform.sizeDelta;
    }
}
