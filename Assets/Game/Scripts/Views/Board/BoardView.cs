using UnityEngine;
using System.Collections.Generic;
using GT.Backgammon.View;
using GT.Backgammon.Logic;
using System;
using GT.Backgammon.Player;
using System.Collections;

public class BoardView : BaseBoardView
{
    public Camera MainCamera;
    public SlotViewImp[] viewSlotObjects;

    public TextMesh TopPlayerStepsToWin;
    public TextMesh BottomPlayerStepsToWin;

    private Func<int, int> convertIndexFunc;
    private Func<int, int> convertColorIndexFunc;
    private Func<PlayerColor, int> convertEatenIndexFunc;

    private float m_moveDuration;

    public SpriteRenderer boardSR;
    public SpriteRenderer borderSideSR;
    public SpriteRenderer borderBottomSR;

    public Texture2D[] checkers;

    public override void InitView(float duration, bool isWhiteBottom)
    {
        m_moveDuration = duration;

        InitIndexFunctions(isWhiteBottom);
        SetEaten(isWhiteBottom);

        m_viewSlots = new ISlotView[viewSlotObjects.Length];

        for (int x = 0; x < viewSlotObjects.Length; ++x)
        {
            m_viewSlots[x] = viewSlotObjects[x];
            m_viewSlots[x].InitSlot(new ImpSlotViewData(OnSlotClicked, OnCheckerSelected, OnCheckerDropped, x, checkers));
        }

        ClearBoard();
    }

    public void ChangeMoveDuration(float duration)
    {
        m_moveDuration = duration;
    }

    public void ChangeBoard(string id)
    {
        StartCoroutine(LoadBoard(id));
    }

    public void LoadBoard(GT.Assets.BoardItemData boarData)
    {
        if (boarData == null)
            return;

        GameSoundController.Instance.SetBoardSounds(boarData.BackgroundMusic, boarData.WinSound);

        MainCamera.backgroundColor = boarData.BackgroundColor;

        boardSR.sprite = boarData.board.ToSprite();
        borderSideSR.sprite = boarData.borderSide.ToSprite();
        borderBottomSR.sprite = boarData.borderBottom.ToSprite();
        checkers = boarData.checkers.ToArray();
        if (m_viewSlots != null)
            for (int i = 0; i < m_viewSlots.Length; i++)
                if (m_viewSlots[i] != null)
                    (m_viewSlots[i] as SlotViewImp).UpdateCheckersSprite(checkers);
    }

    public IEnumerator LoadBoard(string boardId)
    {
        if (string.IsNullOrEmpty(boardId))
            boardId = "52";

        bool loaded = false;
        AssetController.Instance.GetBoardAsset(boardId, (d) => { LoadBoard(d); loaded = true; });

        while (!loaded)
            yield return null;
    }

    #region Input
    private void OnSlotClicked(int index)
    {
        if (index < 0) return;
        TriggerExecuteMove(m_viewSlots[index].LogicIndex, -1);
        possibleMovesDict = null;
        ClearIndicators();
    }

    private void OnCheckerSelected(int slotOriginIndex)
    {
        if (m_viewSlots != null && m_viewSlots.Length > slotOriginIndex && 
            possibleMovesDict.ContainsKey(m_viewSlots[slotOriginIndex].LogicIndex))
        {
            ISlotView slot = m_viewSlots[slotOriginIndex];
            ClearIndicators();
            ActivateIndicators(possibleMovesDict[slot.LogicIndex], Color.green);
            ActivateSlotsColliders(possibleMovesDict[slot.LogicIndex]);
        }
    }

