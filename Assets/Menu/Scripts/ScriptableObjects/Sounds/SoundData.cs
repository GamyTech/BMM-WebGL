using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Sound Data")]
public class SoundData : ScriptableObject
{
    [SerializeField]
    public List<AudioClip> clips;
    [SerializeField]
    public bool isLoop;
    [SerializeField]
    public string soundId;
}
