using UnityEngine;
using System.Collections;
using System;

namespace UnityEngine.UI
{
    public class HorizontalDynamicContentLayoutGroup : HorizontalOrVertcalDynamicContentLayoutGroup
    {
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalcAlongAxis(0, false);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LayoutUtility.GetPreferredSize(rectTransform, 0));
        }

        public override void CalculateLayoutInputVertical()
        {
            base.CalcAlongAxis(1, false);
        }

        public override void SetLayoutHorizontal()
        {
            base.SetChildrenAlongAxis(0, false);
        }

        public override void SetLayoutVertical()
        {
            base.SetChildrenAlongAxis(1, false);
        }
    }
}