    private void OnCheckerDropped(int slotOriginIndex, int slotTargetIndex, Checker checker)
    {
        ClearIndicators();

        if (slotOriginIndex == slotTargetIndex)
        {
            OnSlotClicked(slotOriginIndex);
            return;
        }

        ISlotView originSlot = m_viewSlots[slotOriginIndex];

        if (possibleMovesDict != null && (slotTargetIndex == -1 || !possibleMovesDict.ContainsKey(originSlot.LogicIndex) ||
            m_viewSlots == null || m_viewSlots.Length <= slotTargetIndex ||
            !possibleMovesDict[originSlot.LogicIndex].Contains(m_viewSlots[slotTargetIndex].LogicIndex)))
        {
            ActivateStartIndicators();
            ActivateSlotsColliders(possibleMovesDict.Keys);
            StartCoroutine(ReturnCheckerMove(originSlot, checker));
            return;
        }
        possibleMovesDict = null;
        TriggerExecuteMove(originSlot.LogicIndex, m_viewSlots[slotTargetIndex].LogicIndex);
    }
    #endregion Input

    #region Public Functions
    public void InitIndexFunctions(bool whitePerspective)
    {
        SetColor(whitePerspective);
        convertIndexFunc = ConvertIndexToCCWR;
    }

    public void SetStepsToWin(int topPlayerSteps, int bottomPlayerSteps)
    {
        TopPlayerStepsToWin.text = topPlayerSteps.ToString();
        BottomPlayerStepsToWin.text = bottomPlayerSteps.ToString();
    }
    #endregion Public Functions

    #region Private Functions
    private void ClearBoard()
    {
        for (int i = 0; i < m_viewSlots.Length; i++)
            m_viewSlots[i].ResetSlotView();
    }

    private void SetEaten(bool whitePerspective)
    {
        if (whitePerspective)
            convertEatenIndexFunc = c => { return Board.GetIndexOfEaten(c); };
        else
            convertEatenIndexFunc = c => { return Board.GetIndexOfEatenOpponent(c); };
    }

