using System;

public class ActionJob : IJob
{
    private string m_description;
    public string Description
    {
        get { return m_description; }
    }

    private Action m_jobAction;
    public Action JobAction
    {
        get { return m_jobAction; }
    }

    public ActionJob(Action action)
    {
        m_description = GetType().ToString();
        m_jobAction = action;
    }

    public ActionJob(string desc, Action action)
    {
        m_description = desc;
        m_jobAction = action;
    }

    public override string ToString()
    {
        return Description;
    }
}
