using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Assets/TutorialStep")]
public class TutorialStep : ScriptableObject
{
    public string pageId = "Home";
    public Enums.TutorialProgress SavedProgress = Enums.TutorialProgress.Home_Welcome;
    public bool Skippable = false;
    public bool Hidden = false;
    public List<TutorialLineData> linesList = new List<TutorialLineData>();

    //Mobile Specific
    public bool ClickableHole = false;
    public bool SwipeArrow = false;
    public Vector2 HolePadding = Vector2.zero;

    //PC Specific
    public Enums.Position Position = Enums.Position.None;
}
