using UnityEngine;
using UnityEngine.UI;
using GT.Backgammon.Player;

namespace GT.Backgammon.View
{
    public class GameDataView : MonoBehaviour, IGameDataView
    {
        public Text p1NameText;
        public Text p2NameText;
        public Text betText;
        public Text feeText;
        public Text maxBetText;
        public Text cubeNumText;
        public Text cubeHolderText;

        public void InitGameData(params IPlayer[] players)
        {
            ResetView();
            SetPlayersData(players);
        }

        public void SetPlayersData(params IPlayer[] players)
        {
            p1NameText.text = players[0].playerData.UserName;
            p2NameText.text = players[1].playerData.UserName;
        }

        public void SetBetData(string bet, string fee, string maxBet, int cubeNum, string cubeHolderId)
        {
            betText.text = Utils.LocalizeTerm("Bet") + ": " + bet;
            feeText.text = Utils.LocalizeTerm("Fee") + ": " + fee;
            maxBetText.text = Utils.LocalizeTerm("Max Bet") + ": " + maxBet;
            cubeNumText.text = "Cube: " + cubeNum;
            cubeHolderText.text = "Holder: " + cubeHolderId;
        }

        public void ResetView()
        {
            cubeNumText.text = "Cube: 64";
            cubeHolderText.text = string.Empty;
            p1NameText.text = string.Empty;
            p2NameText.text = string.Empty;
            betText.text = string.Empty;
            feeText.text = string.Empty;
            maxBetText.text = string.Empty;
        }
    }
}
