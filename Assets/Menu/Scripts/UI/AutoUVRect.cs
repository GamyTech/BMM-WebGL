using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoUVRect : MonoBehaviour
{
    private Texture currTex;
    private RawImage m_image;
    private RectTransform m_rectTransform;

    protected RawImage image
    {
        get
        {
            if (m_image == null)
                m_image = GetComponent<RawImage>();
            return m_image;
        }
    }

    protected RectTransform rectTransform
    {
        get
        {
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();
            return m_rectTransform;
        }
    }

    void OnEnable()
    {
        CalcUVRect();
    }

    void Update()
    {
        if(currTex != image.texture)
        {
            currTex = image.texture;
            CalcUVRect();
        }
    }

    void OnRectTransformDimensionsChange()
    {
        if (isActiveAndEnabled)
        {
            CalcUVRect();
        }
    }

    private void CalcUVRect()
    {
        if (image.texture == null) return;
        Rect uvrect = new Rect(new Vector2(0, 0), new Vector2(rectTransform.rect.size.x / image.texture.width,
            rectTransform.rect.size.y / image.texture.height));
        image.uvRect = uvrect;
    }
}
