using System;

namespace UnityEngine.UI
{
    public abstract class HorizontalOrVertcalDynamicContentLayoutGroup : DynamicContentLayoutGroup
    {
        [SerializeField]
        protected float m_Spacing;

        [SerializeField]
        protected bool m_ChildForceExpandWidth = true;

        [SerializeField]
        protected bool m_ChildForceExpandHeight = true;

        #region View params
        protected float t;
        protected float runningFlexible;
        protected float runningMin;
        protected float viewBoundMin;
        protected float viewBoundMax;
        #endregion

        /// <summary>
        ///   <para>The spacing to use between layout elements in the layout group.</para>
        /// </summary>
        public float spacing
        {
            get
            {
                return m_Spacing;
            }
            set
            {
                base.SetProperty<float>(ref m_Spacing, value);
            }
        }

        /// <summary>
		///   <para>Whether to force the children to expand to fill additional available horizontal space.</para>
		/// </summary>
		public bool childForceExpandWidth
        {
            get
            {
                return m_ChildForceExpandWidth;
            }
            set
            {
                base.SetProperty<bool>(ref m_ChildForceExpandWidth, value);
            }
        }

        /// <summary>
        ///   <para>Whether to force the children to expand to fill additional available vertical space.</para>
        /// </summary>
        public bool childForceExpandHeight
        {
            get
            {
                return m_ChildForceExpandHeight;
            }
            set
            {
                base.SetProperty<bool>(ref m_ChildForceExpandHeight, value);
            }
        }


        /// <summary>
		///   <para>Calculate the layout element properties for this layout element along the given axis.</para>
		/// </summary>
		/// <param name="axis">The axis to calculate for. 0 is horizontal and 1 is vertical.</param>
		/// <param name="isVertical">Is this group a vertical group?</param>
		protected void CalcAlongAxis(int axis, bool isVertical)
        {
            float padding = (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal);
            float totalMinSize = padding;
            float totalPrefSize = padding;
            float totalFlexSize = 1f;
            bool flag = isVertical ^ axis == 1;
            for (int i = 0; i < base.elementsList.Count; i++)
            {
                IDynamicElement element = base.elementsList[i];
                float minSize = (float)((axis != 0) ? element.minSize.y : element.minSize.x);
                float prefSize = (float)((axis != 0) ? element.preferredSize.y : element.preferredSize.x);
                if (flag)
                {
                    totalMinSize = Mathf.Max(minSize + padding, totalMinSize);
                    totalPrefSize = Mathf.Max(prefSize + padding, totalPrefSize);
                    //totalFlexSize = (isVertical && childForceExpandHeight) || (!isVertical && childForceExpandWidth) ? 1f : 0f;
                }
                else
                {
                    totalMinSize += minSize + this.spacing;
                    totalPrefSize += prefSize + this.spacing;
                }
            }
            if(!flag && base.elementsList.Count > 0)
            {
                totalMinSize -= this.spacing;
                totalPrefSize -= this.spacing;
            }
            totalPrefSize = Mathf.Max(totalMinSize, totalPrefSize);
            base.SetLayoutInputForAxis(totalMinSize, totalPrefSize, totalFlexSize, axis);
        }

        /// <summary>
        ///   <para>Set the positions and sizes of the child layout elements for the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
        /// <param name="isVertical">Is this group a vertical group?</param>
        protected virtual void SetChildrenAlongAxis(int axis, bool isVertical)
        {
            bool flag = isVertical ^ axis == 1;
            if (flag)
            {
                SetChildrenAlongVerticalAxis(axis);
            }
            else
            {
                SetChildrenAlongNonVerticalAxis(axis);
            }
        }

        /// <summary>
        /// Set the positions and sizes of the child layout elements for the vertical axis.
        /// </summary>
        /// <param name="axis"></param>
        protected virtual void SetChildrenAlongVerticalAxis(int axis)
        {
            float contentSize = base.rectTransform.rect.size[axis];
            float padding = contentSize - (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal);
            for (int i = 0; i < elementsList.Count; i++)
            {
                IDynamicElement element = base.elementsList[i];
                float minSize = (float)((axis != 0) ? element.minSize.y : element.minSize.x);
                float prefSize = (float)((axis != 0) ? element.preferredSize.y : element.preferredSize.x);
                float flexSize = 0f;
                if (axis != 0 ? this.childForceExpandHeight : this.childForceExpandWidth)
                {
                    flexSize = 1f;
                }
                float num3 = Mathf.Clamp(padding, minSize, (flexSize <= 0f) ? prefSize : contentSize);
                float startOffset = base.GetStartOffset(axis, num3);

                if (element.activeObject != null)
                {
                    base.SetChildAlongAxis(element.activeObject, axis, startOffset, num3);
                }
            }
        }

