using UnityEngine;

public abstract class DynamicElement : IDynamicElement
{

    private RectTransform m_activeObject;
    public RectTransform activeObject { get { return m_activeObject; } }

    private Vector2 m_minSize;
    public Vector2 minSize { get { return m_minSize; } set { m_minSize = value; } }

    private Vector2 m_preferredSize;
    public Vector2 preferredSize { get { return m_preferredSize; } set { m_preferredSize = value; } }

    public virtual void ActivateObject(RectTransform viewObject)
    {
        m_activeObject = viewObject;
        if (activeObject != null)
            Populate(activeObject);
    }

    public virtual void DeactivateObject()
    {
        m_activeObject = null;
    }

    protected abstract void Populate(RectTransform activeObject);
}
