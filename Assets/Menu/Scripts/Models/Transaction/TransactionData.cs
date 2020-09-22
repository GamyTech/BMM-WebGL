using System;
using System.Collections.Generic;
using UnityEngine;

public class TransactionData : DynamicElement
{

    public int Id { get; private set; }
    public string Reason { get; private set; }
    public DateTime Date { get; private set; }
    public string MatchId { get; private set; }
    public float Amount { get; private set; }
    public float Currency { get; private set; }

    public TransactionData(Dictionary<string, object> data)
    {
        if (data == null)
            return;

        object o;
        if (data.TryGetValue("Id", out o))
            Id = o.ParseInt();

        if (data.TryGetValue("Reason", out o))
            Reason = o.ToString();

        DateTime time;
        if (data.TryGetValue("RecordedDate", out o) && DateTime.TryParse(o.ToString(), out time))
            Date = time;

        if (data.TryGetValue("Cash", out o))
            Currency = o.ParseFloat();

        if (data.TryGetValue("MatchId", out o) && o.ToString() != "-1")
            MatchId = o.ToString();

        if (data.TryGetValue("ChangedAmount", out o))
            Amount = o.ParseFloat();
    }

    protected override void Populate(RectTransform activeObject)
    {
        activeObject.GetComponent<TransactionView>().Populate(this);
    }
}
