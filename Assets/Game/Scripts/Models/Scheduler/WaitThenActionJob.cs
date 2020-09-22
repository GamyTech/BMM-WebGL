using UnityEngine;
using System.Collections;
using System;

public class WaitThenActionJob : ActionJob
{
    private float m_waitTime;
    public float waitTime
    {
        get { return m_waitTime; }
    }

    public WaitThenActionJob(Action action, float waitTime) : base(action)
    {
        m_waitTime = waitTime;
    }

    public WaitThenActionJob(string desc, Action action, float waitTime) : base(desc, action)
    {
        m_waitTime = waitTime;
    }


}
