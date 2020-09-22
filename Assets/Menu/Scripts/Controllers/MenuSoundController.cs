using UnityEngine;

public class MenuSoundController : BaseSoundController
{
    private static MenuSoundController m_instance;
    public static MenuSoundController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<MenuSoundController>()); }}

    void Awake()
    {
        SettingsController.Instance.RegisterSoundController(Instance);
        ExtendedInputModule.OnSelectableSubmitted += PlayClickSound;
    }

    private void OnDestroy()
    {
        m_instance = null;
        ExtendedInputModule.OnSelectableSubmitted -= PlayClickSound;
    }

    private void PlayClickSound()
    {
        Play(Enums.MenuSound.Click);
    }

    public void Play(Enums.MenuSound sound)
    {
        SoundData data = AssetController.Instance.GetSoundData(sound.ToString());
        if (data == null)
            return;

        SoundEffectSource[0].clip = data.clips[Random.Range(0, data.clips.Count)];
        SoundEffectSource[0].loop = data.isLoop;
        SoundEffectSource[0].Play();
    }

    public void Play(AudioClip clip, bool loop = false)
    {
        SoundEffectSource[0].clip = clip;
        SoundEffectSource[0].loop = loop;
        SoundEffectSource[0].Play();
    }

    public void Stop()
    {
        for(int x= 0; x < SoundEffectSource.Count; ++x)
        {
            SoundEffectSource[x].Stop();
            SoundEffectSource[x].clip = null;
        }
    }
}
