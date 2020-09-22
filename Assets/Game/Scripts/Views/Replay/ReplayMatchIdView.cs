using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReplayMatchIdView : MonoBehaviour {

    public InputField MatchIdInputField;
    public Text CurrentMatchIdText;

    private void Awake()
    {
        ReplayGameController.Instance.OnLoadNewMatch += LoadNewMatch;
    }

    private void OnDestroy()
    {
        ReplayGameController.Instance.OnLoadNewMatch -= LoadNewMatch;
    }

    public void SearchMatchID()
    {
        ReplayGameController.Instance.RequestMatch(MatchIdInputField.text);
    }

    private void LoadNewMatch(string matchId, List<GT.Backgammon.GameState> logs)
    {
        CurrentMatchIdText.text = "Curent: " + matchId;
    }

}
