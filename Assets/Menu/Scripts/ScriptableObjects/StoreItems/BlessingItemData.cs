using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Store Item/Blessing Data")]
    public class BlessingItemData : StoreItemData
    {
        [SerializeField]
        public GameObject prefab;
    }
}
