using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class Gradient : BaseMeshEffect
    {
        public enum Type
        {
            Vertical,
            Horizontal,
        }

        [SerializeField]
        private Type m_gradientType = Type.Horizontal;
        [SerializeField]
        private Change.ChangeType m_changeType = Change.ChangeType.Linear;
        [SerializeField]
        private Color m_startColor = Color.white;
        [SerializeField]
        private Color m_endColor = Color.black;
        [Range(-1, 1), SerializeField]
        private float m_offset;
        [SerializeField]
        private bool useMainImageColor = false;

        public Type gradientType
        {
            get { return m_gradientType; }
            set
            {
                m_gradientType = value;
                SetDirty();
            }
        }

        public Change.ChangeType changeType
        {
            get { return m_changeType; }
            set
            {
                m_changeType = value;
                SetDirty();
            }
        }

        public Color startColor
        {
            get { return m_startColor; }
            set
            {
                m_startColor = value;
                SetDirty();
            }
        }

        public Color endColor
        {
            get { return m_endColor; }
            set
            {
                m_endColor = value;
                SetDirty();
            }
        }

        public float offset
        {
            get { return m_offset; }
            set
            {
                m_offset = value;
                SetDirty();
            }
        }


        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
            {
                return;
            }
            List<UIVertex> list = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(list);
            List<UIVertex> outList = ListPool<UIVertex>.Get();
            this.ApplyGradient(list, outList, gradientType, changeType, offset, endColor, startColor, new Vector2(0, 0), new Vector2(1, 1));
            vh.Clear();
            vh.AddUIVertexTriangleStream(outList);
            ListPool<UIVertex>.Release(list);
            ListPool<UIVertex>.Release(outList);
        }

        protected void ApplyGradient(List<UIVertex> inVerts, List<UIVertex> outVerts, Type gradientType, 
            Change.ChangeType changeType, float offset, Color startColor, Color endColor, Vector2 offsetPosition, Vector2 scale)
        {
            int nCount = inVerts.Count;
            if (nCount <= 0)
                return;
            switch (gradientType)
            {
                case Type.Vertical:
                    {
                        float fBottomY = inVerts[0].position.y;
                        float fTopY = inVerts[0].position.y;
                        float fYPos = 0f;

                        for (int i = 0; i < nCount; i++)
                        {
                            fYPos = inVerts[i].position.y;
                            if (fYPos > fTopY)
                                fTopY = fYPos;
                            else if (fYPos < fBottomY)
                                fBottomY = fYPos;
                        }

                        float fUIElementHeight = 1f / (fTopY - fBottomY);

                        for (int i = 0; i < nCount; i++)
                        {
                            UIVertex v = inVerts[i];
                            Color sColor = useMainImageColor ? v.color * startColor : startColor;
                            Color eColor = useMainImageColor ? v.color * endColor : endColor;
                            v.color = Change.ChangeWithType(changeType, sColor, eColor, (v.position.y - fBottomY) * fUIElementHeight - offset);
                            v.position =  Vector2.Scale(v.position, scale) + offsetPosition;
                            outVerts.Add(v);
                        }
                    }
                    break;
                case Type.Horizontal:
                    {
                        float fLeftX = inVerts[0].position.x;
                        float fRightX = inVerts[0].position.x;
                        float fXPos = 0f;

                        for (int i = 0; i < nCount; i++)
                        {
                            fXPos = inVerts[i].position.x;
                            if (fXPos > fRightX)
                                fRightX = fXPos;
                            else if (fXPos < fLeftX)
                                fLeftX = fXPos;
                        }

                        float fUIElementWidth = 1f / (fRightX - fLeftX);

                        for (int i = 0; i < nCount; i++)
                        {
                            UIVertex v = inVerts[i];
                            v.color = Change.ChangeWithType(changeType, startColor, endColor, (v.position.x - fLeftX) * fUIElementWidth - offset);
                            v.position = Vector2.Scale(v.position, scale) + offsetPosition;
                            outVerts.Add(v);
                        }
                    }
                    break;
            }
        }

        protected virtual void SetDirty()
        {
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }
    }
}
