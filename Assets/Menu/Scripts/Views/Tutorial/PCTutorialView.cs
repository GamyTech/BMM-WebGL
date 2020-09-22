using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

public class PCTutorialView : BaseTutorialView
{
    public GameObject GlobalContent;
    public RectTransform hole;
    public RectTransform secondHole;
    public Button holeButton;
    public Button NextCollider;
    public GameObject Collider;
    public GameObject NextIcon;
    public Button SkipButton;
    public RectTransform ArrowRectTransform;
    public RectTransform Bubble;
    public TutorialLine LinePrefab;

    private RectTransform m_targetRect;
    private Func<Transform> m_getTarget;
    private List<TutorialLine> m_lines = new List<TutorialLine>();
    private bool m_hasTarget;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        Destroy(this.gameObject);
#else
        secondHole.gameObject.SetActive(false);
        SkipButton.onClick.AddListener(TutorialController.Instance.NextStep);
#endif
    }

    private void LateUpdate()
    {
        if (m_getTarget == null)
            return;

        SetHoleTarget();
        if (m_hasTarget)
        {
            hole.position = m_targetRect.position;
            hole.sizeDelta = m_targetRect.rect.size;
            SetArrowPosition();
        }
    }

    public override void CloseTutorial()
    {
        GlobalContent.SetActive(false);
        Reset();
    }

    public override void ShowInstruction(bool show)
    {
        NextCollider.gameObject.SetActive(show);
        Collider.SetActive(!show);
    }

    private void Reset()
    {
        m_hasTarget = false;
        m_currentStep = null;
        m_getTarget = null;
        m_targetRect = null;
        hole.gameObject.SetActive(false);
        holeButton.gameObject.SetActive(false);
        NextCollider.gameObject.SetActive(false);
        Collider.SetActive(false);
        ArrowRectTransform.localPosition = Vector3.zero;
        hole.position = Vector3.zero;

        for (int x = 0; x < m_lines.Count; ++x)
            Destroy(m_lines[x].gameObject);
        m_lines.Clear();
    }

    private void SetHiddenBehaviour()
    {
        NextCollider.gameObject.SetActive(!m_currentStep.Hidden);
        GlobalContent.SetActive(!m_currentStep.Hidden);
        if (m_currentStep.Hidden)
            StartCoroutine(CheckConditionsAndContinue());
    }

    private void SetLines()
    {
        for (int x = m_currentStep.linesList.Count - 1; x >= 0; --x)
        {
            TutorialLine newLine = Instantiate(LinePrefab, Bubble);
            newLine.transform.SetAsFirstSibling();
            int topPadding = x == 0 ? 38 : 0;
            int bottomPadding = x == m_currentStep.linesList.Count - 1 ? 38 : 15;
            newLine.Init(m_currentStep.linesList[x], topPadding, bottomPadding);
            m_lines.Add(newLine);
        }
    }

    private void SetInteractable()
    {
        NextCollider.interactable = !m_currentStep.ClickableHole && !m_currentStep.Skippable;
        NextIcon.gameObject.SetActive(!m_currentStep.ClickableHole && !m_currentStep.Skippable);

        holeButton.gameObject.SetActive(m_currentStep.ClickableHole);
        SkipButton.gameObject.SetActive(m_currentStep.Skippable);
    }

    private void SetArrowRotation()
    {
        if (m_currentStep.ClickableHole)
            ArrowRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, (float)(m_currentStep.Position) * 90.0f);
    }

    private void SetGetTarget()
    {
        TutorialController.GetTargetPool.TryGetValue(m_currentStep.SavedProgress, out m_getTarget);
    }

    private void SetHoleTarget()
    {
        try { m_targetRect = m_getTarget() as RectTransform; } catch (Exception) { }
        m_hasTarget = m_targetRect != null;
        hole.gameObject.SetActive(m_hasTarget);
    }

    private void SetArrowPosition()
    {
        ArrowRectTransform.gameObject.SetActive(m_hasTarget && m_currentStep.ClickableHole);
        if (m_hasTarget && m_currentStep.ClickableHole)
            ArrowRectTransform.localPosition = ArrowRectTransform.up * (30.0f + ((int)m_currentStep.Position % 2 == 0 ? hole.rect.height : hole.rect.width) / 2.0f);
    }

    public override void ShowNewStep(TutorialStep step)
    {
        Reset();
        m_currentStep = step;
        StartCoroutine(HandleCurrentStep());
    }


    private IEnumerator HandleCurrentStep()
    {
        Func<IEnumerator> conditionToStart;
        if (TutorialController.ConditionToStartPool.TryGetValue(m_currentStep.SavedProgress, out conditionToStart))
            yield return StartCoroutine(conditionToStart());

        SetArrowRotation();
        SetGetTarget();
        SetLines();
        SetInteractable();
        SetHiddenBehaviour();
    }

    public void ShowSecondHole(Transform target)
    {
        secondHole.gameObject.SetActive(true);
        RectTransform rectTraget = target as RectTransform;
        secondHole.position = rectTraget.position;
        secondHole.sizeDelta = new Vector2(rectTraget.rect.width, rectTraget.rect.height);
    }

    public void HideSecondHole()
    {
        secondHole.gameObject.SetActive(false);
    }

    public void NextButton(bool skipAction)
    {
        UnityAction nextAction;
        if (!skipAction && TutorialController.NextActionPool.TryGetValue(m_currentStep.SavedProgress, out nextAction))
            nextAction();

        StartCoroutine(CheckConditionsAndContinue());
    }

    private IEnumerator CheckConditionsAndContinue()
    {
        Func<IEnumerator> conditionForNext;
        if (TutorialController.ConditionForNextPool.TryGetValue(m_currentStep.SavedProgress, out conditionForNext))
            yield return conditionForNext();

        TutorialController.Instance.NextStep();
    }

}
