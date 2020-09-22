using UnityEngine;
using System.Collections.Generic;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Pages/Page Popup")]
    public class PagePopup : ScriptableObject
    {
        [SerializeField]
        public string pageId;
        [SerializeField]
        public string headline;
        [SerializeField]
        public List<string> content;
    }
}
