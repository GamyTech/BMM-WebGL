using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform), typeof(ObjectPool))]
    public abstract class DynamicContentLayoutGroup : UIBehaviour, ILayoutElement, ILayoutController, ILayoutGroup
    {
        [SerializeField]
        protected RectOffset m_Padding = new RectOffset();

        [FormerlySerializedAs("m_Alignment"), SerializeField]
        protected TextAnchor m_ChildAlignment;

        private bool m_markedForRebuild = false;

        private Vector2 m_TotalMinSize = new Vector2(0, 0);
        private Vector2 m_TotalPreferredSize = new Vector2(0, 0);
        private Vector2 m_TotalFlexibleSize = new Vector2(0, 0);
        private List<IDynamicElement> m_elementsToDeleteList = new List<IDynamicElement>();

        [NonSerialized]
        private List<IDynamicElement> m_elementsList = new List<IDynamicElement>();
        [NonSerialized]
        private RectTransform m_Rect;
        [NonSerialized]
        private ObjectPool m_objectPool;
        [NonSerialized]
        private RectTransform m_viewPort;
        [NonSerialized]
        private ScrollRect m_parentScrollRect;

        [SerializeField]
        private bool m_poolPriority = true;

        /// <summary>
        ///   <para>The padding to add around the child layout elements.</para>
        /// </summary>
        public RectOffset padding
        {
            get
            {
                return this.m_Padding;
            }
            set
            {
                this.SetProperty<RectOffset>(ref this.m_Padding, value);
            }
        }

        /// <summary>
		///   <para>The alignment to use for the child layout elements in the layout group.</para>
		/// </summary>
		public TextAnchor childAlignment
        {
            get
            {
                return this.m_ChildAlignment;
            }
            set
            {
                this.SetProperty<TextAnchor>(ref this.m_ChildAlignment, value);
            }
        }

        protected RectTransform rectTransform
        {
            get
            {
                if (this.m_Rect == null)
                {
                    this.m_Rect = GetComponent<RectTransform>();
                }
                return this.m_Rect;
            }
        }

        public List<IDynamicElement> elementsList { get { return m_elementsList; } }

        public ObjectPool objectPool
        {
            get
            {
                if (this.m_objectPool == null)
                    this.m_objectPool = GetComponent<ObjectPool>();
                return this.m_objectPool;
            }
        }

        protected RectTransform viewPort
        {
            get
            {
                if (m_viewPort == null)
                    m_viewPort = parentScrollRect.viewport;
                return m_viewPort;
            }
        }

        protected ScrollRect parentScrollRect
        {
            get
            {
                if (m_parentScrollRect == null)
                    m_parentScrollRect = GetComponentInParent<ScrollRect>();
                return m_parentScrollRect;
            }
        }

        public float minWidth { get { return this.GetTotalMinSize(0); } }

        public float preferredWidth { get { return this.GetTotalPreferredSize(0); } }

        public float flexibleWidth { get { return this.GetTotalFlexibleSize(0); } }

        public float minHeight { get { return this.GetTotalMinSize(1); } }

        public float preferredHeight { get { return this.GetTotalPreferredSize(1); } }

        public float flexibleHeight { get { return this.GetTotalFlexibleSize(1); } }

        public int layoutPriority { get { return 0; } }

        public bool poolPriority { get { return m_poolPriority; } }

        private bool isRootLayoutGroup { get { return transform.parent == null || transform.parent.GetComponent(typeof(ILayoutGroup)) == null; } }

        #region Elements Hendling
        /// <summary>
        /// Overridable add new element
        /// Can be used with derivatives to initialize size of element
        /// </summary>
        /// <param name="element"></param>
        protected virtual void AddElementAndInit(IDynamicElement element)
        {
            m_elementsList.Add(element);
        }

        /// <summary>
        /// Set Elements to list
        /// Clears existing elements and sets new ones
        /// </summary>
        /// <param name="elements"></param>
        public void SetElements(List<IDynamicElement> elements)
        {
            ClearElements();
            AddElements(elements);
        }

        /// <summary>
        /// Add single element to list
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(IDynamicElement element)
        {
            AddElementAndInit(element);
            this.SetDirty();
        }

        /// <summary>
        /// Add multiple elements to list
        /// </summary>
        /// <param name="elements"></param>
        public void AddElements(List<IDynamicElement> elements)
        {
            for (int i = 0; i < elements.Count; i++)
                AddElementAndInit(elements[i]);
            this.SetDirty();
        }

        /// <summary>
        /// Remove single element from list
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElement(IDynamicElement element)
        {
            if (element.activeObject != null)
                DeactivateElement(element);
            m_elementsList.Remove(element);
            this.SetDirty();
        }

        /// <summary>
        /// Clear current elements in list
        /// Deactivating them first
        /// </summary>
        public void ClearElements()
        {
            for (int i = 0; i < m_elementsList.Count; i++)
                DeactivateElement(m_elementsList[i]);
            m_elementsList.Clear();
            this.SetDirty();
        }

        /// <summary>
        /// Activates element if its not active already
        /// </summary>
        /// <param name="element"></param>
        protected void ActivateElement(IDynamicElement element)
        {
            if (element.activeObject != null)
                return;

            GameObject go = null;
            if (poolPriority)
            {
                if (objectPool.objectPrefab != null)
                    go = objectPool.GetObjectFromPool();
                else if (element is ICustomPrefab && ((ICustomPrefab)element).Prefab != null)
                    go = ObjectPoolManager.GetObject(((ICustomPrefab)element).Prefab);
            }
            else
            {
                if (element is ICustomPrefab && ((ICustomPrefab)element).Prefab != null)
                    go = ObjectPoolManager.GetObject(((ICustomPrefab)element).Prefab);
                else if (objectPool.objectPrefab != null)
                    go = objectPool.GetObjectFromPool();
            }

            if(go == null)
                throw new NullReferenceException("Dynamic layout group " + name + " >>> Prefab is null");
            
            go.InitGameObjectAfterInstantiation(transform);
            element.ActivateObject(go.transform as RectTransform);
        }

        /// <summary>
        /// Deactivates element if its currently active
        /// </summary>
        /// <param name="element"></param>
        protected void DeactivateElement(IDynamicElement element)
        {
            if (element.activeObject == null)
                return;

            if (poolPriority)
            {
                if (objectPool.objectPrefab != null)
                    objectPool.PoolObject(element.activeObject.gameObject);
                else if (element is ICustomPrefab && ((ICustomPrefab)element).Prefab != null)
                    ObjectPoolManager.PoolObject(element.activeObject.gameObject);
            }
            else
            {
                if (element is ICustomPrefab && ((ICustomPrefab)element).Prefab != null)
                    ObjectPoolManager.PoolObject(element.activeObject.gameObject);
                else if (objectPool.objectPrefab != null)
                    objectPool.PoolObject(element.activeObject.gameObject);
            }
            element.DeactivateObject();
        }

        /// <summary>
        /// Handle element
        /// if element is in view and his view is null get viewObject from pool and activate it
        /// if element is out of view and his view is not null deactivate view and return to pool
        /// </summary>
        /// <param name="element"></param>
        /// <param name="inView"></param>
        protected void HandleElement(IDynamicElement element, bool inView)
        {
            if (inView)
                ActivateElement(element);
            else if(!m_elementsToDeleteList.Contains(element))
                m_elementsToDeleteList.Add(element);
        }
        #endregion Elements Hendling

        public abstract void CalculateLayoutInputHorizontal();

        public abstract void CalculateLayoutInputVertical();

        public abstract void SetLayoutHorizontal();

        public abstract void SetLayoutVertical();

        protected virtual void onValueChanged(Vector2 pos)
        {
            SetDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            parentScrollRect.onValueChanged.AddListener(onValueChanged);
            this.SetDirty();
        }

        protected override void OnDisable()
        {
            parentScrollRect.onValueChanged.RemoveListener(onValueChanged);
            SetDirty();
            base.OnDisable();
        }

        /// <summary>
        /// Rebuild Layout if its marked for rebuild
        /// </summary>
        protected virtual void Update()
        {
            if (m_markedForRebuild)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
                m_markedForRebuild = false;
            }
            
            
            while(m_elementsToDeleteList.Count > 0)
            {
                DeactivateElement(m_elementsToDeleteList[0]);
                m_elementsToDeleteList.RemoveAt(0);
            }
        }

        /// <summary>
        ///   <para>Callback for when properties have been changed by animation.</para>
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
            this.SetDirty();
        }

        /// <summary>
		///   <para>The min size for the layout group on the given axis.</para>
		/// </summary>
		/// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
		/// <returns>
		///   <para>The min size.</para>
		/// </returns>
		protected float GetTotalMinSize(int axis)
        {
            return this.m_TotalMinSize[axis];
        }

        /// <summary>
        ///   <para>The preferred size for the layout group on the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
        /// <returns>
        ///   <para>The preferred size.</para>
        /// </returns>
        protected float GetTotalPreferredSize(int axis)
        {
            return this.m_TotalPreferredSize[axis];
        }

        /// <summary>
        ///   <para>The flexible size for the layout group on the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
        /// <returns>
        ///   <para>The flexible size.</para>
        /// </returns>
        protected float GetTotalFlexibleSize(int axis)
        {
            return this.m_TotalFlexibleSize[axis];
        }

        /// <summary>
		///   <para>Used to set the calculated layout properties for the given axis.</para>
		/// </summary>
		/// <param name="totalMin">The min size for the layout group.</param>
		/// <param name="totalPreferred">The preferred size for the layout group.</param>
		/// <param name="totalFlexible">The flexible size for the layout group.</param>
		/// <param name="axis">The axis to set sizes for. 0 is horizontal and 1 is vertical.</param>
		protected void SetLayoutInputForAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
        {
            //Debug.Log("SetLayoutInputForAxis: " + totalMin + " " + totalPreferred + " " + totalFlexible + " " + axis);
            this.m_TotalMinSize[axis] = totalMin;
            this.m_TotalPreferredSize[axis] = totalPreferred;
            this.m_TotalFlexibleSize[axis] = totalFlexible;
        }

        /// <summary>
		///   <para>Set the position and size of a child layout element along the given axis.</para>
		/// </summary>
		/// <param name="rect">The RectTransform of the child layout element.</param>
		/// <param name="axis">The axis to set the position and size along. 0 is horizontal and 1 is vertical.</param>
		/// <param name="pos">The position from the left side or top.</param>
		/// <param name="size">The size.</param>
		protected void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
        {
            if (rect == null)
            {
                return;
            }
            rect.SetInsetAndSizeFromParentEdge((axis != 0) ? RectTransform.Edge.Top : RectTransform.Edge.Left, pos, size);
        }

        /// <summary>
		///   <para>Returns the calculated position of the first child layout element along the given axis.</para>
		/// </summary>
		/// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
		/// <param name="requiredSpaceWithoutPadding">The total space required on the given axis for all the layout elements including spacing and excluding padding.</param>
		/// <returns>
		///   <para>The position of the first child along the given axis.</para>
		/// </returns>
		protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float spaceNeeded = requiredSpaceWithoutPadding + (float)((axis != 0) ? this.padding.vertical : this.padding.horizontal);
            float totalSpace = this.rectTransform.rect.size[axis];
            float remaininigSpace = totalSpace - spaceNeeded;
            float num4;
            if (axis == 0)
            {
                num4 = (float)((int)this.childAlignment % (int)TextAnchor.MiddleLeft) * 0.5f;
            }
            else
            {
                num4 = (float)((int)this.childAlignment / (int)TextAnchor.MiddleLeft) * 0.5f;
            }
            return (float)((axis != 0) ? this.padding.top : this.padding.left) + remaininigSpace * num4;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (this.isRootLayoutGroup)
            {
                this.SetDirty();
            }
        }

        protected virtual void OnTransformChildrenChanged()
        {
            this.SetDirty();
        }

        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
            {
                return;
            }
            currentValue = newValue;
            this.SetDirty();
        }

        /// <summary>
		///   <para>Mark the LayoutGroup as dirty.</para>
		/// </summary>
		protected void SetDirty()
        {
            if (!this.IsActive())
            {
                return;
            }
            m_markedForRebuild = true;
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            this.SetDirty();
        }
#endif
    }
}
