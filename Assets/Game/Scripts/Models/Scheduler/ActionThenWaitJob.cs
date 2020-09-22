using UnityEngine;
using System.Collections;
using System;

public class ActionThenWaitJob : ActionJob
{
    private float m_waitTime;
    public float waitTime
    {
        get { return m_waitTime; }
    }

    public ActionThenWaitJob(Action action, float waitTime) : base(action)
    {
        m_waitTime = waitTime;
    }

    public ActionThenWaitJob(string desc, Action action, float waitTime) : base(desc, action)
    {
        m_waitTime = waitTime;
    }


}
