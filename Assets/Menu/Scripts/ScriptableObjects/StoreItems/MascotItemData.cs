using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Store Item/Mascot Data")]
    public class MascotItemData : StoreItemData
    {
        [SerializeField]
        public List<Texture2D> backGroundPictures;
        [SerializeField]
        public List<Texture2D> topPictures;
        [SerializeField]
        public GameObject prefab;
    }
}
