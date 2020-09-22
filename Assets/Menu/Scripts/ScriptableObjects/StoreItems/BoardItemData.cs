using UnityEngine;
using System.Collections.Generic;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Store Item/Board Data")]
    public class BoardItemData : StoreItemData
    {
        [SerializeField]
        public Texture2D board;
        [SerializeField]
        public Texture2D borderSide;
        [SerializeField]
        public Texture2D borderBottom;
        [SerializeField]
        public List<Texture2D> checkers;
        [SerializeField]
        public AudioClip BackgroundMusic;
        [SerializeField]
        public AudioClip WinSound;
        [SerializeField]
        public Color BackgroundColor;

    }
}
