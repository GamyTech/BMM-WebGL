using UnityEngine;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/GameSpecificData")]
    public class GameSpecificData : ScriptableObject
    {
        public Sprite background;
        public Sprite PCBackground;
        public Sprite logo;
        public Sprite[] GameTutorialPictures = new Sprite[7];
    }
}
