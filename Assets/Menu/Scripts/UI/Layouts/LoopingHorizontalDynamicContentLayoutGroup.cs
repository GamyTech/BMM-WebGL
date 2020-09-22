namespace UnityEngine.UI
{
    public class LoopingHorizontalDynamicContentLayoutGroup : HorizontalDynamicContentLayoutGroup
    {
        [SerializeField]
        protected bool m_LoopIfAllFit;

        /// <summary>
        ///   <para>Scroll and loop elements if the size of the view is big enough to fit all of them</para>
        /// </summary>
        public bool LoopIfAllFit
        {
            get
            {
                return this.m_LoopIfAllFit;
            }
            set
            {
                base.SetProperty<bool>(ref this.m_LoopIfAllFit, value);
            }
        }

        private int midElementIndex;
        
        private bool isScrollable = true;
        private int startIndex;
        private float skipped;

        protected override void AddElementAndInit(IDynamicElement element)
        {
            base.AddElementAndInit(element);
        }

        protected override void SetChildrenAlongNonVerticalAxis(int axis)
        {
            float contentSize = base.rectTransform.rect.size[axis];

            CalculateViewParams(axis);

            int elementCount = base.elementsList.Count;
            if (elementCount <= 0)
                return;
            
            if (!CheckScrollability(contentSize - (viewBoundMax - viewBoundMin)))
            {
                base.SetChildrenAlongNonVerticalAxis(axis);
                return;
            }

            CalculateStartIndexAndSkippedSpace(axis, rectTransform.localPosition.x);
            
            float midPoint = (viewBoundMin + viewBoundMax) * 0.5f;

            int index = startIndex;
            do
            {
                IDynamicElement element = elementsList[index];
                float runningSize = GetRunningSizeOf(element, axis, t, runningFlexible);

                float boundMin = runningMin - skipped;
                float boundMax = runningSize + runningMin - skipped;

                if (boundMin <= midPoint && boundMax >= midPoint)
                    midElementIndex = index;

                HandleElement(element, boundMin <= viewBoundMax && boundMax >= viewBoundMin);
                if (element.activeObject != null)
                    base.SetChildAlongAxis(element.activeObject, axis, runningMin - skipped, runningSize);

                runningMin += runningSize + this.spacing;

                if (boundMin > viewBoundMax)
                    break;

                index = ++index >= elementCount ? 0 : index;

            } while (index != startIndex);
        }

        private bool CheckScrollability(float contentOversize)
        {
            bool isScrollable = (LoopIfAllFit || contentOversize > 0);

            if (this.isScrollable != isScrollable)
            {
                this.isScrollable = isScrollable;
                parentScrollRect.horizontal = isScrollable;
                transform.localPosition = new Vector3(isScrollable ? 0 : -contentOversize * 0.5f, 0, 0);
            }
            return isScrollable;
        }

        private void CalculateStartIndexAndSkippedSpace(int axis, float space)
        {
            startIndex = 0;
            
            if (space < 0)
                CalculateStartIndexAndSkippedSpaceRight(axis, space);
            else
                CalculateStartIndexAndSkippedSpaceLeft(axis, space);
        }

        private void CalculateStartIndexAndSkippedSpaceLeft(int axis, float space)
        {
            float spaceLeft = space;
            int elementCount = base.elementsList.Count;
            while (spaceLeft > 0)
            {
                --startIndex;
                if (startIndex < 0)
                    startIndex = elementCount - 1;
                IDynamicElement element = base.elementsList[startIndex];
                float size = GetRunningSizeOf(element, axis, t, runningFlexible);

                spaceLeft -= size + this.spacing;
            }

            skipped = space - spaceLeft;
        }

        private void CalculateStartIndexAndSkippedSpaceRight(int axis, float space)
        {
            float spaceLeft = space;
            float size = 0;
            int elementCount = base.elementsList.Count;
            while (spaceLeft < 0)
            {
                IDynamicElement element = base.elementsList[startIndex];
                size = GetRunningSizeOf(element, axis, t, runningFlexible);

                spaceLeft += size + this.spacing;
                if (spaceLeft < 0)
                    if (++startIndex >= elementCount)
                        startIndex = 0;
            }

            skipped = space - spaceLeft + size + this.spacing;
        }

        internal bool IsScrollable()
        {
            return isScrollable;
        }

        public Vector3 GetPagePosition(int page, int axis)
        {
            IDynamicElement element = base.elementsList[page];

            if (element.activeObject == null)
                HandleElement(element, true);

            Vector3[] corners = new Vector3[4];
            viewPort.GetLocalCorners(corners);
            Vector3 viewPortMid = (corners[2] - corners[0]) * 0.5f;

            return element.activeObject.localPosition - viewPortMid;
        }

        public int GetMidPageIndex()
        {
            return midElementIndex;
        }
    }
}
