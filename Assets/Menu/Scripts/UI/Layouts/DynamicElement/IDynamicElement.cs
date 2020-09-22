using UnityEngine;

public interface IDynamicElement
{
    RectTransform activeObject
    {
        get;
    }

    Vector2 preferredSize
    {
        get;
        set;
    }

    Vector2 minSize
    {
        get;
        set;
    }

    void ActivateObject(RectTransform viewObject);
    void DeactivateObject();
}
