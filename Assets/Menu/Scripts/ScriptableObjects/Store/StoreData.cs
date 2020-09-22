using UnityEngine;
using GT.Store;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/StoreData")]
    public class StoreData : ScriptableObject
    {
        public Enums.StoreType storeType;
        public Enums.PageId pageId;
        public int selectedCount;
        public bool canBeUnselected;
        public bool canBeGiven;
        public string BuyString;
        public bool consumeOnDeselect;
        public GameObject itemPrefab;

        public AudioClip BoughtSound;
    }
}
