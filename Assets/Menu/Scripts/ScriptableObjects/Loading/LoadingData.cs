using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/LoadingData")]
    public class LoadingData : ScriptableObject
    {
        public Sprite Background;
        public Sprite PCBackground;
        public RuntimeAnimatorController logoAnim;
        public bool FadeToWhite;
    }
}
