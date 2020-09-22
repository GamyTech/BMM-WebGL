using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

public class MobileTutorialView : BaseTutorialView
{

    public GameObject GlobalContent;
    public RectTransform hole;
    public Button holeButton;
    public Button NextCollider;
    public GameObject Collider;
    public RectTransform pointer;
    public RectTransform Bubble;
    public GameObject swipeArrows;
    public GameObject NextButtonContainer;

    public TutorialLine LinePrefab;

    private RectTransform m_targetRect;
    private Func<Transform> m_getTarget;
    private List<TutorialLine> m_lines = new List<TutorialLine>();

    private void Start()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        Destroy(this.gameObject);
#endif
    }

    private void LateUpdate()
    {
        if (m_getTarget == null)
            return;

        if (SetHoleTarget())
        {
            hole.position = Vector3.Slerp(hole.position, m_targetRect.position, 0.5f);
            hole.sizeDelta = m_targetRect.rect.size + m_currentStep.HolePadding;
            SetTextPos();
            SetPointerPos();
        }
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
        
        swipeArrows.SetActive(m_currentStep.SwipeArrow);
        SetGetTarget();
        SetLines();
        SetInteractable();
        SetHiddenBehaviour();
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
        m_currentStep = null;
        m_getTarget = null;
        m_targetRect = null;
        hole.gameObject.SetActive(false);
        holeButton.gameObject.SetActive(false);
        NextCollider.gameObject.SetActive(true);
        Collider.SetActive(false);
        Bubble.localPosition = Vector3.zero;
        Bubble.gameObject.SetActive(false);
        pointer.localPosition = Vector3.zero;
        pointer.gameObject.SetActive(false);
        swipeArrows.SetActive(false);

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
        bool hasNextButton = !m_currentStep.ClickableHole && !m_currentStep.Skippable;
        for (int x = m_currentStep.linesList.Count - 1; x >= 0; --x)
        {
            TutorialLine newLine = Instantiate(LinePrefab, Bubble);
            newLine.transform.SetAsFirstSibling();
            int topPadding = x == 0 ? (hasNextButton ? 50 : 38) : 0;
            int bottomPadding = x == m_currentStep.linesList.Count - 1? 38 : 15;
            newLine.Init(m_currentStep.linesList[x], topPadding, bottomPadding);
            m_lines.Add(newLine);
        }
    }

    private void SetInteractable()
    {
        NextButtonContainer.SetActive(!m_currentStep.ClickableHole && !m_currentStep.Skippable);
        holeButton.gameObject.SetActive(m_currentStep.ClickableHole);
        NextCollider.interactable = !m_currentStep.ClickableHole || m_currentStep.Skippable;
    }

    private void SetGetTarget()
    {
        Bubble.gameObject.SetActive(!TutorialController.GetTargetPool.TryGetValue(m_currentStep.SavedProgress, out m_getTarget));
    }

    private bool SetHoleTarget()
    {
        try { m_targetRect = m_getTarget() as RectTransform; } catch (Exception) { }
        bool hasTarget = m_targetRect != null;
        hole.gameObject.SetActive(hasTarget);
        Bubble.gameObject.SetActive(hasTarget);

        return hasTarget;
    }

    private void SetTextPos()
    {
        float offsetMultiplicator = m_currentStep.Position == Enums.Position.Down ? -1f : 1f;
        float YOffset = offsetMultiplicator * 50f;
        YOffset += hole.localPosition.y + (offsetMultiplicator * hole.rect.height / 2.0f);
        YOffset += offsetMultiplicator * Bubble.rect.height / 2.0f;
        Bubble.localPosition = Vector3.up * YOffset;
    }

    private void SetPointerPos()
    {
        pointer.gameObject.SetActive(m_currentStep.ClickableHole);
        if (m_currentStep.ClickableHole)
        {
            pointer.rotation = Quaternion.Euler(0.0f, 0.0f, Bubble.position.y < m_targetRect.position.y ? 0.0f : 180.0f);
            pointer.localPosition = new Vector3( hole.localPosition.x, Bubble.localPosition.y + pointer.up.y * (15.0f + Bubble.rect.height / 2.0f), 0.0f);
        }
    }

    public void NextButton(bool forceAction)
    {
        UnityAction nextAction;
        if((!m_currentStep.Skippable || forceAction) && TutorialController.NextActionPool.TryGetValue(m_currentStep.SavedProgress, out nextAction))
            nextAction();

        StartCoroutine(CheckConditionsAndContinue());
    }

    private IEnumerator CheckConditionsAndContinue()
    {
        Func<IEnumerator> conditionForNext;
        if (TutorialController.ConditionForNextPool.TryGetValue(m_currentStep.SavedProgress, out conditionForNext))
            yield return conditionForNext();
        Reset();

        TutorialController.Instance.NextStep();
    }
}
