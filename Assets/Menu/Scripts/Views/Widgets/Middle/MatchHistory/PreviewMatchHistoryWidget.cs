using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GT.User;

public class PreviewMatchHistoryWidget : Widget
{
    private const int PREVIEWED_ITEMS = 3;

    public GameObject LastTourneyPreview;
    public ObjectPool tourneyMatchPool;
    public SmoothLayoutElement tourneyElement;

    public VerticalLayoutGroup contentLayout;
    public Text HeadlineText;
    public Button MoreHistoryButton;
    public ObjectPool matchPool;
    public SmoothLayoutElement element;

    private List<GameObject> activeObjectsList = new List<GameObject>();

    public override void EnableWidget()
    {
        base.EnableWidget();
        UserController.Instance.gtUser.OnMatchHistoryChanged += GtUser_OnMatchHistoryChanged;

        if (UserController.Instance.gtUser.MatchHistoryData.NeedToUpdate)
        {
            UserController.Instance.GetLastMatchHistory();

            if (UserController.Instance.gtUser.MatchHistoryData.ElementsList.Count > 0)
                Populate(UserController.Instance.gtUser.MatchHistoryData.ElementsList, UserController.Instance.gtUser.MatchHistoryData.lastTourney);
            else
                LoadingController.Instance.ShowPageLoading(transform as RectTransform);
        }
        else
            Populate(UserController.Instance.gtUser.MatchHistoryData.ElementsList, UserController.Instance.gtUser.MatchHistoryData.lastTourney);
    }

    public override void DisableWidget()
    {
        if (UserController.Instance != null && UserController.Instance.gtUser != null)
            UserController.Instance.gtUser.OnMatchHistoryChanged -= GtUser_OnMatchHistoryChanged;

        DestroyActiveObjects();
        base.DisableWidget();
    }

    private void Populate(List<FragmentedListDynamicElement> items, TourneyHistoryData tourney)
    {
        DestroyActiveObjects();
        MoreHistoryButton.gameObject.SetActive(items.Count > PREVIEWED_ITEMS);

        if (items.Count <= 0)
            HeadlineText.text = Utils.LocalizeTerm("Match History Is Empty").ToUpper();
        else
        {
            HeadlineText.text = Utils.LocalizeTerm("Completed Cash Games").ToUpper();

            for (int i = 0; i < Mathf.Min(PREVIEWED_ITEMS, items.Count); i++)
            {
                if (items[i] is MatchHistoryData)
                {
                    GameObject go = matchPool.GetObjectFromPool();
                    go.InitGameObjectAfterInstantiation(matchPool.transform);
                    go.GetComponent<MatchView>().Populate((MatchHistoryData)items[i]);
                    activeObjectsList.Add(go);
                }
            }

            Canvas.ForceUpdateCanvases();
            element.SetVerticalStateUsingLayoutSize(SmoothLayoutElement.State.Max);
        }

        if (tourney != null)
            ShowTourneyPreview(tourney);
        else
            LastTourneyPreview.SetActive(false);
    }

    private void ShowTourneyPreview(TourneyHistoryData tourney)
    {
        LastTourneyPreview.SetActive(true);
        
        GameObject go = tourneyMatchPool.GetObjectFromPool();
        go.InitGameObjectAfterInstantiation(tourneyMatchPool.transform);
        go.GetComponent<TourneyView>().Populate(tourney);

        Canvas.ForceUpdateCanvases();
        tourneyElement.SetVerticalStateUsingLayoutSize(SmoothLayoutElement.State.Max);
    }

    private void DestroyActiveObjects()
    {
        matchPool.PoolObjects(activeObjectsList);
        activeObjectsList.Clear();

        if (tourneyMatchPool.transform.childCount > 0)
            tourneyMatchPool.PoolObject(tourneyMatchPool.transform.GetChild(0).gameObject);
    }

    #region Events
    private void GtUser_OnMatchHistoryChanged(HistoryMatches newValue)
    {
        LoadingController.Instance.HidePageLoading();
        if (newValue != null)
            Populate(newValue.ElementsList, newValue.lastTourney);
    }
    #endregion Events

    #region Inputs
    public void ViewMoreHistory()
    {
        PageController.Instance.ChangePage(Enums.PageId.FullMatchHistory);
    }
    #endregion Inputs
}
