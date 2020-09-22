using UnityEngine;

public class GameSoundController : BaseSoundController
{
    private static GameSoundController m_instance;
    public static GameSoundController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<GameSoundController>()); } }

    public AudioClip DefaultMusic;
    public AudioClip DefaultWinSound;
    public AudioClip DefaultRollSound;

    private bool m_vibrate;
    private AudioClip m_winSound;
    private AudioClip m_playerOneDiceRollSound;
    private AudioClip m_playerOneDiceIdleSound;
    private AudioClip m_playerTwoDiceRollSound;
    private AudioClip m_playerTwoDiceIdleSound;

    void Start()
    {
        if(SettingsController.Instance != null)
            SettingsController.Instance.RegisterSoundController(Instance);
    }

    private void OnDestroy()
    {
        m_instance = null;
    }

    public void SetBoardSounds(AudioClip background, AudioClip win)
    {
        if (win == null)
            m_winSound = DefaultWinSound;
        else
            m_winSound = win;
        MusicSource.clip = background ?? DefaultMusic;
        MusicSource.Play();
    }

    public void SetPlayerOneDiceSounds(AudioClip roll, AudioClip idle)
    {
        if (roll == null)
            m_playerOneDiceRollSound = DefaultRollSound;
        else
            m_playerOneDiceRollSound = roll;
        m_playerOneDiceIdleSound = idle;
    }

    public void SetPlayerTwoDiceSounds(AudioClip roll, AudioClip idle)
    {
        if (roll == null)
            m_playerTwoDiceRollSound = DefaultRollSound;
        else
            m_playerTwoDiceRollSound = roll;
        m_playerTwoDiceIdleSound = idle;
    }

    public void PlayNonSpecificEffect(Enums.GameSound sound)
    {
        if (AssetController.Instance != null)
        {
            SoundData data = AssetController.Instance.GetSoundData(sound.ToString());
            if (data != null)
            {
                SoundEffectSource[0].clip = data.clips[Random.Range(0, data.clips.Count)];
                SoundEffectSource[0].loop = data.isLoop;
                SoundEffectSource[0].Play();
            }
        }
    }

    public void PlayVictorySound()
    {
        SoundEffectSource[0].clip = m_winSound;
        SoundEffectSource[0].loop = false;
        SoundEffectSource[0].Play();
    }

    public void PlayDiceSound(bool playerOne, bool animationStop)
    {
        if (animationStop && SoundEffectSource.Count > 1)
        {
            SoundEffectSource[1].clip = playerOne ? m_playerOneDiceIdleSound : m_playerTwoDiceIdleSound;
            SoundEffectSource[1].Play();
        }
        else
        {
            SoundEffectSource[0].clip = playerOne ? m_playerOneDiceRollSound : m_playerTwoDiceRollSound;
            SoundEffectSource[0].loop = false;
            SoundEffectSource[0].Play();
        }
    }

    public void StopSecondSoundEffect()
    {
        if(SoundEffectSource.Count > 1)
            SoundEffectSource[1].Stop();
    }
}
