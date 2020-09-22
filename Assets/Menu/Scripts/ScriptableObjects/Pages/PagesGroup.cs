using UnityEngine;
using System.Collections.Generic;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Pages/Group")]
    public class PagesGroup : ScriptableObject
    {
        [SerializeField]
        public string groupId;
        [SerializeField]
        public List<string> pageList;
    }
}
