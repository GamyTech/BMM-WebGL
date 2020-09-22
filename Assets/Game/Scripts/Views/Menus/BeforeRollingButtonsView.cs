using UnityEngine;
using UnityEngine.UI;

public class BeforeRollingButtonsView : MonoBehaviour {

    public Toggle AutoRollToggle;
    public Toggle RollToggle;
    public Toggle DoubleToggle;
    public Button OpenButton;
    public Text DoubleAmountText;
    public Text DoubleFeeText;
    public Text DoubleCubeText;
    public RectTransform ArrowButtonIcon;

    private bool isButtonsViewOpen = true;
    private bool currentButtonsBarState;

    public void AutoRoll(bool isOn)
    {
        RemoteGameController.Instance.isAuto = isOn;
        AutoRollToggle.gameObject.GetComponent<Animator>().SetBool("ToggleOn", isOn);
        HandleToggleChanged(isOn);

        if (isOn)
        {
            DisableDoubleToggle();
            DisableRollToggle();
        }
    }

    public void Double(bool isOn)
    {
        RemoteGameController.Instance.isDouble = isOn;
        DoubleToggle.gameObject.GetComponent<Animator>().SetBool("ToggleOn", isOn);
        HandleToggleChanged(isOn);

        if (isOn)
        {
            DisableAutoRollToggle();
            DisableRollToggle();
        }
    }

    public void Roll(bool isOn)
    {
        RemoteGameController.Instance.isRoll = isOn;
        RollToggle.gameObject.GetComponent<Animator>().SetBool("ToggleOn", isOn);
        HandleToggleChanged(isOn);

        if (isOn)
        {
            DisableAutoRollToggle();
            DisableDoubleToggle();
        }
    }

    public void DisableRollToggle()
    {
        RollToggle.isOn = false;
        RollToggle.gameObject.GetComponent<Animator>().SetBool("ToggleOn", RollToggle.isOn);
    }

    public void DisableDoubleToggle()
    {
        DoubleToggle.isOn = false;
        DoubleToggle.gameObject.GetComponent<Animator>().SetBool("ToggleOn", false);
    }

    public void DisableAutoRollToggle()
    {
        AutoRollToggle.isOn = false;
        AutoRollToggle.gameObject.GetComponent<Animator>().SetBool("ToggleOn", false);
    }

    public void SetDoubleButton(string doubleBet, string doubleFee)
    {
        if (TourneyController.Instance.IsInTourney())
        {
            DoubleAmountText.transform.parent.gameObject.SetActive(false);
            return;
        }
        DoubleAmountText.text = doubleBet;
        DoubleFeeText.text = Utils.LocalizeTerm("Fee") + " " + doubleFee;
    }

    public void SetDoubleCubeValue(int num)
    {
        DoubleCubeText.text = num.ToString();
    }

    public void PlayerCanDouble(bool active)
    {
        DoubleToggle.gameObject.SetActive(active);
    }

    public void SetActive(bool active)
    {
        GetComponent<Animator>().SetBool("IsOpen", active);
        isButtonsViewOpen = active;
        ArrowButtonIcon.localScale = new Vector3(1, isButtonsViewOpen ? 1 : -1);

        if (currentButtonsBarState != active)
            GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.BeforeRollBarHideShow);
        currentButtonsBarState = active;
    }

    public void ArrowButton()
    {
        SetActive(!isButtonsViewOpen);
    }

    public void ActivateArrowButton(bool b)
    {
        OpenButton.gameObject.SetActive(b);
    }

    public void WaitingForPlayerIndicators()
    {
        AutoRollToggle.gameObject.GetComponent<Animator>().SetTrigger("Waiting");
        RollToggle.gameObject.GetComponent<Animator>().SetTrigger("Waiting");
        DoubleToggle.gameObject.GetComponent<Animator>().SetTrigger("Waiting");
    }

    public void StopIndicators()
    {
        AutoRollToggle.gameObject.GetComponent<Animator>().SetTrigger("Stop");
        RollToggle.gameObject.GetComponent<Animator>().SetTrigger("Stop");
        DoubleToggle.gameObject.GetComponent<Animator>().SetTrigger("Stop");
    }

    public void Reset()
    {
        AutoRollToggle.isOn = false;
        RollToggle.isOn = false;
        DoubleToggle.isOn = false;
    }

    private void HandleToggleChanged(bool isOn)
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.BeforeRollToggle);
        if (isOn)
        {
            RemoteGameController.Instance.PlayerTurnActions();
            SetActive(false);
        }
    }
}
