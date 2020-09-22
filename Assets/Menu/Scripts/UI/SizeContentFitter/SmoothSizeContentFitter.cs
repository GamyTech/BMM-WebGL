using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    /// <summary>
    ///   <para>Resizes a RectTransform to fit the size of its content.</para>
    /// </summary>
    [ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public class SmoothSizeContentFitter : UIBehaviour, ILayoutController, ILayoutSelfController
    {
        /// <summary>
        ///   <para>The size fit mode to use.</para>
        /// </summary>
        public enum FitMode
        {
            /// <summary>
            ///   <para>Don't perform any resizing.</para>
            /// </summary>
            Unconstrained,
            /// <summary>
            ///   <para>Resize to the minimum size of the content.</para>
            /// </summary>
            MinSize,
            /// <summary>
            ///   <para>Resize to the preferred size of the content.</para>
            /// </summary>
            PreferredSize
        }

        private const float epsilon = 0.001f;

        [SerializeField]
        protected ContentSizeFitter.FitMode m_HorizontalFit;

        [SerializeField]
        protected ContentSizeFitter.FitMode m_VerticalFit;

        [SerializeField]
        protected float smoothTime = 0.15f;

        [NonSerialized]
        private RectTransform m_Rect;

        [NonSerialized]
        private float speedX = 0;
        [NonSerialized]
        private float speedY = 0;
        //[NonSerialized]
        public Vector2 targetSize;

        private DrivenRectTransformTracker m_Tracker;

        /// <summary>
        ///   <para>The fit mode to use to determine the width.</para>
        /// </summary>
        public ContentSizeFitter.FitMode horizontalFit
        {
            get
            {
                return this.m_HorizontalFit;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<ContentSizeFitter.FitMode>(ref this.m_HorizontalFit, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        ///   <para>The fit mode to use to determine the height.</para>
        /// </summary>
        public ContentSizeFitter.FitMode verticalFit
        {
            get
            {
                return this.m_VerticalFit;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<ContentSizeFitter.FitMode>(ref this.m_VerticalFit, value))
                {
                    this.SetDirty();
                }
            }
        }

        private RectTransform rectTransform
        {
            get
            {
                if (this.m_Rect == null)
                {
                    this.m_Rect = base.GetComponent<RectTransform>();
                }
                return this.m_Rect;
            }
        }

        protected SmoothSizeContentFitter()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
        }

        /// <summary>
        ///   <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            this.SetDirty();
        }

        protected virtual void LateUpdate()
        {
            if (m_HorizontalFit != ContentSizeFitter.FitMode.Unconstrained)
            {
                float preferredWidth = rectTransform.sizeDelta.x;
                if (targetSize.x != preferredWidth)
                {
                    if (Utils.Approximately(targetSize.x, preferredWidth, epsilon) == false)
                    {
                        // still moving
                        preferredWidth = Mathf.SmoothDamp(preferredWidth, targetSize.x, ref speedX, smoothTime);
                    }
                    else
                    {
                        // just stopped
                        preferredWidth = targetSize.x;
                        speedX = 0;
                    }
                }
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth);
            }

            if (m_VerticalFit != ContentSizeFitter.FitMode.Unconstrained)
            {
                float preferredHeight = rectTransform.sizeDelta.y;
                if (targetSize.y != preferredHeight)
                {
                    if (Utils.Approximately(targetSize.y, preferredHeight, epsilon) == false)
                    {
                        // still moving
                        preferredHeight = Mathf.SmoothDamp(preferredHeight, targetSize.y, ref speedY, smoothTime);
                    }
                    else
                    {
                        // just stopped
                        preferredHeight = targetSize.y;
                        speedY = 0;
                    }
                }
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
            }
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            ContentSizeFitter.FitMode fitMode = (axis != 0) ? this.verticalFit : this.horizontalFit;
            if (fitMode == ContentSizeFitter.FitMode.Unconstrained)
            {
                this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.None);
                return;
            }
            this.m_Tracker.Add(this, this.rectTransform, (axis != 0) ? DrivenTransformProperties.SizeDeltaY : DrivenTransformProperties.SizeDeltaX);
            if (fitMode == ContentSizeFitter.FitMode.MinSize)
            {
                if (axis != 0) // vertical
                {
                    targetSize.y = LayoutUtility.GetMinSize(this.m_Rect, axis);
                }
                else // horizontal
                {
                    targetSize.x = LayoutUtility.GetMinSize(this.m_Rect, axis);
                }
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {

                    this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetMinSize(this.m_Rect, axis));
                }
#endif
            }
            else
            {
                if (axis != 0) // vertical
                {
                    targetSize.y = LayoutUtility.GetPreferredSize(this.m_Rect, axis);
                }
                else // horizontal
                {
                    targetSize.x = LayoutUtility.GetPreferredSize(this.m_Rect, axis);
                }
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    this.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(this.m_Rect, axis));
                }
#endif
            }
        }

        /// <summary>
        ///   <para>Method called by the layout system.</para>
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
            this.m_Tracker.Clear();
            this.HandleSelfFittingAlongAxis(0);
        }

        /// <summary>
        ///   <para>Method called by the layout system.</para>
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            this.HandleSelfFittingAlongAxis(1);
        }

        /// <summary>
        ///   <para>Mark the ContentSizeFitter as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (!this.IsActive())
            {
                return;
            }
            LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            this.SetDirty();
        }
#endif
    }
}
