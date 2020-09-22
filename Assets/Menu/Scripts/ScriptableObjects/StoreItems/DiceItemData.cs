using UnityEngine;
using System.Collections.Generic;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Store Item/Dice Data")]
    public class DiceItemData : StoreItemData
    {
        [SerializeField]
        public List<Texture2D> topDicePictures;
        [SerializeField]
        public List<Texture2D> bottomDicePictures;
        [SerializeField]
        public DiceView prefab;
        [SerializeField]
        public AudioClip RollSound;
        [SerializeField]
        public AudioClip IdleSound;
    }
}
