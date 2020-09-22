using System.Collections.Generic;
using GT.Websocket;

public class UserVirificationData : UserInfoData
{
    public string Email;

    public override Dictionary<string, object> ToStringDict(bool isVerified)
    {
        Dictionary<string, object> d = base.ToStringDict(isVerified);
        if (isVerified)
        {
            return d;
        }
        if (string.IsNullOrEmpty(Email) == false) d.Add(PassableVariable.Email.ToString(), Email);
        return d;
    }

    public override Dictionary<PassableVariable, object> GetPassableDictionary()
    {
        Dictionary<PassableVariable, object> d = base.GetPassableDictionary();        
        if (Email != null) d.Add(PassableVariable.Email, Email);
        return d;
    }

    public override void Reset()
    {
        base.Reset();
        Email = string.Empty;
    }
}