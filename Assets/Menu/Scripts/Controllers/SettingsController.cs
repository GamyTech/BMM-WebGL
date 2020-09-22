using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsController : MonoBehaviour
{
    public delegate void ScreenSizeChanged(int width, int height);
    public static event ScreenSizeChanged OnScreenSizeChanged;

    private static SettingsController m_instance;
    public static SettingsController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<SettingsController>()); }}

    public List<Vector2> Resolutions = new List<Vector2>();

    public bool VibrateOn { get; private set; }
    public bool SFXOn { get; private set; }
    public bool MusicOn { get; private set; }
    private BaseSoundController m_soundController;


    public static int lastWidth { get; private set; }
    public static int lastHeight { get; private set; }

    public static List<Vector2> FixedResolutions { get; private set; }
    public static int HighestResIndex { get; private set; }
    public static int currentResIndex { get; private set; }

#if UNITY_STANDALONE
    public static bool FirstTime = true;
#endif

    void Awake()
    {
        SFXOn = GTDataManagementKit.GetBoolFromPrefs(Enums.PlayerPrefsVariable.Sfx, true);
        MusicOn = GTDataManagementKit.GetBoolFromPrefs(Enums.PlayerPrefsVariable.Music, true);
        VibrateOn = GTDataManagementKit.GetBoolFromPrefs(Enums.PlayerPrefsVariable.Vibration, true);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        lastWidth = Screen.width;
        lastHeight = Screen.height;

        FixedResolutions = new List<Vector2>();
        for (int x = 0; x < Resolutions.Count; ++x)
            if (Resolutions[x].x <= Screen.currentResolution.width && Resolutions[x].y <= Screen.currentResolution.height)
                FixedResolutions.Add(Resolutions[x]);

#if UNITY_STANDALONE
        if (FirstTime)
            SetResolution(GTDataManagementKit.GetIntFromPrefs(Enums.PlayerPrefsVariable.Resolution, -1));
        FirstTime = false;
#endif
    }


    void FixedUpdate()
    {
        if (lastWidth != Screen.width || lastHeight != Screen.height)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            StartCoroutine(SendSizeChangedEvent());
        }
#if UNITY_STANDALONE
        if (currentResIndex == -1 && Input.GetKeyDown(KeyCode.Escape))
            SetResolution(HighestResIndex);
#endif
    }

    public void RegisterSoundController(BaseSoundController instance)
    {
        m_soundController = instance;

        if (m_soundController.MusicSource != null)
            m_soundController.MusicSource.mute = !MusicOn;

        for (int x = 0; x < m_soundController.SoundEffectSource.Count; ++x)
            m_soundController.SoundEffectSource[x].mute = !SFXOn;
    }

    public void TurnMusic(bool isOn)
    {
        MusicOn = isOn;
        if (m_soundController != null && m_soundController.MusicSource != null)
            m_soundController.MusicSource.mute = !MusicOn;
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.Music, isOn.ToString());
    }

    public void TurnSoundEffect(bool isOn)
    {
        SFXOn = isOn;
        if (m_soundController != null)
            for (int x = 0; x < m_soundController.SoundEffectSource.Count; ++x)
                m_soundController.SoundEffectSource[x].mute = !SFXOn;
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.Sfx, isOn.ToString());
    }

    public void TurnVibration(bool isOn)
    {
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.Vibration, isOn.ToString());
        VibrateOn = isOn;
    }

    public void Vibrate()
    {
        Debug.Log("Vibrate");
#if UNITY_IOS || UNITY_ANDROID
        if (VibrateOn)
            Handheld.Vibrate();
#endif
    }

    private void SetHighestRes()
    {
        HighestResIndex = 0;
        for (int x = 0; x < FixedResolutions.Capacity; ++x)
        {
            int FixedSizeWidth = (int)FixedResolutions[x].x;
            int FixedSizedHeight = (int)FixedResolutions[x].y;

            if (FixedSizeWidth > Screen.currentResolution.width || FixedSizedHeight > Screen.currentResolution.height)
                continue;

            if (FixedSizeWidth + FixedSizedHeight > FixedResolutions[HighestResIndex].x + FixedResolutions[HighestResIndex].y)
                HighestResIndex = x;
        }

        SetResolution(HighestResIndex);
    }

    public void SetResolution(int ResIndex)
    {
        if (currentResIndex == ResIndex)
            return;
        if (ResIndex == -1)
        {
            SetFullScreen();
            return;
        }

        currentResIndex = ResIndex;
        Screen.SetResolution((int)FixedResolutions[currentResIndex].x, (int)FixedResolutions[currentResIndex].y, false);
        OnScreenSizeChanged(Screen.currentResolution.width, Screen.currentResolution.height);
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.Resolution, currentResIndex);
    }


    public void SetFullScreen()
    {
        currentResIndex = -1;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        OnScreenSizeChanged(Screen.currentResolution.width, Screen.currentResolution.height);
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.Resolution, currentResIndex);
    }

    private IEnumerator SendSizeChangedEvent()
    {
        Canvas.ForceUpdateCanvases();
        yield return null;
        if (OnScreenSizeChanged != null)
            OnScreenSizeChanged(lastWidth, lastHeight);
    }
}

public class BaseSoundController : MonoBehaviour
{
    public AudioSource MusicSource;
    public List<AudioSource> SoundEffectSource = new List<AudioSource>();
}
