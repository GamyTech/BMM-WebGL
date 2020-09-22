using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameTutorialView : MonoBehaviour
{
    public ScrollRect scrollRect;
    public IndicatorsView indicatorView;
    public Image BulbPicture;
    public Image[] BoardPictures = new Image[5];
    public Image[] BackGroundBoardPictures = new Image[5];
    public IndicatorsView indicatorsView;
    public List<GameObject> activeCategories = new List<GameObject>();

    #region Movement
    public float smoothTime = 0.1f;
    public float MinDelta = 100f;
    public float MaxTime = 0.25f;
    private float targetPos;
    private float velocity = 0;
    private float mouseStartPos;
    private bool isDragging = false;
    #endregion Movement

    #region Pages
    private int m_currentPlayCategoryIndex = 0;
    public int currentPlayCategoryIndex
    {
        get { return m_currentPlayCategoryIndex; }
        set
        {
            if (m_currentPlayCategoryIndex.Equals(value))
                return;

            m_currentPlayCategoryIndex = Mathf.Clamp(value, 0, totalCategories - 1);
            StartCoroutine(Utils.WaitForFrame(1,() => indicatorsView.SelectIndex(m_currentPlayCategoryIndex)));
        }
    }
    #endregion Pages

    private int totalCategories { get { return activeCategories.Count; } }

    void Start()
    {
        SetSprites();
        m_currentPlayCategoryIndex = 0;
        indicatorsView.Init(totalCategories, currentPlayCategoryIndex);
    }

    private void OnEnable()
    {
        currentPlayCategoryIndex = 0;
    }

    #region Private Methods
    private void SetSprites()
    {
        Sprite[] GameSpecifcSprites = AssetController.Instance.GetGameSpecific().GameTutorialPictures;
        if (GameSpecifcSprites.Length != 7)
            Debug.LogError("Not the right Number of Picture in GameSpecifc");
        else
        {
            for (int x = 0; x < BackGroundBoardPictures.Length; ++x)
                BackGroundBoardPictures[x].sprite = GameSpecifcSprites[0];
            BulbPicture.sprite = GameSpecifcSprites[1];

            BoardPictures[0].sprite = GameSpecifcSprites[2];
            BoardPictures[1].sprite = GameSpecifcSprites[3];
            BoardPictures[2].sprite = GameSpecifcSprites[4];
            BoardPictures[3].sprite = GameSpecifcSprites[5];
            BoardPictures[4].sprite = GameSpecifcSprites[6];
        }
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!isDragging)
            MoveToPage(currentPlayCategoryIndex);
    }

    private int getClosestPage(float pos, int totalPages)
    {
        return Mathf.RoundToInt(pos * (totalPages - 1));
    }

    private float getTargetPosition(int page, int totalPages)
    {
        return page / (float)(totalPages - 1);
    }

    private void MoveToPage(int page)
    {
        targetPos = getTargetPosition(page, totalCategories);
        Vector2 pos = scrollRect.normalizedPosition;
        if (Utils.Approximately(targetPos, pos.x, .0001f)) return;

        pos.x = Mathf.SmoothDamp(pos.x, targetPos, ref velocity, smoothTime);
        scrollRect.normalizedPosition = pos;
    }
    #endregion Private Methods

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #region Movement Handling
    public void OnEndDrag(BaseEventData eventData)
    {
        isDragging = false;
        float deltaX = mouseStartPos - ((PointerEventData)eventData).position.x;

        if (deltaX > 0 && deltaX > MinDelta)
            ++currentPlayCategoryIndex;
        else if (deltaX < 0 && deltaX < MinDelta)
            --currentPlayCategoryIndex;
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
        isDragging = true;
        mouseStartPos = ((PointerEventData)eventData).position.x;
    }

    public void OnClick(BaseEventData eventData)
    {
        if (!(eventData as PointerEventData).dragging)
            currentPlayCategoryIndex += ((PointerEventData)eventData).position.x > Screen.width * 0.5 ? 1 : -1;
    }

    #endregion Movement Handling

}