        /// <summary>
        /// Set the positions and sizes of the child layout elements for the not vertical axis.
        /// </summary>
        /// <param name="axis"></param>
        protected virtual void SetChildrenAlongNonVerticalAxis(int axis)
        {
            CalculateViewParams(axis);

            for (int j = 0; j < base.elementsList.Count; j++)
            {
                IDynamicElement element = base.elementsList[j];
                float runningSize = GetRunningSizeOf(element, axis, t, runningFlexible);

                float boundMax = runningSize + runningMin;

                HandleElement(element, runningMin <= viewBoundMax && boundMax >= viewBoundMin);
                if (element.activeObject != null)
                    base.SetChildAlongAxis(element.activeObject, axis, runningMin, runningSize);

                runningMin += runningSize + this.spacing;

                if (runningMin > viewBoundMax)
                    return;
            }
        }

        protected void CalculateViewParams(int axis)
        {
            float contentSize = base.rectTransform.rect.size[axis];
            float contentFromViewOffset = (axis != 0) ? ((1 - rectTransform.pivot.y) * base.GetTotalPreferredSize(axis)) + rectTransform.localPosition.y :
                    (rectTransform.pivot.x * base.GetTotalPreferredSize(axis)) - rectTransform.localPosition.x;
            Vector3[] corners = new Vector3[4];
            viewPort.GetLocalCorners(corners);
            viewBoundMin = (axis != 0) ? corners[0].y : corners[0].x;
            viewBoundMax = (axis != 0) ? corners[1].y : corners[3].x;
            viewBoundMin += contentFromViewOffset;
            viewBoundMax += contentFromViewOffset;

            runningMin = (float)((axis != 0) ? base.padding.top : base.padding.left);
            if (base.GetTotalFlexibleSize(axis) == 0f && base.GetTotalPreferredSize(axis) < contentSize)
            {
                runningMin = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal));
            }
            if (base.GetTotalMinSize(axis) != base.GetTotalPreferredSize(axis))
            {
                t = Mathf.Clamp01((contentSize - base.GetTotalMinSize(axis)) / (base.GetTotalPreferredSize(axis) - base.GetTotalMinSize(axis)));
            }
            runningFlexible = 0f;
            if (contentSize > base.GetTotalPreferredSize(axis) && base.GetTotalFlexibleSize(axis) > 0f)
            {
                runningFlexible = (contentSize - base.GetTotalPreferredSize(axis)) / base.GetTotalFlexibleSize(axis);
            }
        }

        /// <summary>
        /// Get size of the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="axis"></param>
        /// <param name="t"></param>
        /// <param name="runningFlexible"></param>
        protected float GetRunningSizeOf(IDynamicElement element, int axis, float t, float runningFlexible)
        {
            float minSize = (float)((axis != 0) ? element.minSize.y : element.minSize.x);
            float prefSize = (float)((axis != 0) ? element.preferredSize.y : element.preferredSize.x);
            float flexSize = 0f;
            if ((axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth)
            {
                flexSize = 1f;
            }
            float runningSize = Mathf.Lerp(minSize, prefSize, t);
            runningSize += flexSize * runningFlexible;
            return runningSize;
        }

        /// <summary>
        /// Add element to elements list, 
        /// And initialize its size acording to prefab's layout element sizes
        /// </summary>
        /// <param name="element"></param>
        protected override void AddElementAndInit(IDynamicElement element)
        {
            base.AddElementAndInit(element);

            LayoutElement prefabElemant = null;
            if (poolPriority)
            {
                if (objectPool.objectPrefab != null)
                    prefabElemant = objectPool.objectPrefab.GetComponent<LayoutElement>();
                else if (element is ICustomPrefab && ((ICustomPrefab)element).Prefab != null)
                    prefabElemant = ((ICustomPrefab)element).Prefab.GetComponent<LayoutElement>();
            }
            else
            {
                if (element is ICustomPrefab && ((ICustomPrefab)element).Prefab != null)
                    prefabElemant = ((ICustomPrefab)element).Prefab.GetComponent<LayoutElement>();
                else if (objectPool.objectPrefab != null)
                    prefabElemant = objectPool.objectPrefab.GetComponent<LayoutElement>();
            }

            if (prefabElemant != null)
            {
                element.minSize =  new Vector2(prefabElemant.minWidth, prefabElemant.minHeight);
                element.preferredSize = new Vector2(prefabElemant.preferredWidth, prefabElemant.preferredHeight);
            }
            else
                Debug.LogError("Prefab of the element you trying to add missing layout element component");
        }
    }
}
