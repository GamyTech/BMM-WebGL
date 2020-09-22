using UnityEngine;
using System.Collections.Generic;

namespace GT.Assets
{
    [CreateAssetMenu(menuName = "Assets/Pages/Page")]
    public class PageData : ScriptableObject
    {
        [SerializeField]
        public string PageId;
        [SerializeField]
        public string ParentStoreType;
        [SerializeField]
        public string StoreType;
        [SerializeField]
        public string NavigationState;
        [SerializeField]
        public string NavigationCategory;
        [SerializeField]
        public string BackTransitionPageId;
        [SerializeField]
        public string Title;
        [SerializeField, Tooltip("Enables scroll rect")]
        public bool IsScrollable;
        [SerializeField]
        public bool NoMiddlePadding;
        [SerializeField, Tooltip("Enables PC Quick Navigation")]
        public bool AcessPCQuickNavigation;
        [SerializeField, Tooltip("Enables Padding to Keep fixed size MiddleWidget's content")]
        public bool EnablePadding;
        [SerializeField, Tooltip("Enables size content fitter")]
        public bool IsSizeToFitContent;
        [SerializeField, Tooltip("Middle widgets position between 0 (Bottom) to 1 (Top)")]
        public float MidWidgetsPosition;
        [SerializeField]
        public List<string> FloatingTopWidgets = new List<string>();
        [SerializeField]
        public List<string> TopWidgets = new List<string>();
        [SerializeField]
        public List<string> MiddleWidgets = new List<string>();
        [SerializeField]
        public List<string> BottomWidgets = new List<string>();
        [SerializeField]
        public List<string> FloatingBottomWidgets = new List<string>();

        public override string ToString()
        {
            return PageId.ToString();
        }
    }
}
