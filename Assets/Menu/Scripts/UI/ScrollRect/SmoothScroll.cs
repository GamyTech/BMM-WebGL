using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(ScrollRect)), ExecuteInEditMode]
    public class SmoothScroll : UIBehaviour, IInitializePotentialDragHandler
    {
        private const float epsilon = 0.001f;
        private float speedX = 0;
        private float speedY = 0;
        private bool targeting = false;

        public float smoothTime = 0.15f;
        [Range(0, 1)]
        private float targetHorizontalPos;

        [Range(0, 1)]
        private float targetVerticalPos;

        private ScrollRect m_scrollRect;
        private RectTransform m_rectTransform;

        protected ScrollRect scrollRect
        {
            get
            {
                if (m_scrollRect == null)
                    m_scrollRect = GetComponent<ScrollRect>();
                return m_scrollRect;
            }
        }

        protected RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        protected RectTransform contentRectTransform
        {
            get { return scrollRect.content; }
        }

        protected virtual void LateUpdate()
        {
            if (targeting == false) return;

            if (targetHorizontalPos != scrollRect.horizontalNormalizedPosition)
            {
                if (Utils.Approximately(targetHorizontalPos, scrollRect.horizontalNormalizedPosition, epsilon) == false)
                {
                    // still moving
                    scrollRect.horizontalNormalizedPosition = Mathf.SmoothDamp(scrollRect.horizontalNormalizedPosition, targetHorizontalPos, ref speedX, smoothTime);
                }
                else
                {
                    // just stopped
                    scrollRect.horizontalNormalizedPosition = targetHorizontalPos;
                    targeting = false;
                    speedX = 0;
                }
            }
            if (targetVerticalPos != scrollRect.verticalNormalizedPosition)
            {
                if (Utils.Approximately(targetVerticalPos, scrollRect.verticalNormalizedPosition, epsilon) == false)
                {
                    // still moving
                    scrollRect.verticalNormalizedPosition = Mathf.SmoothDamp(scrollRect.verticalNormalizedPosition, targetVerticalPos, ref speedY, smoothTime);
                }
                else
                {
                    // just stopped
                    scrollRect.verticalNormalizedPosition = targetVerticalPos;
                    targeting = false;
                    speedY = 0;
                }
            }
        }

        #region User Functions
        public void ScrollToMax(RectTransform to)
        {
            if (to == null || to.parent != contentRectTransform)
                return;

            targeting = true;

            if (scrollRect.vertical)
            {
                float contentHeightDifference = (contentRectTransform.rect.height - rectTransform.rect.height);
                float selectedPosition = (contentRectTransform.rect.height + to.localPosition.y);
                float currentScrollRectPosition = scrollRect.normalizedPosition.y * contentHeightDifference;
                float above = currentScrollRectPosition - (to.rect.height / 2) + rectTransform.rect.height;

                float step = selectedPosition - above;
                float newY = currentScrollRectPosition + step;
                targetVerticalPos = Mathf.Clamp01(newY / contentHeightDifference);

                if(targetVerticalPos == scrollRect.verticalNormalizedPosition)
                {
                    targeting = false;
                }
            }

            if (scrollRect.horizontal)
            {
                float contentWidthDifference = (contentRectTransform.rect.width - rectTransform.rect.width);
                float selectedPosition = to.localPosition.x;
                float currentScrollRectPosition = scrollRect.horizontalNormalizedPosition * contentWidthDifference;
                float left = currentScrollRectPosition + (to.rect.width / 2);

                float step = selectedPosition - left;
                float newX = currentScrollRectPosition + step;
                targetHorizontalPos = Mathf.Clamp01(newX / contentWidthDifference);

                if (targetHorizontalPos == scrollRect.horizontalNormalizedPosition)
                {
                    targeting = false;
                }
            }
        }

        public void ScrollToView(RectTransform to)
        {
            if (to == null || to.parent != contentRectTransform)
                return;

            targeting = true;

            if (scrollRect.vertical)
            {
                targetVerticalPos = scrollRect.verticalNormalizedPosition;

                float contentHeightDifference = (contentRectTransform.rect.height - rectTransform.rect.height);
                float selectedPosition = (contentRectTransform.rect.height + to.localPosition.y);
                float currentScrollRectPosition = scrollRect.verticalNormalizedPosition * contentHeightDifference;
                float above = currentScrollRectPosition - (to.rect.height / 2) + rectTransform.rect.height;
                float below = currentScrollRectPosition + (to.rect.height / 2);

                if (selectedPosition > above)
                {
                    float step = selectedPosition - above;
                    float newY = currentScrollRectPosition + step;
                    targetVerticalPos = Mathf.Clamp01(newY / contentHeightDifference);
                }
                else if (selectedPosition < below)
                {
                    float step = selectedPosition - below;
                    float newY = currentScrollRectPosition + step;
                    targetVerticalPos = Mathf.Clamp01(newY / contentHeightDifference);
                }

                if (targetVerticalPos == scrollRect.verticalNormalizedPosition)
                {
                    targeting = false;
                }
            }

            if (scrollRect.horizontal)
            {
                targetHorizontalPos = scrollRect.horizontalNormalizedPosition;

                float contentWidthDifference = (contentRectTransform.rect.width - rectTransform.rect.width);
                float selectedPosition = to.localPosition.x;
                float currentScrollRectPosition = scrollRect.horizontalNormalizedPosition * contentWidthDifference;
                float right = currentScrollRectPosition - (to.rect.width / 2) + rectTransform.rect.width;
                float left = currentScrollRectPosition + (to.rect.width / 2);

                if (selectedPosition < left)
                {
                    float step = selectedPosition - left;
                    float newX = currentScrollRectPosition + step;
                    targetHorizontalPos = Mathf.Clamp01(newX / contentWidthDifference);
                }
                else if (selectedPosition > right)
                {
                    float step = selectedPosition - right;
                    float newX = currentScrollRectPosition + step;
                    targetHorizontalPos = Mathf.Clamp01(newX / contentWidthDifference);
                }

                if (targetHorizontalPos == scrollRect.horizontalNormalizedPosition)
                {
                    targeting = false;
                }
            }
        }
        #endregion User Functions

        #region Stop Moving Event
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            targeting = false;
        }
        #endregion Stop Moving Event
    }
}
