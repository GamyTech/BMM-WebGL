using UnityEngine;

public abstract class  BaseTutorialView : MonoBehaviour
{
    protected TutorialStep m_currentStep;

    public abstract void ShowNewStep(TutorialStep step);
    public abstract void CloseTutorial();
    public abstract void ShowInstruction(bool show);
}