    private void PlayMoveSound()
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.Move);
    }

    private void PlayEatenSound()
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.Eat);
    }
    #endregion Private Functions

    #region Convert From Logic Index To View Index Functions
    private void SetColor(bool white)
    {
        if (white)
            convertColorIndexFunc = ConvertToWhiteIndex;
        else
            convertColorIndexFunc = ConvertToBlackIndex;
    }

    private int ConvertToBlackIndex(int index)
    {
        switch (index)
        {
            case 24: return 25;
            case 25: return 24;
            case 26: return 27;
            case 27: return 26;
            default: return index;
        }
    }

    private int ConvertToWhiteIndex(int index)
    {
        switch (index)
        {
            case 24: return 24;
            case 25: return 25;
            case 26: return 26;
            case 27: return 27;
            default: return 23 - index;
        }
    }

    /// <summary>
    /// Counter Clockwise + Right Bearoff
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int ConvertIndexToCCWR(int index)
    {
        switch (index)
        {
            case 24: return 25;
            case 25: return 24;
            case 26: return 26;
            case 27: return 27;
            default: return index;
        }
    }
    #endregion Convert From Logic Index To View Index Functions

    #region View Implementations

    public override IEnumerator Move(Move move)
    {

        SlotViewImp originSlot = m_viewSlots[ConvertLogicToViewIndex(move.from)] as SlotViewImp;
        SlotViewImp targetSlot = m_viewSlots[ConvertLogicToViewIndex(move.to)] as SlotViewImp;

        DisableCheckersColliders();

        GameObject checker1 = originSlot.GetChecker();

        if (move.isEaten)
        {
            SlotViewImp EatenSlot = m_viewSlots[GetEatenIndex(Board.GetOpponentsColor(move.color))] as SlotViewImp;
            GameObject checker2 = targetSlot.GetChecker();

            EatenSlot.AddCheckerAsChild(checker2);
            EatenSlot.SetSlotColor(Board.GetOpponentsColor(move.color));
            targetSlot.AddCheckerAsChild(checker1);

            yield return SmoothEatMove(targetSlot, EatenSlot, checker1, checker2);
            PlayEatenSound();
        }
        else
        {
            targetSlot.AddCheckerAsChild(checker1);

            yield return SmoothMove(targetSlot, originSlot, checker1, move.color);
            PlayMoveSound();
        }

        targetSlot.SetSlotColor(move.color);
        originSlot.SetSlotColorEmpty();

        StartCoroutine(targetSlot.SlotOrganize());
        StartCoroutine(originSlot.SlotOrganize());

        targetSlot.SetSortingOrder();

        ClearIndicators();
    }

    public override IEnumerator Undo(Move move)
    {
        SlotViewImp originSlot = m_viewSlots[ConvertLogicToViewIndex(move.from)] as SlotViewImp;
        SlotViewImp targetSlot = m_viewSlots[ConvertLogicToViewIndex(move.to)] as SlotViewImp;

        DisableCheckersColliders();

        GameObject checker1 = targetSlot.GetChecker();

        if (move.isEaten)
        {
            SlotViewImp EatenSlot = m_viewSlots[GetEatenIndex(Board.GetOpponentsColor(move.color))] as SlotViewImp;
            GameObject checker2 = EatenSlot.GetChecker();

            targetSlot.AddCheckerAsChild(checker2);
            originSlot.AddCheckerAsChild(checker1);

            targetSlot.SetSlotColor(Board.GetOpponentsColor(move.color));
            originSlot.SetSlotColor(move.color);

            yield return SmoothEatMove(originSlot, targetSlot, checker1, checker2, true);
            PlayEatenSound();
        }
        else
        {
            originSlot.AddCheckerAsChild(checker1);
            originSlot.SetCheckerToBoardSprite(move.color);

            originSlot.SetSlotColor(move.color);
            targetSlot.SetSlotColorEmpty();

            yield return SmoothMove(originSlot, targetSlot, checker1, move.color, true);
            PlayMoveSound();
        }

        StartCoroutine(targetSlot.SlotOrganize());
        StartCoroutine(originSlot.SlotOrganize());
        targetSlot.SetSortingOrder();
        originSlot.SetSortingOrder();

        ClearIndicators();
    }

    public void ActivateCheckersColliders()
    {
        if (possibleMovesDict != null)
        {
            for (int i = 0; i < m_viewSlots.Length; i++)
            {
                (m_viewSlots[i] as SlotViewImp).DisableCheckersCollider();
                (m_viewSlots[i] as SlotViewImp).DisableSlotCollider();
            }

            foreach (var item in possibleMovesDict)
            {
                (m_viewSlots[ConvertLogicToViewIndex(item.Key)] as SlotViewImp).SetCheckersCollider();
                (m_viewSlots[ConvertLogicToViewIndex(item.Key)] as SlotViewImp).EnableSlotCollider();
            }
        }
    }

    public void ActivateSlotsColliders(IEnumerable<int> collection)
    {
        for (int i = 0; i < m_viewSlots.Length; i++)
            (m_viewSlots[i] as SlotViewImp).DisableSlotCollider();
        foreach (var item in collection)
            (m_viewSlots[ConvertLogicToViewIndex(item)] as SlotViewImp).EnableSlotCollider();
    }

    public void DisableCheckersColliders()
    {
        for (int i = 0; i < m_viewSlots.Length; i++)
            (m_viewSlots[i] as SlotViewImp).DisableCheckersCollider();
    }

    public override void ShowMovesIndicators()
    {
        ClearIndicators();
        ActivateCheckersColliders();

        if (possibleMovesDict == null)
            return;

        ActivateStartIndicators();
    }

    private void ActivateStartIndicators()
    {
        if (possibleMovesDict.ContainsKey(Board.EATEN_BLACK_INDEX))
            ActivateIndicators(possibleMovesDict[Board.EATEN_BLACK_INDEX], Color.green);
        else if (possibleMovesDict.ContainsKey(Board.EATEN_WHITE_INDEX))
            ActivateIndicators(possibleMovesDict[Board.EATEN_WHITE_INDEX], Color.green);
        else
            base.ShowMovesIndicators();
    }

    protected override int ConvertLogicToViewIndex(int logicIndex)
    {
        return convertIndexFunc(convertColorIndexFunc(logicIndex));
    }

    protected override int ConvertViewToLogicIndex(int viewIndex)
    {
        return convertColorIndexFunc(convertIndexFunc(viewIndex));
    }

    protected override int GetEatenIndex(PlayerColor color)
    {
        return convertIndexFunc(convertEatenIndexFunc(color));
    }

    protected override void ClearIndicators()
    {
        for (int i = 0; i < m_viewSlots.Length; i++)
            m_viewSlots[i].ResetIndicator();
    }

    #endregion View Implementations

    #region Movement

    public IEnumerator ReturnCheckerMove(ISlotView slot, Checker checkerObject)
    {
        BoxCollider slotCollider = (slot as SlotViewImp).GetComponent<BoxCollider>();
        Checker checker = checkerObject.GetComponent<Checker>();

        slotCollider.enabled = false;
        checkerObject.boxCollider.enabled = false;
        checkerObject.spriteRenderer.sortingOrder = 200;
        Vector3 v = (slot as SlotViewImp).GetIndex();

        yield return Change.GenericChange(checkerObject.transform.position, v, m_moveDuration, Change.EaseOutQuad, p =>
        {
            checker.isMoving = true;
            checkerObject.transform.position = p;
        }, null);

        checker.isMoving = false;
        slotCollider.enabled = true;
        checkerObject.boxCollider.enabled = true;
        checkerObject.boxCollider.size = new Vector3(0.6f, 0.8f, 0.6f);
        (slot as SlotViewImp).SetSortingOrder();
    }

    public IEnumerator SmoothMove(SlotViewImp slot, SlotViewImp originSlot, GameObject checkerObject, PlayerColor color, bool isUndo = false)
    {
        BoxCollider checkerCollider = checkerObject.GetComponent<BoxCollider>();
        Checker checker = checkerObject.GetComponent<Checker>();

        checkerCollider.enabled = false;

        StartCoroutine(slot.SlotOrganize());
        StartCoroutine(originSlot.SlotOrganize());
        checkerObject.GetComponent<SpriteRenderer>().sortingOrder = 200;
        Vector3 v = slot.GetIndex();
        yield return Change.GenericChange(checkerObject.transform.position, v, m_moveDuration, Change.EaseOutQuad, p =>
        {
            checker.isMoving = true;
            checkerObject.transform.position = p;
        }, null);

        checker.isMoving = false;
        slot.SetCheckerToBearOffSprite(color);
        slot.SetSortingOrder();

        checkerCollider.enabled = true;
    }

    public IEnumerator SmoothEatMove(SlotViewImp slotFrom, SlotViewImp slotEaten, GameObject checkerObject, GameObject checkerEatenObject, bool isUndo = false)
    {
        float counter = 0;

        Checker eaterChecker = checkerObject.GetComponent<Checker>();
        eaterChecker.boxCollider.enabled = false;
        eaterChecker.spriteRenderer.sortingOrder = 200;

        Checker checkerEaten = checkerEatenObject.GetComponent<Checker>();
        checkerEaten.boxCollider.enabled = false;
        checkerEaten.spriteRenderer.sortingOrder = 200;

        Vector3 toPosition = new Vector3(slotFrom.transform.position.x, slotFrom.transform.position.y + (slotFrom.spacingDelta * (slotFrom.Quantity - 1)), 0f);
        Vector3 toEatenPosition = new Vector3(slotEaten.transform.position.x, slotEaten.transform.position.y + (slotEaten.spacingDelta * (slotEaten.Quantity - 1)), 0f);
        Vector3 fromPosition = eaterChecker.transform.position;
        Vector3 fromEatenPosition = checkerEaten.transform.position;

        while (counter < m_moveDuration)
        {
            eaterChecker.transform.position = Change.EaseOutQuad(fromPosition, toPosition, counter / m_moveDuration);
            checkerEaten.transform.position = Change.EaseOutQuad(fromEatenPosition, toEatenPosition, counter / m_moveDuration);
            yield return null;
            counter += Time.deltaTime;
        }

        StartCoroutine(slotFrom.SlotOrganize());
        StartCoroutine(slotEaten.SlotOrganize());
        slotFrom.SetSortingOrder();
        slotEaten.SetSortingOrder();

        eaterChecker.boxCollider.enabled = true;
        checkerEaten.boxCollider.enabled = true;
    }

    #endregion Movement
}

