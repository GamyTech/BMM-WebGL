using System.Collections;
using System;

public class EnumeratorJob : IJob
{
    private string m_description;
    public string Description
    {
        get { return m_description; }
    }

    private Action m_finishedCallback;
    public Action FinishedCallback
    {
        get { return m_finishedCallback; }
    }

    private IEnumerator m_jobRoutine;
    public IEnumerator JobRoutine
    {
        get { return m_jobRoutine; }
    }

    public EnumeratorJob(IEnumerator routine, Action callback = null)
    {
       // m_description = GetType().ToString();
        m_jobRoutine = routine;
        m_finishedCallback = callback;
    }

    public EnumeratorJob(string desc, IEnumerator routine, Action callback = null)
    {
        m_description = desc;
        m_jobRoutine = routine;
        m_finishedCallback = callback;
    }

    public override string ToString()
    {
        return Description;
    }
}
