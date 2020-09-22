using UnityEngine;
using System.Collections;
using System;

public class PageLoadingView : MonoBehaviour {

    private SmallLoadingView m_loading;
    public SmallLoadingView loading
    {
        get
        {
            if (m_loading == null)
                m_loading = GetComponentInChildren<SmallLoadingView>(true);
            return m_loading;
        }
    }

    public void ShowLoading(RectTransform over, Action finishedCallback = null, bool instant = false)
    {
        if (over == null)
            over = transform as RectTransform;
        loading.ShowLoading(over, finishedCallback, instant);
    }

    public void HideLoading(Action finishedCallback = null, bool instant = false)
    {
        loading.HideLoading(finishedCallback, instant);
    }
}
