using UnityEngine;

public abstract class FragmentedListDynamicElement : DynamicElement, IFragmentedListElement
{
    public abstract int Id { get; }
    public abstract bool NeedToUpdate { get; }
    public abstract bool IsFiller { get; }
}

public class DynamicFillerElement : FragmentedListDynamicElement, IFragmentedListElement
{
    public override int Id { get { return -1; } }
    public override bool NeedToUpdate { get { return true; } }
    public override bool IsFiller { get { return true; } }

    protected override void Populate(RectTransform activeObject) { }
}
