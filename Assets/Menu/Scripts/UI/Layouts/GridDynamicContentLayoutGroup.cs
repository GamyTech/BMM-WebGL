using UnityEngine;
using System.Collections;
using System;

namespace UnityEngine.UI
{
    public class GridDynamicContentLayoutGroup : DynamicContentLayoutGroup
    {
        [SerializeField]
        protected GridLayoutGroup.Corner m_StartCorner;

        [SerializeField]
        protected GridLayoutGroup.Axis m_StartAxis;

        [SerializeField]
        protected Vector2 m_CellSize = new Vector2(100f, 100f);

        [SerializeField]
        protected Vector2 m_Spacing = new Vector2(0, 0);

        [SerializeField]
        protected GridLayoutGroup.Constraint m_Constraint;

        [SerializeField]
        protected int m_ConstraintCount = 2;

        /// <summary>
        ///   <para>Which corner should the first cell be placed in?</para>
        /// </summary>
        public GridLayoutGroup.Corner startCorner
        {
            get
            {
                return this.m_StartCorner;
            }
            set
            {
                base.SetProperty<GridLayoutGroup.Corner>(ref this.m_StartCorner, value);
            }
        }

        /// <summary>
        ///   <para>Which axis should cells be placed along first?</para>
        /// </summary>
        public GridLayoutGroup.Axis startAxis
        {
            get
            {
                return this.m_StartAxis;
            }
            set
            {
                base.SetProperty<GridLayoutGroup.Axis>(ref this.m_StartAxis, value);
            }
        }

        /// <summary>
        ///   <para>The size to use for each cell in the grid.</para>
        /// </summary>
        public Vector2 cellSize
        {
            get
            {
                return this.m_CellSize;
            }
            set
            {
                base.SetProperty<Vector2>(ref this.m_CellSize, value);
            }
        }

        /// <summary>
        ///   <para>The spacing to use between layout elements in the grid.</para>
        /// </summary>
        public Vector2 spacing
        {
            get
            {
                return this.m_Spacing;
            }
            set
            {
                base.SetProperty<Vector2>(ref this.m_Spacing, value);
            }
        }

        /// <summary>
        ///   <para>Which constraint to use for the GridLayoutGroup.</para>
        /// </summary>
        public GridLayoutGroup.Constraint constraint
        {
            get
            {
                return this.m_Constraint;
            }
            set
            {
                base.SetProperty<GridLayoutGroup.Constraint>(ref this.m_Constraint, value);
            }
        }

        /// <summary>
        ///   <para>How many cells there should be along the constrained axis.</para>
        /// </summary>
        public int constraintCount
        {
            get
            {
                return this.m_ConstraintCount;
            }
            set
            {
                base.SetProperty<int>(ref this.m_ConstraintCount, Mathf.Max(1, value));
            }
        }

        /// <summary>
		///   <para>Called by the layout system.</para>
		/// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            int constraint,num;
            if (this.m_Constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                constraint = (num = this.m_ConstraintCount);
            }
            else if (this.m_Constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                constraint = (num = Mathf.CeilToInt((float)base.elementsList.Count / (float)this.m_ConstraintCount - 0.001f));
            }
            else
            {
                num = 1;
                constraint = Mathf.CeilToInt(Mathf.Sqrt((float)base.elementsList.Count));
            }
            float min = (float)base.padding.horizontal + (this.cellSize.x + this.spacing.x) * (float)num - this.spacing.x;
            float pref = (float)base.padding.horizontal + (this.cellSize.x + this.spacing.x) * (float)constraint - this.spacing.x;
            base.SetLayoutInputForAxis(min, pref, -1f, 0);
        }

        /// <summary>
		///   <para>Called by the layout system.</para>
		/// </summary>
        public override void CalculateLayoutInputVertical()
        {
            int num;
            if (this.m_Constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                num = Mathf.CeilToInt((float)base.elementsList.Count / (float)this.m_ConstraintCount - 0.001f);
            }
            else if (this.m_Constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                num = this.m_ConstraintCount;
            }
            else
            {
                float x = base.rectTransform.rect.size.x;
                int num2 = Mathf.Max(1, Mathf.FloorToInt((x - (float)base.padding.horizontal + this.spacing.x + 0.001f) / (this.cellSize.x + this.spacing.x)));
                num = Mathf.CeilToInt((float)base.elementsList.Count / (float)num2);
            }
            float num3 = (float)base.padding.vertical + (this.cellSize.y + this.spacing.y) * (float)num - this.spacing.y;
            base.SetLayoutInputForAxis(num3, num3, -1f, 1);
        }

        /// <summary>
		///   <para>Called by the layout system.</para>
		/// </summary>
        public override void SetLayoutHorizontal()
        {
            this.SetCellsAlongAxis(0);
        }

        /// <summary>
		///   <para>Called by the layout system.</para>
		/// </summary>
        public override void SetLayoutVertical()
        {
            this.SetCellsAlongAxis(1);
        }

