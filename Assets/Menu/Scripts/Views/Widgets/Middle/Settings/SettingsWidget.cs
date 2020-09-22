using UnityEngine.UI;
using UnityEngine;

public class SettingsWidget : Widget
{
    public Toggle soundToggle;
    public Toggle musicToggle;
    public Toggle vibrationToggle;
    public GameObject ResolutionPanel;
    public GameObject VibrationPanel;
    public GameObject LanguagePanel;

    public override void EnableWidget()
    {
        base.EnableWidget();

#if UNITY_ANDROID || UNITY_IOS
        ResolutionPanel.gameObject.SetActive(false);
#else
        VibrationPanel.gameObject.SetActive(false);
#if UNITY_WEBGL
        LanguagePanel.gameObject.SetActive(false);
        ResolutionPanel.gameObject.SetActive(false);
#endif
#endif

        soundToggle.isOn = SettingsController.Instance.SFXOn;
        musicToggle.isOn = SettingsController.Instance.MusicOn;
        vibrationToggle.isOn = SettingsController.Instance.VibrateOn;
    }

    #region Inputs
    public void MusicToggle(bool isOn)
    {
        SettingsController.Instance.TurnMusic(isOn);
    }
    public void SoundEffectsToggle(bool isOn)
    {
        SettingsController.Instance.TurnSoundEffect(isOn);
    }
    public void VibrationToggle(bool isOn)
    {
        SettingsController.Instance.TurnVibration(isOn);
    }
    #endregion Inputs
}
