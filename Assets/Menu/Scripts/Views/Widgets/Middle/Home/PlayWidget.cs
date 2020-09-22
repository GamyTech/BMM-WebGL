using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using GT.User;
using System.Linq;

public class PlayWidget : Widget
{
    public IndicatorsView IndicatorsView;
    public ScrollRect scrollRect;
    public Transform Content;
    public CustomRoomCategoryView CustomRoomCatPrefab;
    public SingleBetRoomCategoryView SingleRoomCatPrefab;
    public MultiBetRoomCategoryView MultiRoomCatPrefab;
    public ObjectPool TourneyRoomCatPool;

    public Toggle TourneyToggle;
    public Toggle CashGamesToggle;

    private List<BetRoomCategoryView> categoryViews;

    #region Movement
    public float smoothTime = 0.1f;
    public float MinDelta = 50f;
    public float MaxTime = 0.25f;
    private float currentPos;
    private float targetPos;
    private float velocity = 0;
    private float mouseStartPos;
    private float startTime;
    private bool isDragging = false;
    #endregion Movement

    private enum GameType
    {
        CashGames = 0,
        Tourneys = 1
    }
    private GameType currentType;

    private int totalCategories { get { return Content.childCount; } }
    #region Pages
    public int CurrentCategoryIndex { get; protected set; }

    public BetRoomCategoryView CurrentTargetCategory { get; private set; }
    #endregion Pages

    public override void EnableWidget()
    {
        base.EnableWidget();

        TourneyController.Instance.OnTourneyListChanged += GTUser_OnTourneyListChanged;

        SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
        currentType = GameType.CashGames;
        CurrentCategoryIndex = user.LastSelectedCategory;
        categoryViews = new List<BetRoomCategoryView>();

        if (currentType == GameType.CashGames)
            CashGamesToggle.isOn = true;
        else
            TourneyToggle.isOn = true;

        ShowCurrentViews();
        UpdateTourneyListener();
    }

    public override void DisableWidget()
    {
        if (TourneyController.Instance != null)
            TourneyController.Instance.OnTourneyListChanged -= GTUser_OnTourneyListChanged;

        SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
        user.LastSelectedGameType = (int)currentType;
        user.LastSelectedCategory = CurrentCategoryIndex;
        SavedUsers.SaveUserToFile(user);

        if (currentType == GameType.Tourneys)
            UserController.Instance.StopListenToTourneysEvents();

        base.DisableWidget();
    }

    private void UpdateTourneyListener()
    {
        if (currentType == GameType.Tourneys)
            UserController.Instance.ListenToTourneysEvents();
        else
            UserController.Instance.StopListenToTourneysEvents();
    }

    private void ShowCurrentViews()
    {
        RemoveRooms();

        if (currentType == GameType.CashGames)
            CreateBetViewsForCash(ContentController.CashRoomsByCategory);
        else
        {
            if (TourneyController.Instance.showWelocomeMessage && !GTDataManagementKit.GetBoolFromPrefs(Enums.PlayerPrefsVariable.SawTourneyRules, false))
            {
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.SawTourneyRules, true.ToString());
                WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyRulesPopup);
            }