        /// <summary>
        ///   <para>Set the positions and sizes of the child elements for the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
        private void SetCellsAlongAxis(int axis)
        {
            if (axis == 0)
                return;

            if(startAxis == GridLayoutGroup.Axis.Horizontal)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LayoutUtility.GetPreferredSize(rectTransform, 1));
            else if(startAxis == GridLayoutGroup.Axis.Vertical)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LayoutUtility.GetPreferredSize(rectTransform, 0));


            Vector2 contentFromViewOffset = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y);

            Vector3[] corners = new Vector3[4];
            viewPort.GetLocalCorners(corners);
            Vector2 viewBoundMin = new Vector2(corners[0].x, corners[0].y);
            Vector2 viewBoundMax = new Vector2(corners[3].x, corners[1].y);
            viewBoundMin += contentFromViewOffset;
            viewBoundMax += contentFromViewOffset;

            float contentSizeX = base.rectTransform.rect.size.x;
            float contentSizeY = base.rectTransform.rect.size.y;
            int cols;
            int rows;
            if (this.m_Constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                cols = this.m_ConstraintCount;
                rows = Mathf.CeilToInt((float)base.elementsList.Count / (float)cols - 0.001f);
            }
            else if (this.m_Constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                rows = this.m_ConstraintCount;
                cols = Mathf.CeilToInt((float)base.elementsList.Count / (float)rows - 0.001f);
            }
            else
            {
                if (this.cellSize.x + this.spacing.x <= 0f)
                {
                    cols = 2147483647;
                }
                else
                {
                    cols = Mathf.Max(1, Mathf.FloorToInt((contentSizeX - (float)base.padding.horizontal + this.spacing.x + 0.001f) / (this.cellSize.x + this.spacing.x)));
                }
                if (this.cellSize.y + this.spacing.y <= 0f)
                {
                    rows = 2147483647;
                }
                else
                {
                    rows = Mathf.Max(1, Mathf.FloorToInt((contentSizeY - (float)base.padding.vertical + this.spacing.y + 0.001f) / (this.cellSize.y + this.spacing.y)));
                }
            }


            int num3 = (int)((int)this.startCorner % (int)GridLayoutGroup.Corner.LowerLeft);
            int num4 = (int)((int)this.startCorner / (int)GridLayoutGroup.Corner.LowerLeft);
            int num5;
            int num6;
            int num7;
            if (this.startAxis == GridLayoutGroup.Axis.Horizontal)
            {
                num5 = cols;
                num6 = Mathf.Clamp(cols, 1, base.elementsList.Count);
                num7 = Mathf.Clamp(rows, 1, Mathf.CeilToInt((float)base.elementsList.Count / (float)num5));
            }
            else
            {
                num5 = rows;
                num7 = Mathf.Clamp(rows, 1, base.elementsList.Count);
                num6 = Mathf.Clamp(cols, 1, Mathf.CeilToInt((float)base.elementsList.Count / (float)num5));
            }
            Vector2 vector = new Vector2((float)num6 * this.cellSize.x + (float)(num6 - 1) * this.spacing.x, (float)num7 * this.cellSize.y + (float)(num7 - 1) * this.spacing.y);
            Vector2 vector2 = new Vector2(base.GetStartOffset(0, vector.x), base.GetStartOffset(1, vector.y));
            for (int j = 0; j < base.elementsList.Count; j++)
            {
                int num8;
                int num9;
                if (this.startAxis == GridLayoutGroup.Axis.Horizontal)
                {
                    num8 = j % num5;
                    num9 = j / num5;
                }
                else
                {
                    num8 = j / num5;
                    num9 = j % num5;
                }
                if (num3 == 1)
                {
                    num8 = num6 - 1 - num8;
                }
                if (num4 == 1)
                {
                    num9 = num7 - 1 - num9;
                }

                Vector2 boundMin = new Vector2(vector2.x + (this.cellSize[0] + this.spacing[0]) * (float)num8,
                    vector2.y + (this.cellSize[1] + this.spacing[1]) * (float)num9);
                Vector2 boundMax = boundMin + cellSize;
                bool inView = boundMin.x <= viewBoundMax.x && boundMax.x >= viewBoundMin.x && boundMin.y <= viewBoundMax.y && boundMax.y >= viewBoundMin.y;
                HandleElement(base.elementsList[j], inView);

                base.SetChildAlongAxis(base.elementsList[j].activeObject, 0, vector2.x + (this.cellSize[0] + this.spacing[0]) * (float)num8, this.cellSize[0]);
                base.SetChildAlongAxis(base.elementsList[j].activeObject, 1, vector2.y + (this.cellSize[1] + this.spacing[1]) * (float)num9, this.cellSize[1]);
            }
        }

        /// <summary>
        /// Add element to elements list, 
        /// And initialize its size acording grid cell size
        /// </summary>
        /// <param name="element"></param>
        protected override void AddElementAndInit(IDynamicElement element)
        {
            base.AddElementAndInit(element);
            element.minSize = new Vector2(m_CellSize.x, m_CellSize.y);
            element.preferredSize = new Vector2(m_CellSize.x, m_CellSize.y);
        }
    }
}
