using UnityEngine;

[CreateAssetMenu(menuName = "Assets/MedalData")]
public class MedalData : ScriptableObject
{
    public Texture2D MedalTexture;
    public string MedalName;
    public int PointsPerLevel;
}
