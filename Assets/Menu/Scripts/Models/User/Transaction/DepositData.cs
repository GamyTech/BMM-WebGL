using System.Collections.Generic;
using System;

public class DepositData
{

    public SpecialDepositOffer SpecialOffer { get; protected set; }
    public List<DepositAmount> DepositAmounts { get; protected set; }

    public DepositData(Dictionary<string, object> dict)
    {
        object o;

        int popularIndex = 1;
        if (dict.TryGetValue("MostPopular", out o))
            popularIndex = o.ParseInt();

        if (dict.TryGetValue("BonusName", out o))
                SpecialOffer = new SpecialDepositOffer(o as Dictionary<string, object>);

        DepositAmounts = new List<DepositAmount>();
        if (dict.TryGetValue("AmountData", out o))
        {
            int index = 0;
            foreach (var item in (Dictionary<string, object>)o)
            {
                DepositAmounts.Add(new DepositAmount(item, popularIndex == index));
                index++;
            }
        }
    }
}

public class DepositAmount
{
    public float amount { get;  set; }
    public bool isDiscountInPercent { get; protected set; }
    public float discountAmount { get; protected set; }
    public bool popular { get; protected set; }

    public float bonusCash { get { return isDiscountInPercent ? discountAmount / 100.0f * amount : discountAmount; } }
    public float savingsPercent { get { return isDiscountInPercent ? discountAmount : discountAmount * 100f / amount; } }
    public float amountWithBonus { get { return amount + bonusCash; } }

    public DepositAmount(KeyValuePair<string,object> pair, bool mostPopular)
    {
        popular = mostPopular;
        amount = pair.Key.ParseFloat();
        discountAmount = pair.Value.ToString().Substring(1).ParseFloat();
        isDiscountInPercent = pair.Value.ToString()[0] == '%';
    }

    public DepositAmount(float amount, float bonus)
    {
        this.amount = amount;
        discountAmount = bonus;
        isDiscountInPercent = false;
    }
}

public class SpecialDepositOffer
{

    public string Id { get; protected set; }
    public string Title { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public DateTime Expiration { get; protected set; }
    public SpriteData ImageData { get; protected set; }
    public float SecondsToExpiration { get { return (float)(Expiration - DateTime.UtcNow).TotalSeconds; } }

    public SpecialDepositOffer(Dictionary<string, object> dict)
    {
        object o;
        if (dict.TryGetValue("GroupId", out o))
            Id = o.ToString();
        else Id = string.Empty;

        if (dict.TryGetValue("BonusTitle", out o))
            Title = o.ToString();

        if (dict.TryGetValue("BonusName", out o))
            Name = o.ToString();

        if (dict.TryGetValue("BonusDescription", out o))
            Description = o.ToString();

        if (dict.TryGetValue("ImageUrl", out o))
            ImageData = new SpriteData(o.ToString());

        TimeSpan time = TimeSpan.Zero;
        if (dict.TryGetValue("TimeLeftForBonus", out o) && TimeSpan.TryParse(o.ToString(), out time))
            Expiration = DateTime.Now + time;
    }
}
