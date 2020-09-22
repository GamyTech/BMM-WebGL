using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClientReplayNavView : MonoBehaviour {

    public Toggle PlayToggle;
    public Text SpeedText;
    public List<GameObject> SpeedIndicators = new List<GameObject>();

    public List<float> MoveDuration = new List<float>();
    public List<float> BetweenActionDuration = new List<float>();
    public List<float> InfoDisplayDuration = new List<float>();
    public List<float> DiceSpeed = new List<float>();

    private int m_speed = 0;

    private void OnEnable()
    {
        ReplayGameController.Instance.OnLoadNewMatch += LoadNewMatch;
        ReplayGameController.Instance.OnStopPlaying += Reset;
        ReplayGameController.Instance.OnStartPlayingForward += OnPlay;
    }

    private void OnDisable()
    {
        if (ReplayGameController.Instance == null)
            return;

        ReplayGameController.Instance.OnLoadNewMatch -= LoadNewMatch;
        ReplayGameController.Instance.OnStopPlaying -= Reset;
        ReplayGameController.Instance.OnStartPlayingForward -= OnPlay;
    }

    public void Reset()
    {
        SetSpeed(0);
        PlayToggle.isOn = false;
    }

    private void OnPlay()
    {
        PlayToggle.isOn = true;
    }

    private void SetSpeed(int speed)
    {
        m_speed = speed;

        for(int x = 0; x < SpeedIndicators.Count; ++x)
            SpeedIndicators[x].SetActive(x <= speed);

        ReplayGameController.Instance.SetStepSpeed(MoveDuration[m_speed], BetweenActionDuration[m_speed], InfoDisplayDuration[m_speed], DiceSpeed[m_speed]);
        SpeedText.text = (m_speed + 1).ToString();
    }

    #region Input
    public void OnQuitButton()
    {
        SceneController.Instance.ChangeScene(SceneController.SceneName.Menu);
    }

    public void OnChangeSpeedButton()
    {
        ++m_speed;
        if (m_speed >= SpeedIndicators.Count)
            m_speed = 0;
        SetSpeed(m_speed);
    }

    public void OnPlayToggle(bool isOn)
    {
        if (isOn)
            ReplayGameController.Instance.PlayForward();
        else
            ReplayGameController.Instance.StopPlaying();
    }
    #endregion Input

    private void LoadNewMatch(string matchId, List<GT.Backgammon.GameState> logs)
    {
        Reset();
    }

}
