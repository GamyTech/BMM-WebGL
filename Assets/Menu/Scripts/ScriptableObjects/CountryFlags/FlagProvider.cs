using GT.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/FlagProvider")]
public class FlagProvider : ScriptableObject
{
    [System.Serializable()]
    public class LocalDictionary : SerializableDictionaryBase<string, Sprite> { }

    [SerializeField]
    public LocalDictionary CountryFlag;
}
