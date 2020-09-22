using UnityEngine;
using UnityEngine.UI;

public class SupportReplayNavView : MonoBehaviour {

    public Toggle ForwardToggle;
    public Toggle BackwardToggle;

    private void OnEnable()
    {
        ReplayGameController.Instance.OnStartPlayingForward += OnStartPlayingForward;
        ReplayGameController.Instance.OnStopPlaying += Reset;
    }

    private void OnDisable()
    {
        ReplayGameController.Instance.OnStopPlaying -= Reset;
        ReplayGameController.Instance.OnStartPlayingForward -= OnStartPlayingForward;
    }

    public void Reset()
    {
        ForwardToggle.isOn = false;
        BackwardToggle.isOn = false;
    }

    #region Input
    public void JumpToFirstState()
    {
        Reset();
        ReplayGameController.Instance.JumpToFirstState();
    }

    public void JumpToLastState()
    {
        Reset();
        ReplayGameController.Instance.JumpToLastState();
    }

    public void MoveToNextState()
    {
        Reset();
        ReplayGameController.Instance.MoveToNextState();
    }

    public void MoveToPreviousState()
    {
        Reset();
        ReplayGameController.Instance.MoveToPreviousState();
    }

    public void PlayForward(bool isOn)
    {
        if (isOn)
            ReplayGameController.Instance.PlayForward();
        else
            ReplayGameController.Instance.StopPlaying();
    }

    public void PlayBackwardward(bool isOn)
    {
        if (isOn)
            ReplayGameController.Instance.PlayBackward();
        else
            ReplayGameController.Instance.StopPlaying();
    }
    #endregion Input

    private void OnStartPlayingForward()
    {
        ForwardToggle.isOn = true;
    }
}
