using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GT.Backgammon;

public class GameLogsView : MonoBehaviour {

    public SmoothScroll smoothScroll;
    public HorizontalOrVertcalDynamicContentLayoutGroup content;
    public ScrollRect scrollRect;
    public GameObject Container;

    private int logsCount;

    private void OnEnable()
    {
        ReplayGameController.Instance.OnShowStep += OnNewStepShown;
        ReplayGameController.Instance.OnLoadNewMatch += OnLoadNewMatch;
    }

    private void OnDisable()
    {
        ReplayGameController.Instance.OnShowStep -= OnNewStepShown;
        ReplayGameController.Instance.OnLoadNewMatch -= OnLoadNewMatch;
    }

    private void OnLoadNewMatch(string matchId, List<GameState> logs)
    {
        Reset();
        if (logs == null || logs.Count == 0)
            return;

        logsCount = logs.Count;
        content.SetElements(new List<IDynamicElement>(logs.ToArray()));
    }

    public void Reset()
    {
        content.ClearElements();
        logsCount = 0;
    }

    public void SetActive(bool active)
    {
        StopAllCoroutines();
        Container.SetActive(active);
        if (active && logsCount > 1)
            RefreshSlectedAndShow(ReplayGameController.Instance.ShownState);

    }

    private void OnNewStepShown(int index)
    {
        StopAllCoroutines();
        Container.SetActive(false);
        RefreshSlectedAndShow(index);
    }

    private void RefreshSlectedAndShow(int selectedIndex)
    {
        StartCoroutine(Utils.WaitForFrame(1,
            () =>
            {
                if(!Container.activeSelf)
                {
                    scrollRect.verticalScrollbar.value = 1f - ((float)selectedIndex / (float)(logsCount - 1));
                    GameLogView[] gameLogViewList = content.GetComponentsInChildren<GameLogView>();
                    for (int x = 0; x < gameLogViewList.Length; ++x)
                        gameLogViewList[x].RefreshSelected();
                }
            }));
    }
}