            if (TourneyController.Instance.tourneysList != null)
                CreateBetViewsForTourneys(SortTourneys(TourneyController.Instance.tourneysList));
        }

        Canvas.ForceUpdateCanvases();
        IndicatorsView.Init(totalCategories, CurrentCategoryIndex);
        InstantMoveToPage(CurrentCategoryIndex);
    }

    private List<Tourney> SortTourneys(List<Tourney> tourneysList)
    {
        return tourneysList.OrderBy(o => o.BuyIn).ToList();
    }

    private void CreateBetViewsForTourneys(List<Tourney> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            GameObject go = TourneyRoomCatPool.GetObjectFromPool();
            go.InitGameObjectAfterInstantiation(Content.transform);
            TourneyRoomCategoryView cat = go.GetComponent<TourneyRoomCategoryView>();
            cat.Init(rooms[i]);
            categoryViews.Add(cat);
        }
    }

    private void CreateBetViewsForCash(Dictionary<string, List<BetRoom>> rooms)
    {
        foreach (KeyValuePair<string, List<BetRoom>> item in rooms)
        {
            BetRoomCategoryView prefab;
            if (item.Key == ContentController.CustomCatId)
                prefab = CustomRoomCatPrefab;
            else if (item.Value.Count == 1)
                prefab = SingleRoomCatPrefab;
            else
                prefab = MultiRoomCatPrefab;

            RoomCategory catData = ContentController.GetCategoryData(item.Key);
            BetRoomCategoryView cat = Instantiate(prefab, Content.transform, false);

            int x = 0;
            while (x < categoryViews.Count && catData.Index > categoryViews[x].Category.Index)
                ++x;
            if (x == categoryViews.Count)
                cat.transform.SetAsLastSibling();
            else
                cat.transform.SetSiblingIndex(categoryViews[x].transform.GetSiblingIndex());

            cat.Init(item.Key, catData, item.Value);
            categoryViews.Add(cat);
        }
        if (categoryViews.Count > 0)
            CurrentTargetCategory = categoryViews[0];
    }

    private void RemoveRooms()
    {
        if (categoryViews != null)
        {
            for (int i = 0; i < categoryViews.Count; i++)
            {
                BetRoomCategoryView cat = categoryViews[i];
                if (cat is TourneyRoomCategoryView)
                    TourneyRoomCatPool.PoolObject(cat.gameObject);
                else
                    DestroyImmediate(cat.gameObject);
            }
            categoryViews.Clear();
        }
    }

    void Update()
    {
        HandleMovement();
    }

    private void ActivatePageIndication(int page)
    {
        IndicatorsView.SelectIndex(page);
    }

    #region Movement Handling
    public void OnValueChanged(Vector2 pos)
    {
        currentPos = pos.x;
    }

    void HandleMovement()
    {
        if (!isDragging)
            MoveToPage(CurrentCategoryIndex);
    }

    int getClosestPage(float pos, int totalPages)
    {
        return Mathf.RoundToInt(pos * (totalPages - 1));
    }

    float getTargetPosition(int page, int totalPages)
    {
        return page / (float)(totalPages - 1);
    }

    protected void MoveToPage(int page)
    {
        targetPos = getTargetPosition(page, totalCategories);
        Vector2 pos = scrollRect.normalizedPosition;
        if (Utils.Approximately(targetPos, pos.x, .0001f)) return;

        pos.x = Mathf.SmoothDamp(pos.x, targetPos, ref velocity, smoothTime);
        scrollRect.normalizedPosition = pos;

        ActivatePageIndication(page);
    }

    void InstantMoveToPage(int page)
    {
        targetPos = getTargetPosition(page, totalCategories);
        Vector2 pos = scrollRect.normalizedPosition;
        pos.x = targetPos;
        scrollRect.normalizedPosition = pos;

        ActivatePageIndication(page);
    }

    public void OnEndDrag(BaseEventData eventData)
    {
        isDragging = false;
        float deltaX = mouseStartPos - ((PointerEventData)eventData).position.x;
        float deltaT = Time.time - startTime;
        if (deltaT > MaxTime)
        {
            SetCurrentCategory(getClosestPage(currentPos, totalCategories));
        }
        else if (deltaX > MinDelta && deltaX > 0)
        {
            ChangeCurrentCategory(1);
            return;
        }
        else if (deltaX < MinDelta && deltaX < 0)
        {
            ChangeCurrentCategory(-1);
            return;
        }
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
        isDragging = true;
        mouseStartPos = ((PointerEventData)eventData).position.x;
        startTime = Time.time;
    }

    public void OnScroll(BaseEventData eventData)
    {
        if ((eventData as PointerEventData).scrollDelta.y > 0)
        {
            ChangeCurrentCategory(1);
        }
        else if ((eventData as PointerEventData).scrollDelta.y < 0)
        {
            ChangeCurrentCategory(-1);
        }
    }
    #endregion Movement Handling

    protected void ChangeCurrentCategory(int dif)
    {
        SetCurrentCategory(CurrentCategoryIndex + dif);
    }

    protected void SetCurrentCategory(int page)
    {
        CurrentCategoryIndex = Mathf.Max(0, Mathf.Min(totalCategories - 1, page));
        if (categoryViews.Count > CurrentCategoryIndex)
            CurrentTargetCategory = categoryViews[CurrentCategoryIndex];
    }

    private void ChangeGameType(GameType type)
    {
        if (currentType != type)
        {
            currentType = type;
            CurrentCategoryIndex = 0;
            ShowCurrentViews();
            UpdateTourneyListener();
        }
    }

    #region Input
    public void SelectTourneys(bool select)
    {
        if (select)
            ChangeGameType(GameType.Tourneys);
    }

    public void SelectCashGames(bool select)
    {
        if (select)
            ChangeGameType(GameType.CashGames);
    }
    #endregion Input

    #region Events 
    private void GTUser_OnTourneyListChanged(List<Tourney> tourneys)
    {
        if (currentType == GameType.Tourneys)
            ShowCurrentViews();
    }
    #endregion Events
}
