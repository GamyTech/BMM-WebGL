using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GT.Backgammon.Player;
using GT.Backgammon.Logic;
using GT.Assets;
using GT.Websocket;
using System;
using System.Collections.Generic;

public class GlobalInfoView : MonoBehaviour
{

    #region Public Members
    public Text StateTimeText;
    public GameObject DisplayPanel;
    public Text StateGlobalInfoText;
    #endregion Public Members

    #region Public Functions
    public void Reset()
    {
        DisplayPanel.SetActive(false);
        StateTimeText.text = "00:00:00";
    }

    public void SetStateTime(TimeSpan time)
    {
        StateTimeText.text = time.Hours.ToString("00") + ":" + time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00");
    }

    public void Say(string info)
    {
        DisplayPanel.SetActive(true);
        StateGlobalInfoText.text = info.ToUpper();
    }
    #endregion Public Functions
}
