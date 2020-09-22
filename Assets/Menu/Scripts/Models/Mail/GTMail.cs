using System;
using System.Collections.Generic;
using UnityEngine;

public class GTMail : FragmentedListDynamicElement
{
    public bool Read;
    public string Sender { get; private set; }
    public string RawMessage { get; private set; }
    public DateTime SentTime { get; private set; }
    public TextData Title { get; private set; }
    public TextData HeadLine { get; private set; }
    public ImageData Icon { get; private set; }
    public List<TextData> Messages { get; private set; }
    public List<ButtonData> buttons { get; private set; }

    private int m_id;
    public override int Id { get { return m_id; } }
    public override bool IsFiller { get { return false; } }
    public override bool NeedToUpdate { get { return Read == false; } }

    public GTMail(Dictionary<string, object> data)
    {
        if (data == null)
            return;

        object o;
        if (data.TryGetValue("Id", out o))
            m_id = o.ParseInt();

        if (data.TryGetValue("Read", out o))
            Read = o.ParseBool();

        DateTime time;
        if (data.TryGetValue("StartDate", out o) && DateTime.TryParse(o.ToString(), out time))
            SentTime = time;

        if (data.TryGetValue("Sender", out o))
            Sender = o.ToString();

        if (data.TryGetValue("Title", out o))
            Title = new TextData(o as Dictionary<string, object>, 47);

        if (data.TryGetValue("Headline", out o))
            HeadLine = new TextData(o as Dictionary<string, object>, 47);

        if (data.TryGetValue("Picture", out o))
            Icon = new ImageData(o as Dictionary<string, object>);

        List<TextData> textElements = new List<TextData>();
        if (data.TryGetValue("TextElements", out o))
        {
            List<object> textsList = o as List<object>;
            for (int x = 0; x < textsList.Count; ++x)
                textElements.Add(new TextData(textsList[x] as Dictionary<string, object>));
        }

        List<ButtonData> buttonElements = new List<ButtonData>();
        if (data.TryGetValue("ButtonElements", out o))
        {
            List<object> textsList = o as List<object>;
            for (int x = 0; x < textsList.Count; ++x)
                buttonElements.Add(new ButtonData(textsList[x] as Dictionary<string, object>));
        }
    }

    protected override void Populate(RectTransform activeObject)
    {
        activeObject.GetComponent<GTMailPreview>().Populate(this);
    }
}
