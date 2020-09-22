using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class FadingElement : MonoBehaviour
{
    enum Status
    {
        Idle,
        FadingIn,
        FadingOut,
    }

    public float fadeTime = 0.2f;
    public bool changeActiveState = false;

    private Status fadingStatus = Status.Idle;

    private CanvasGroup m_canvasGroup;
    public CanvasGroup canvasGroup
    {
        get
        {
            if (m_canvasGroup == null)
                m_canvasGroup = GetComponent<CanvasGroup>();
            return m_canvasGroup;
        }
    }

    IEnumerator fadingInRoutine;
    IEnumerator fadingOutRoutine;
    List<Action> finishedCallbacks = new List<Action>();

    public void FadeIn(bool instant = false, Action finishedCallback = null)
    {
        if (fadingStatus == Status.FadingOut)
            StopFadeOut();

        finishedCallbacks.Add(finishedCallback);
        if(changeActiveState && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (instant || !gameObject.activeInHierarchy)
        {
            canvasGroup.alpha = 1;
            Finished(false);
            return;
        }

        if (fadingStatus == Status.FadingIn)
            return;

        fadingStatus = Status.FadingIn;
        fadingInRoutine = Change.GenericChange(canvasGroup.alpha, 1, fadeTime, Change.Lerp, a => canvasGroup.alpha = a, FadeInFinished);
        if (gameObject.activeInHierarchy)
            StartCoroutine(fadingInRoutine);
    }

    public void FadeOut(bool instant = false, Action finishedCallback = null)
    {
        if (fadingStatus == Status.FadingIn)
            StopFadeIn();

        finishedCallbacks.Add(finishedCallback);
        if (changeActiveState && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (instant || !gameObject.activeInHierarchy)
        {
            canvasGroup.alpha = 0;
            Finished(true);
            return;
        }

        if (fadingStatus == Status.FadingOut)
            return;

        fadingStatus = Status.FadingOut;
        fadingOutRoutine = Change.GenericChange(canvasGroup.alpha, 0, fadeTime, Change.Lerp, a => canvasGroup.alpha = a, FadeOutFinished);
        if (gameObject.activeInHierarchy)
            StartCoroutine(fadingOutRoutine);
    }

    void StopFadeIn()
    {
        if (fadingInRoutine != null)
            StopCoroutine(fadingInRoutine);
        Finished(false);
    }

    void StopFadeOut()
    {
        if (fadingOutRoutine != null)
            StopCoroutine(fadingOutRoutine);
        Finished(true);
    }

    void FadeInFinished()
    {
        canvasGroup.alpha = 1;
        Finished(false);
    }

    void FadeOutFinished()
    {
        canvasGroup.alpha = 0;
        Finished(true);
    }

    void Finished(bool isFadeOut)
    {
        fadingStatus = Status.Idle;
        for (int i = 0; i < finishedCallbacks.Count; i++)
        {
            if(finishedCallbacks[i] != null)
                finishedCallbacks[i]();
        }
        finishedCallbacks.Clear();

        if(changeActiveState && isFadeOut && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
