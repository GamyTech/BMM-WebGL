using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement_Widget : Widget {

    public VerticalDynamicContentLayoutGroup content;
    public ScrollRect scrollRect;
    public GameObject viewPort;
    public SmoothScroll smoothScroll;

    private List<Dictionary<string, object>> TestAchievementList
    {
        get
        {
            return new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
                new Dictionary<string, object>()
                {
                    { "description", "kill 3 pupies" },
                    { "step", 3 },
                    { "progress", 0 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "do it 5 times" },
                    { "step", 5 },
                    { "progress", 3 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "exist" },
                    { "step", 10 },
                    { "progress", 10 },
                    { "collected", false }
                },
                new Dictionary<string, object>()
                {
                    { "description", "change your userName" },
                    { "step", 1 },
                    { "progress", 1 },
                    { "collected", true }
                },
            };
        }
    }

    private List<Achievement> achievements = new List<Achievement>();
    public AchievementView currentTarget;
    private int currentTargetID;
    private bool haveCompleted = false;
    private bool haveUpcoming = false;

    public override void EnableWidget()
    {
        InitAchievements(TestAchievementList);
        base.EnableWidget();
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
    }

    private void Update()
    {
        if (currentTarget != null && currentTargetID != currentTarget.ID)
                currentTarget = null;
    }


    private void InitAchievements(List<Dictionary<string, object>> AchievementList)
    {
        content.ClearElements();
        achievements.Clear();
        haveCompleted = false;

        for (int x = 0;  x < AchievementList.Count; ++x)
        {
            object o;
            string description = "NO DESCRIPTION";
            if(AchievementList[x].TryGetValue("description", out o))        
                description = o.ToString();

            bool collected = false;
            if (AchievementList[x].TryGetValue("collected", out o))
                collected = o.ParseBool();

            int step = 1;
            if (AchievementList[x].TryGetValue("step", out o))
                step = o.ParseInt();

            int progress = 0;
            if (AchievementList[x].TryGetValue("progress", out o))
                progress = o.ParseInt();

            Achievement a = new Achievement(description, step, progress, collected, x);
            achievements.Add(a);

            if (!haveCompleted && a.step == a.progess)
                haveCompleted = true;
            if (!haveUpcoming && a.step != a.progess)
                haveUpcoming = true;
        }

        content.SetElements(new List<IDynamicElement>(achievements.ToArray()));

    }

    public void UpcomingButton()
    {
       if(haveUpcoming && scrollRect.verticalNormalizedPosition >= 0.0001f)
            StartCoroutine(TargetNextView(false));
    }
    public void CompletedButton()
    {
        if (haveCompleted && scrollRect.verticalNormalizedPosition >= 0.0001f)
            StartCoroutine(TargetNextView(true));
    }

    private IEnumerator TargetNextView(bool Completed)
    {
        AchievementView[] visibleAchievements;
        AchievementView MatchAchievement = null;
        do
        {
            yield return null;
            visibleAchievements = content.transform.GetComponentsInChildren<AchievementView>();
            int x = 0;
            do
            {
                if (visibleAchievements[x].Completed == Completed)
                {
                    if (currentTarget == null)
                        MatchAchievement = visibleAchievements[x];
                    else if (visibleAchievements[x].ID != currentTargetID)
                        MatchAchievement = visibleAchievements[x];
                }
                ++x;

            } while (MatchAchievement == null && x < visibleAchievements.Length) ;

            if (MatchAchievement == null)
                scrollRect.verticalNormalizedPosition -= Time.deltaTime * 0.5f;

        } while (MatchAchievement == null && scrollRect.verticalNormalizedPosition >= 0.0001f);

        if (MatchAchievement == null)
            yield break;

        currentTarget = MatchAchievement;
        currentTargetID = currentTarget.ID;
        smoothScroll.ScrollToMax(currentTarget.transform as RectTransform);

    }
}
