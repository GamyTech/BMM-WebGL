using System.Collections.Generic;
using GT.Websocket;

public class CashOutData : UserInfoData
{
    public string PayPal;
    public string IBAN;
    public string SWIFT;
    public string Amount;

    public override Dictionary<string, object> ToStringDict(bool isVerified)
    {
        Dictionary<string, object> d = base.ToStringDict(isVerified);

        if (string.IsNullOrEmpty(Amount) == false) d.Add(PassableVariable.Amount.ToString(), Amount);

        if (isVerified)
        {
            return d;
        }
        if (string.IsNullOrEmpty(PayPal) == false) d.Add(PassableVariable.PayPal.ToString(), PayPal);
        if (string.IsNullOrEmpty(IBAN) == false) d.Add(PassableVariable.IBAN.ToString(), IBAN);
        if (string.IsNullOrEmpty(SWIFT) == false) d.Add(PassableVariable.SWIFT.ToString(), SWIFT);
        return d;
    }

    public override Dictionary<PassableVariable, object> GetPassableDictionary()
    {
        Dictionary<PassableVariable, object> d = base.GetPassableDictionary();
        
        if (PayPal != null) d.Add(PassableVariable.PayPal, PayPal);
        if (IBAN != null) d.Add(PassableVariable.IBAN, IBAN);
        if (SWIFT != null) d.Add(PassableVariable.SWIFT, SWIFT);
        if (Amount != null) d.Add(PassableVariable.Amount, Amount);

        return d;
    }

    public override void Reset()
    {
        base.Reset();
        PayPal = string.Empty;
        IBAN = string.Empty;
        SWIFT = string.Empty;
        Amount = string.Empty;
    }
}


