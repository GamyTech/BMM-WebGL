using UnityEngine;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Store Item/Chat Data")]
    public class ChatItemData : StoreItemData
    {
        [SerializeField]
        public string text;
        [SerializeField]
        public GameObject prefab;
    }
}
