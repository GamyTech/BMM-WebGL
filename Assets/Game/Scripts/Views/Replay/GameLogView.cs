using UnityEngine.Events;
using System.Collections.Generic;
using GT.Backgammon;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameLogView : MonoBehaviour {

    public Text LogIdText;
    public Text LogTypeText;
    public Text LogInfoText;
    public Text LogPlayerText;
    public Text LogTimeText;
    public Image SelectedImage;

    public bool IsSelected { get; private set; }

    private GameState m_state;

    public void Populate(GameState stateData)
    {
        m_state = stateData;
        RefreshSelected();

        //m_state.SelectAction += GameState_OnSelect;
        LogIdText.text = m_state.LogId.ToString();
        LogTypeText.text = m_state.LogType.ToString();
        LogTimeText.text = m_state.RelativeTime.Minutes.ToString() + ":" + m_state.RelativeTime.Seconds.ToString();

        string logInfo = "";
        string playercolor = "";
        switch (m_state.LogType)
        {
            case GameLogType.DoubleCubeYes:
                playercolor = m_state.CurrentPlayerColor.ToString();
                break;
            case GameLogType.SendDoubleCubeLogic:
                playercolor = m_state.CurrentPlayerColor.ToString();
                break;
            case GameLogType.SendMove:
                playercolor = m_state.CurrentPlayerColor.ToString();
                logInfo = "dice : " + m_state.CurrentDice[0].ToString() + "|" + m_state.CurrentDice[1].ToString();
                if (m_state.CurrentMoves == null || m_state.CurrentMoves.Length == 0)
                    logInfo += " No Possible Moves";
                else if (m_state.CurrentMoves.Length == 1)
                    logInfo += " One Possible Move";
                break;
            case GameLogType.StoppedGame:
                logInfo = "winner : " + (m_state.Winner != "Canceled" ? ReplayGameController.Instance.GetPlayerName(m_state.Winner): m_state.Winner);
                break;
            case GameLogType.StartMatch:
                logInfo = "Start";
                break;
            default:
                break;
        }

        LogInfoText.text = logInfo;
        LogPlayerText.text = playercolor;
    }

    public void RefreshSelected()
    {
        IsSelected = m_state.Selected;
        SelectedImage.gameObject.SetActive(IsSelected);
    }

    public void SelectButton()
    {
        ReplayGameController.Instance.JumpToState(m_state.Index);
    }
}
