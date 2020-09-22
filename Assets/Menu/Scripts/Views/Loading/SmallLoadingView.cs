using UnityEngine;
using System;

[RequireComponent (typeof(FadingElement))]
public class SmallLoadingView : MonoBehaviour
{
    public RectTransform rotatingObject;
    public Animator logoAnim;
    public bool rotate = true;
    public float rotationSpeed = 10f;

    private RectTransform overTransform;
    private FadingElement m_fadingElement;
    private RectTransform m_loadingPanel;

    public FadingElement fadingElement
    {
        get
        {
            if (m_fadingElement == null)
                m_fadingElement = GetComponentInChildren<FadingElement>(true);
            return m_fadingElement;
        }
    }

    public RectTransform loadingPanel
    {
        get
        {
            if (m_loadingPanel == null)
                m_loadingPanel = transform.GetChild(0).transform as RectTransform;
            return m_loadingPanel;
        }
    }

    void Start()
    {
        if(logoAnim != null)
            logoAnim.runtimeAnimatorController = AssetController.Instance.loadingData.logoAnim;
    }

    void Update()
    {
        if (rotate && rotatingObject != null)
            rotatingObject.Rotate(new Vector3(0, 0, 1), Time.deltaTime * rotationSpeed);
        if (overTransform != null)
        {
            loadingPanel.position = overTransform.position;
            loadingPanel.sizeDelta = overTransform.rect.size;
        }
    }

    public void ShowLoading(RectTransform over, Action finishedCallback = null, bool instant = false)
    {
        overTransform = over;
        ShowLoading(finishedCallback, instant);
    }

    public void ShowLoading(Action finishedCallback = null, bool instant = false)
    {
        fadingElement.FadeIn(instant, finishedCallback);
    }

    public void HideLoading(Action finishedCallback = null, bool instant = false)
    {
        fadingElement.FadeOut(instant, () =>
        {
            if (finishedCallback != null)
                finishedCallback();
        });
    }
}
