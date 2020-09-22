using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameSettingView : MonoBehaviour
{
    #region Public Members
    public GameObject SurrenderView;
    public GameObject ClickCollider;
    public GameTutorialView TutorialView;
    public Toggle SoundToggle;
    public Toggle MusicToggle;
    public Toggle VibrateToggle;
    public List<GameObject> HidenObjectOnPC = new List<GameObject>();
    public GameObject ResolutionObject;
    public Text ResolutionText;
    public Button NextResButton;
    public Button PreviousResButton;

    #endregion Public Members;

    void Awake()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        for (int x = 0; x < HidenObjectOnPC.Capacity; ++x)
            HidenObjectOnPC[x].SetActive(false);
#endif

#if !UNITY_STANDALONE
        ResolutionObject.SetActive(false);
#endif
    }

    void OnEnable()
    {
        Init();
    }

#region Private Functions
    private void Init()
    {
        SetToggle(MusicToggle, SettingsController.Instance.MusicOn);
        SetToggle(SoundToggle, SettingsController.Instance.SFXOn);
        SetToggle(VibrateToggle, SettingsController.Instance.VibrateOn);

#if UNITY_STANDALONE
        SetRes(SettingsController.currentResIndex);
#endif
    }

    private void SetToggle(Toggle t, bool isOn)
    {
        t.isOn = isOn;
        t.GetComponent<Animator>().SetBool("isOn", isOn);
        t.GetComponent<Animator>().Play(isOn ? "On" : "Off");
    }

    private void SetToggleAnimation(Toggle t, bool isOn)
    {
        t.GetComponent<Animator>().SetBool("isOn", isOn);
    }
#endregion Private Functions

#region Public Functions

    public void Hide()
    {
        if (!gameObject.activeSelf)
            return;

        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.ViewHide);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (gameObject.activeSelf)
            return;

        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.ViewShow);
        gameObject.SetActive(true);
    }

    public void OpenTutorialView()
    {
        Debug.Log("OpenTutorialView");
        TutorialView.Show();
        Hide();
    }

    public void ResignButton()
    {
        Debug.Log("ResignButton");
        SurrenderView.SetActive(true);
        Hide();
    }

    public void SoundToggleAction(bool isOn)
    {
        SettingsController.Instance.TurnSoundEffect(isOn);
        SetToggleAnimation(SoundToggle, isOn);
    }

    public void MusicToggleAction(bool isOn)
    {
        SettingsController.Instance.TurnMusic(isOn);
        SetToggleAnimation(MusicToggle, isOn);
    }


    public void VibrationToggleAction(bool isOn)
    {
        SettingsController.Instance.TurnVibration(isOn);
        SetToggleAnimation(VibrateToggle, isOn);
    }

    public void NextResolution()
    {
        int newRes;
        if (SettingsController.currentResIndex == -1)
            return;
        
        if (SettingsController.currentResIndex >= SettingsController.FixedResolutions.Count - 1)
            newRes = -1;
        else
            newRes = SettingsController.currentResIndex + 1;
        
        SetRes(newRes);
    }

    public void PreviousResolution()
    {
        int newRes;
        if (SettingsController.currentResIndex == 0)
            return;
         
        if (SettingsController.currentResIndex == - 1)
            newRes = SettingsController.FixedResolutions.Count - 1;
        else
            newRes = SettingsController.currentResIndex - 1;
           
        SetRes(newRes);
    }

    private void SetRes(int index)
    {
        NextResButton.interactable = index != -1;
        PreviousResButton.interactable = index != 0;
        ResolutionText.text = index == -1 ? "Full Screen" : SettingsController.FixedResolutions[index].x + "X" + SettingsController.FixedResolutions[index].y;
        SettingsController.Instance.SetResolution(index);
    }

#endregion Public Functions

}
