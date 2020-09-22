using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
    public class SmoothResize : UIBehaviour
    {
        enum OnReachedSizeModifier
        {
            None,
            Disable,
        }

        public delegate void SizeChanged(Vector2 newSize);

        public event SizeChanged OnSizeChanged;

        const float epsilon = 0.001f;

        private float xVelocity;
        private float yVelocity;

        private bool xStopped = false;
        private bool yStopped = false;

        public float smoothTime = .15f;

        [SerializeField]
        private OnReachedSizeModifier zeroSizeModifier = OnReachedSizeModifier.None;

        [SerializeField]
        private Vector2 m_targetSize;
        private LayoutElement m_layoutElement;
        private RectTransform m_childTransform;

        public Vector2 targetSize
        {
            get { return m_targetSize; }
            set { m_targetSize = value; }
        }

        public float targetHeight
        {
            get { return targetSize.y; }
            set
            {
                Vector2 size = targetSize;
                size.y = value;
                targetSize = size;
            }
        }

        public float targetWidth
        {
            get { return targetSize.x; }
            set
            {
                Vector2 size = targetSize;
                size.x = value;
                targetSize = size;
            }
        }

        protected LayoutElement layoutElement
        {
            get
            {
                if (m_layoutElement == null)
                    m_layoutElement = GetComponent<LayoutElement>();
                return m_layoutElement;
            }
        }

        protected RectTransform childTransform
        {
            get
            {
                if (m_childTransform == null)
                    m_childTransform = transform.GetChild(0).transform as RectTransform;
                return m_childTransform;
            }
        }

        protected virtual void LateUpdate()
        {
            AutoResize();
        }

        private void AutoResize()
        {
            bool isChanged = false;
            // when target size changed extend layout element size to match
            if (Mathf.Abs(Mathf.Max(layoutElement.minWidth, targetSize.x) - layoutElement.preferredWidth) > epsilon && targetSize.x >= 0)
            {
                xStopped = false;
                float newSizeX = Mathf.SmoothDamp(layoutElement.preferredWidth, Mathf.Max(layoutElement.minWidth, targetSize.x), ref xVelocity, smoothTime);
                layoutElement.preferredWidth = newSizeX;
                isChanged = true;
            }
            else if (xStopped == false)
            {
                xStopped = true;
                layoutElement.preferredWidth = Mathf.Max(layoutElement.minWidth, targetSize.x);
                isChanged = true;
                if (targetSize.x == 0)
                {
                    executeModifier(zeroSizeModifier, true);
                }
            }

            if (Mathf.Abs(Mathf.Max(layoutElement.minHeight, targetSize.y) - layoutElement.preferredHeight) > epsilon && targetSize.y >= 0)
            {
                yStopped = false;
                float newSizeY = Mathf.SmoothDamp(layoutElement.preferredHeight, Mathf.Max(layoutElement.minHeight, targetSize.y), ref yVelocity, smoothTime);
                layoutElement.preferredHeight = newSizeY;
                isChanged = true;
            }
            else if (yStopped == false)
            {
                yStopped = true;
                layoutElement.preferredHeight = Mathf.Max(layoutElement.minHeight, targetSize.y);
                isChanged = true;
                if(targetSize.y == 0)
                {
                    executeModifier(zeroSizeModifier, false);
                }
            }
            if (isChanged)
            {
                if (OnSizeChanged != null)
                    OnSizeChanged(new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight));
            }
        }

        private void setMaxHeight(int extra = 0)
        {
            gameObject.SetActive(true);
            targetHeight = childTransform.sizeDelta.y + extra;
        }

        private void setMinHeight(int extra = 0)
        {
            targetHeight = 0 + extra;
        }

        private void setMaxWidth(int extra = 0)
        {
            gameObject.SetActive(true);
            targetWidth = childTransform.sizeDelta.x + extra;
        }

        private void setMinWidth(int extra = 0)
        {
            targetWidth = 0 + extra;
        }

        /// <summary>
        /// Set resizer height state
        /// </summary>
        /// <param name="state">0 - Min, 1 - Max</param>
        /// <param name="extra">Extra size</param>
        public void SetHeightState(int state, int extra = 0)
        {
            Canvas.ForceUpdateCanvases();
            if (state == 0)
            {
                setMinHeight(extra);
            }
            else if(state == 1)
            {
                setMaxHeight(extra);
            }
            else
            {
                Debug.LogError("Resizer invalid state");
            }
        }

        /// <summary>
        /// Set resizer width state
        /// </summary>
        /// <param name="state">0 - Min, 1 - Max</param>
        /// <param name="extra">Extra size</param>
        public void SetWidthState(int state, int extra = 0)
        {
            Canvas.ForceUpdateCanvases();
            if (state == 0)
            {
                setMinWidth(extra);
            }
            else if(state == 1)
            {
                setMaxWidth(extra);
            }
            else
            {
                Debug.LogError("Resizer invalid state");
            }
        }

        #region Aid Functions
        private void executeModifier(OnReachedSizeModifier modifier, bool isWidth)
        {
            switch (modifier)
            {
                case OnReachedSizeModifier.Disable:
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        #endregion Aid Functions

        #region Reset
        public void ResetSize(float width, float height)
        {
            ResetHeight(height);
            ResetWidth(width);
        }

        public void ResetHeight(float height = 0)
        {
            layoutElement.preferredHeight = height;
            targetHeight = height;
        }

        public void ResetWidth(float width = 0)
        {
            layoutElement.preferredWidth = width;
            targetWidth = width;
        }
        #endregion Reset

    }
}
