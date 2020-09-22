using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class OuterGradient : Gradient
    {
        private RectTransform m_rectTransform;
        [Range(0, .5f), SerializeField]
        private float m_scaleX = .5f;
        [Range(0, .5f), SerializeField]
        private float m_scaleY = .5f;

        public RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = transform as RectTransform;
                return m_rectTransform;
            }
        }

        public float scaleX
        {
            get { return m_scaleX; }
            set { m_scaleX = value; }
        }

        public float scaleY
        {
            get { return m_scaleY; }
            set { m_scaleY = value; }
        }


        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
            {
                return;
            }
            List<UIVertex> list = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(list);
            Vector2 size = rectTransform.rect.size;
            Vector2 pos = new Vector2(size.x / 2 * (1 - scaleX), size.y / 2 * (1 - scaleY));
            List<UIVertex> outList1 = ListPool<UIVertex>.Get();
            this.ApplyGradient(list, outList1, Type.Vertical, changeType, offset, startColor, endColor, new Vector2(0, pos.y), new Vector2(1, scaleY));
            List<UIVertex> outList2 = ListPool<UIVertex>.Get();
            this.ApplyGradient(list, outList2, Type.Vertical, changeType, -offset, endColor, startColor, new Vector2(0, -pos.y), new Vector2(1, scaleY));
            List<UIVertex> outList3 = ListPool<UIVertex>.Get();
            this.ApplyGradient(list, outList3, Type.Horizontal, changeType, offset, startColor, endColor, new Vector2(pos.x, 0), new Vector2(scaleX, 1));
            List<UIVertex> outList4 = ListPool<UIVertex>.Get();
            this.ApplyGradient(list, outList4, Type.Horizontal, changeType, -offset, endColor, startColor, new Vector2(-pos.x, 0), new Vector2(scaleX, 1));
            list.AddRange(outList1);
            list.AddRange(outList2);
            list.AddRange(outList3);
            list.AddRange(outList4);
            vh.Clear();
            vh.AddUIVertexTriangleStream(list);
            ListPool<UIVertex>.Release(list);
            ListPool<UIVertex>.Release(outList1);
            ListPool<UIVertex>.Release(outList2);
            ListPool<UIVertex>.Release(outList3);
            ListPool<UIVertex>.Release(outList4);
        }
    }
}
