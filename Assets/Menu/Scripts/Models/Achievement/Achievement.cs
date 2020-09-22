using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement : DynamicElement {


    public string description;
    public int step;
    public int progess;
    public bool collected;
    public int ID;

    // Use this for initialization
    public Achievement(string description, int step, int progess, bool collected, int ID)
    {
        this.description = description;
        this.step = step;
        this.progess = progess;
        this.collected = collected;
        this.ID = ID;
    }

    protected override void Populate(RectTransform activeObject)
    {
        activeObject.GetComponent<AchievementView>().Populate(this);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
