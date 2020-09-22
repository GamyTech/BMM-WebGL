using UnityEngine;
using System.Collections.Generic;
using GT.Backgammon.Player;

namespace GT.Backgammon.View
{
    public class TimersView : MonoBehaviour, ITimersView
    {
        public TimerView topView;
        public TimerView bottomView;

        private Dictionary<PlayerColor, TimerView> playerSliderDict;

        public void InitView(bool isWhiteBottom)
        {
            playerSliderDict = new Dictionary<PlayerColor, TimerView>();
            playerSliderDict.Add(isWhiteBottom ? PlayerColor.White : PlayerColor.Black, bottomView);
            playerSliderDict.Add(isWhiteBottom ? PlayerColor.Black : PlayerColor.White, topView);
        }

        public void SetTimer(PlayerColor color, PlayerData data)
        {
            float turnValue = data.CurrentTurnTime / data.TotalTurnTime;
            float bankValue = data.CurrentBankTime / data.TotalBankTime;
            playerSliderDict[color].SetTimer(turnValue, bankValue);
        }
    }
}
