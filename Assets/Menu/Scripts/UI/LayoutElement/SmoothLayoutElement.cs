using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
    public class SmoothLayoutElement : LayoutElement
    {
        #region Enums
        public enum State
        {
            Zero,
            Min,
            Max,
        };

        public enum FinishedAction
        {
            Delete,
            Disable,
        }
        #endregion Enums

        public delegate void SizeChanged(Vector2 newSize);
        public event SizeChanged OnSizeChanged;

        private float speedX = 0;
        private float speedY = 0;

        private Action finishedAction;

        public float smoothTime = 0.15f;
        [SerializeField]
        private Vector2 m_targetSize;

        public Vector2 targetSize
        {
            get { return m_targetSize; }
            set
            {
                m_targetSize = value;
                finishedAction = null;
            }
        }

        public float targetWidth
        {
            get { return targetSize.x; }
            set { targetSize = new Vector2(value, targetSize.y); }
        }

        public float targetHeight
        {
            get { return targetSize.y; }
            set { targetSize = new Vector2(targetSize.x, value); }
        }

        private RectTransform m_rectTransform;
        private RectTransform m_childTransform;

        protected RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        protected RectTransform childTransform
        {
            get
            {
                if (m_childTransform == null)
                    m_childTransform = transform.GetChild(0).GetComponent<RectTransform>();
                return m_childTransform;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            speedX = 0;
            speedY = 0;

            //targetWidth = GetSize(GetPreferredWidth);
            //preferredWidth = targetWidth;
            //targetHeight = GetSize(GetPreferredHeight);
            //preferredHeight = targetHeight;
        }

        protected override void OnDisable()
        {
            if (targetWidth != preferredWidth)
            {
                preferredWidth = targetHeight;
            }

            if (targetHeight == preferredHeight)
            {
                preferredHeight = targetHeight;
            }

            if (finishedAction != null)
            {
                finishedAction();
                finishedAction = null;
            }
            base.OnDisable();
        }

        protected virtual void LateUpdate()
        {
            bool isChanged = false;
            if (m_targetSize.x != preferredWidth)
            {
                isChanged = true;
                if (Mathf.Approximately(m_targetSize.x, preferredWidth) == false)
                {
                    // still moving
                    preferredWidth = Mathf.SmoothDamp(preferredWidth, m_targetSize.x, ref speedX, smoothTime);
                }
                else
                {
                    // just stopped
                    preferredWidth = m_targetSize.x;
                    speedX = 0;
                    if (finishedAction != null)
                    {
                        finishedAction();
                        finishedAction = null;
                    }
                }
            }
            if (m_targetSize.y != preferredHeight)
            {
                isChanged = true;
                if (Mathf.Approximately(m_targetSize.y, preferredHeight) == false)
                {
                    // still moving
                    preferredHeight = Mathf.SmoothDamp(preferredHeight, m_targetSize.y, ref speedY, smoothTime);
                }
                else
                {
                    // just stopped
                    preferredHeight = m_targetSize.y;
                    speedY = 0;
                    if (finishedAction != null)
                    {
                        finishedAction();
                        finishedAction = null;
                    }
                }
            }
            if(isChanged)
            {
                if (OnSizeChanged != null)
                    OnSizeChanged(new Vector2(preferredWidth, preferredHeight));
            }
        }

        #region User Functions
        /// <summary>
        /// Resets the size to zero instantly
        /// </summary>
        public virtual void ResetSize()
        {
            preferredHeight = -1;
            preferredWidth = -1;
            targetSize = new Vector2(0, 0);
        }

        /// <summary>
        /// Set width with custom action when finished
        /// </summary>
        /// <param name="size">Width</param>
        /// <param name="action">Custom action when finished</param>
        public void SetWidth(float size, Action action = null)
        {
            targetWidth = size;
            SetAction(0, action);
        }

        /// <summary>
        /// Set width with known action when finished
        /// </summary>
        /// <param name="size">Width</param>
        /// <param name="action">Known action when finished</param>
        public void SetWidth(float size, FinishedAction action)
        {
            targetWidth = size;
            SetAction(0, CreateAction(action));
        }

        /// <summary>
        /// Set height with custom action when finished
        /// </summary>
        /// <param name="size">Height</param>
        /// <param name="action">Custom action when finished</param>
        public void SetHeight(float size, Action action = null)
        {
            targetHeight = size;
            SetAction(1, action);
        }

        /// <summary>
        /// Set height with known action when finished
        /// </summary>
        /// <param name="size">Height</param>
        /// <param name="action">Known action when finished</param>
        public void SetHeight(float size, FinishedAction action)
        {
            targetHeight = size;
            SetAction(1, CreateAction(action));
        }

        /// <summary>
        /// Set horizontal state using physical size with custom action when finished
        /// </summary>
        /// <param name="state">0 - Min, 1 - Max</param>
        /// <param name="action">Custom action when finished</param>
        /// <param name="extra">Extra width</param>
        public void SetHorizontalStateUsingPhysicalSize(int state, Action action = null, int extra = 0)
        {
            SetPhysicalState(0, state);
            SetAction(0, action);
        }

        /// <summary>
        /// Set horizontal state using physical size with known action when finished
        /// </summary>
        /// <param name="state">0 - Min, 1 - Max</param>
        /// <param name="action">Known action when finished</param>
        /// <param name="extra">Extra width</param>
        public void SetHorizontalStateUsingPhysicalSize(int state, FinishedAction action, int extra = 0)
        {
            SetPhysicalState(1, state);
            SetAction(0, CreateAction(action));
        }

        /// <summary>
        /// Set vertical state using physical size with custom action when finished
        /// </summary>
        /// <param name="state">0 - Min, 1 - Max</param>
        /// <param name="action">Custom action when finished</param>
        /// <param name="extra">Extra height</param>
        public void SetVerticalStateUsingPhysicalSize(int state, Action action = null, int extra = 0)
        {
            SetPhysicalState(1, state);
            SetAction(1, action);
        }

        /// <summary>
        /// Set vertical state using physical size with known action when finished
        /// </summary>
        /// <param name="state">0 - Min, 1 - Max</param>
        /// <param name="action">Known action when finished</param>
        /// <param name="extra">Extra height</param>
        public void SetVerticalStateUsingPhysicalSize(int state, FinishedAction action, int extra = 0)
        {
            SetPhysicalState(1, state);
            SetAction(1, CreateAction(action));
        }

        /// <summary>
        /// Set horizontal state using layout size with custom action when finished
        /// </summary>
        /// <param name="state">Layout state</param>
        /// <param name="action">Custom action when finished</param>
        /// <param name="extra">Extra width</param>
        public void SetHorizontalStateUsingLayoutSize(State state, Action action = null, int extra = 0)
        {
            SetLayoutState(0, state);
            SetAction(0, action);
        }

        /// <summary>
        /// Set horizontal state using layout size with known action when finished
        /// </summary>
        /// <param name="state">Layout state</param>
        /// <param name="action">Known action when finished</param>
        /// <param name="extra">Extra width</param>
        public void SetHorizontalStateUsingLayoutSize(State state, FinishedAction action, int extra = 0)
        {
            SetLayoutState(0, state);
            SetAction(0, CreateAction(action));
        }

        /// <summary>
        /// Set vertical state using layout size with custom action when finished
        /// </summary>
        /// <param name="state">Layout state</param>
        /// <param name="action">Custom action when finished</param>
        /// <param name="extra">Extra height</param>
        public void SetVerticalStateUsingLayoutSize(State state, Action action = null, int extra = 0)
        {
            SetLayoutState(1, state);
            SetAction(1, action);
        }

        /// <summary>
        /// Set vertical state using layout size with known action when finished
        /// </summary>
        /// <param name="state">Layout state</param>
        /// <param name="action">Known action when finished</param>
        /// <param name="extra">Extra height</param>
        public void SetVerticalStateUsingLayoutSize(State state, FinishedAction action, int extra = 0)
        {
            SetLayoutState(1, state);
            SetAction(1, CreateAction(action));
        }
        #endregion User Functions

        #region Aid Functions
        /// <summary>
        /// Set state using physical size
        /// </summary>
        /// <param name="axis">0 - Horizontal, 1 - Vertical</param>
        /// <param name="state">0 - Min, 1 - Max</param>
        /// <param name="extra">Extra size</param>
        private void SetPhysicalState(int axis, int state, int extra = 0)
        {
            Canvas.ForceUpdateCanvases();
            switch (state)
            {
                case 0:
                    if(axis == 0)
                    {
                        targetWidth = 0 + extra;
                    }
                    else
                    {
                        targetHeight = 0 + extra;
                    }
                    break;
                case 1:
                    gameObject.SetActive(true);
                    if (axis == 0)
                    {
                        targetWidth = childTransform.sizeDelta.x + extra;
                    }
                    else
                    {
                        targetHeight = childTransform.sizeDelta.y + extra;
                    }
                    break;
                default:
                    Debug.Log(name + " Unrecognized state " + state);
                    break;
            }
        }

        /// <summary>
        /// Set state of axis
        /// </summary>
        /// <param name="axis">0 - Horizontal, 1 - Vertical</param>
        /// <param name="state"></param>
        private void SetLayoutState(int axis, State state, int extra = 0)
        {
            Canvas.ForceUpdateCanvases();
            switch (state)
            {
                case State.Zero:
                    if (axis == 0)
                    {
                        targetWidth = 0 + extra;
                    }
                    else
                    {
                        targetHeight = 0 + extra;
                    }
                    break;
                case State.Min:
                    gameObject.SetActive(true);
                    if (axis == 0)
                    {
                        targetWidth = GetLayoutSize(GetMinWidth) + extra;
                    }
                    else
                    {
                        targetHeight = GetLayoutSize(GetMinHeight) + extra;
                    }
                    break;
                case State.Max:
                    gameObject.SetActive(true);
                    if (axis == 0)
                    {
                        targetWidth = GetLayoutSize(GetPreferredWidth) + extra;
                    }
                    else
                    {
                        targetHeight = GetLayoutSize(GetPreferredHeight) + extra;
                    }
                    break;
                default:
                    Debug.Log(name + " Unrecognized state " + state);
                    break;
            }
        }

        private void SetAction(int axis, Action action)
        {
            if(axis == 0)
            {
                if (targetWidth == preferredWidth)
                {
                    if (action != null)
                        action();
                }
                else finishedAction = action;
            }
            else
            {
                if(targetHeight == preferredHeight)
                {
                    if (action != null)
                        action();
                }
                else finishedAction = action;
            }
        }

        private Action CreateAction(FinishedAction actionType)
        {
            Action action = null;
            switch (actionType)
            {
                case FinishedAction.Delete:
                    action = () => Destroy(gameObject);
                    break;
                case FinishedAction.Disable:
                    action = () => gameObject.SetActive(false);
                    break;
                default:
                    Debug.Log(name + " Unrecognized actionType " + actionType);
                    break;
            }
            return action;
        }

        private float GetLayoutSize(Func<RectTransform, float> getFunc)
        {
            float size = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform child = transform.GetChild(i) as RectTransform;
                if (child != null)
                {
                    size += getFunc(child);
                }
            }
            return size;
        }
        #endregion Aid Functions

        #region Layout Helper Functions
        /// <summary>
        ///   <para>Returns the minimum width of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        private static float GetMinWidth(RectTransform rect)
        {
            return GetLayoutProperty(rect, (ILayoutElement e) => e.minWidth, 0f);
        }

        /// <summary>
        ///   <para>Returns the preferred width of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        private static float GetPreferredWidth(RectTransform rect)
        {
            return Mathf.Max(GetLayoutProperty(rect, (ILayoutElement e) => e.minWidth, 0f), GetLayoutProperty(rect, (ILayoutElement e) => e.preferredWidth, 0f));
        }

        /// <summary>
        ///   <para>Returns the minimum height of the layout element.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the layout element to query.</param>
        private static float GetMinHeight(RectTransform rect)
        {
            return GetLayoutProperty(rect, (ILayoutElement e) => e.minHeight, 0f);
        }

        /// <summary>
		///   <para>Returns the preferred height of the layout element.</para>
		/// </summary>
		/// <param name="rect">The RectTransform of the layout element to query.</param>
		private static float GetPreferredHeight(RectTransform rect)
        {
            return Mathf.Max(GetLayoutProperty(rect, (ILayoutElement e) => e.minHeight, 0f), GetLayoutProperty(rect, (ILayoutElement e) => e.preferredHeight, 0f));
        }

        private static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue)
        {
            if (rect == null)
            {
                return 0f;
            }
            float num = defaultValue;
            int num2 = -2147483648;
            List<Component> list = new List<Component>();
            rect.GetComponents(typeof(ILayoutElement), list);
            for (int i = 0; i < list.Count; i++)
            {
                ILayoutElement layoutElement = list[i] as ILayoutElement;
                if (layoutElement != null)
                {
                    int layoutPriority = layoutElement.layoutPriority;
                    if (layoutPriority >= num2)
                    {
                        float num3 = property(layoutElement);
                        if (num3 >= 0f)
                        {
                            if (layoutPriority > num2)
                            {
                                num = num3;
                                num2 = layoutPriority;
                            }
                            else if (num3 > num)
                            {
                                num = num3;
                            }
                        }
                    }
                }
            }
            return num;
        }
        #endregion Layout Helper Functions
    }
}
