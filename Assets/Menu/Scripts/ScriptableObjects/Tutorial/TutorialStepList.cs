using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Assets/TutorialStepList")]
public class TutorialStepList : ScriptableObject
{
    public List<TutorialStep> WelcomeSteps = new List<TutorialStep>();
}
