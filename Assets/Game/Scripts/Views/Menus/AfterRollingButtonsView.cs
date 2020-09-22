using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AfterRollingButtonsView : MonoBehaviour {

    #region Public Members
    public Button DoneButton;
    public Button UndoButton;

    public Image UndoIndicatorImage;
    public Image DoneIndicatorImage;

    public Sprite IndicatorOn;
    public Sprite IndicatorOff;
    #endregion Public Members

    #region Public Functions

    public void DisplayDoneButton(bool active)
    {
        DoneButton.gameObject.SetActive(active);
    }

    public void DisplayUndoButton(bool active)
    {
        UndoButton.gameObject.SetActive(active);
    }

    public void ChangeIndicatorUndo(bool active)
    {
        UndoIndicatorImage.sprite = active ? IndicatorOn : IndicatorOff;
    }

    public void ChangeIndicatorDone(bool active)
    {
        DoneIndicatorImage.sprite = active ? IndicatorOn : IndicatorOff;
        DoneIndicatorImage.color = active ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 1f, 1f, 1f);

    }

    public void Reset()
    {
        DoneButton.gameObject.SetActive(false);
        UndoButton.gameObject.SetActive(false);
    }

    public void Undo()
    {
        RemoteGameController.Instance.UndoMove();
    }

    public void EndTurn()
    {
        RemoteGameController.Instance.EndTurn();
    }
    #endregion Public Functions
}
