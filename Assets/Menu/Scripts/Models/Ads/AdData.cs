using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class AdData
{
    const int DEFAULT_SHOW_TIME = 10;

    enum AdDataItemType
    {
        Id = 0,
        ImageUrl,
        Text,
        ActionType,
        Action,
        ShowTime,
    }

    public string AdId { get; private set; }
    public string Text { get; private set; }
    public float ShowTime { get; private set; }
    public bool OpenWebPage { get; private set; }
    public SpriteData ImageData { get; private set; }
    public UnityAction Callback { get; private set; }

    public AdData(Dictionary<string,object> dataDict)
    {
        object o;
        if(dataDict.TryGetValue(AdDataItemType.Id.ToString(), out o))
            AdId = o.ToString();
        
        if (dataDict.TryGetValue(AdDataItemType.Text.ToString(), out o))
            Text = o.ToString();

        if (dataDict.TryGetValue(AdDataItemType.ShowTime.ToString(), out o))
            ShowTime = o.ParseInt();
        else
            ShowTime = 10;

        if (dataDict.TryGetValue(AdDataItemType.ImageUrl.ToString(), out o))
        {
            ImageData = new SpriteData(o.ToString().Replace("http", "https"));
        }
        
        string actionType = string.Empty;
        if (dataDict.TryGetValue(AdDataItemType.ActionType.ToString(), out o))
            actionType = o.ToString();
        
        string actionString = string.Empty;
        if (dataDict.TryGetValue(AdDataItemType.Action.ToString(), out o))
            actionString = o.ToString();

        bool openWebPage;
        Callback = ActionKit.CreateAction(actionType, actionString, out openWebPage);
        OpenWebPage = openWebPage;
    }

    public override string ToString()
    {
        return AdId;
    }
}
