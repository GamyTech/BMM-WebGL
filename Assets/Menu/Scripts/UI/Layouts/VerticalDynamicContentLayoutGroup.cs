using UnityEngine;
using System.Collections;
using System;

namespace UnityEngine.UI
{
    public class VerticalDynamicContentLayoutGroup : HorizontalOrVertcalDynamicContentLayoutGroup
    {
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalcAlongAxis(0, true);
        }

        public override void CalculateLayoutInputVertical()
        {
            base.CalcAlongAxis(1, true);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LayoutUtility.GetPreferredSize(rectTransform, 1));
        }

        public override void SetLayoutHorizontal()
        {
            base.SetChildrenAlongAxis(0, true);
        }

        public override void SetLayoutVertical()
        {
            base.SetChildrenAlongAxis(1, true);
        }
    }
}
