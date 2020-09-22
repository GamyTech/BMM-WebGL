using System.Collections.Generic;
using GT.Websocket;

public class UserInfoData
{
    public string FirstName;
    public string LastName;
    public string PhoneNumber;
    public string Street;
    public string Number;
    public string Country;
    public string City;
    public string PostalCode;
    public string IdImageLink;

    public virtual Dictionary<string, object> ToStringDict(bool isVerified)
    {
        Dictionary<string, object> d = new Dictionary<string, object>();

        if (isVerified)
        {
            return d;
        }

        if (string.IsNullOrEmpty(FirstName) == false) d.Add(PassableVariable.FirstName.ToString(), FirstName);
        if (string.IsNullOrEmpty(LastName) == false) d.Add(PassableVariable.LastName.ToString(), LastName);
        if (string.IsNullOrEmpty(PhoneNumber) == false) d.Add(PassableVariable.PhoneNumber.ToString(), PhoneNumber);
        if (string.IsNullOrEmpty(Street) == false) d.Add(PassableVariable.Street.ToString(), Street);
        if (string.IsNullOrEmpty(Number) == false) d.Add(PassableVariable.Number.ToString(), Number);
        if (string.IsNullOrEmpty(Country) == false) d.Add(PassableVariable.Country.ToString(), Country);
        if (string.IsNullOrEmpty(City) == false) d.Add(PassableVariable.City.ToString(), City);
        if (string.IsNullOrEmpty(PostalCode) == false) d.Add(PassableVariable.PostalCode.ToString(), PostalCode);
        if (string.IsNullOrEmpty(IdImageLink) == false) d.Add(PassableVariable.IdImageLink.ToString(), IdImageLink);
        return d;
    }

    public override string ToString()
    {
        Dictionary<PassableVariable, object> d = GetPassableDictionary();       
        return d.Display();
    }

    public virtual Dictionary<PassableVariable, object> GetPassableDictionary()
    {
        Dictionary<PassableVariable, object> d = new Dictionary<PassableVariable, object>();

        if (FirstName != null) d.Add(PassableVariable.FirstName, FirstName);
        if (LastName != null) d.Add(PassableVariable.LastName, LastName);
        if (PhoneNumber != null) d.Add(PassableVariable.PhoneNumber, PhoneNumber);
        if (Street != null) d.Add(PassableVariable.Street, Street);
        if (Number != null) d.Add(PassableVariable.Number, Number);
        if (Country != null) d.Add(PassableVariable.Country, Country);
        if (City != null) d.Add(PassableVariable.City, City);
        if (PostalCode != null) d.Add(PassableVariable.PostalCode, PostalCode);
        if (IdImageLink != null) d.Add(PassableVariable.IdImageLink, IdImageLink);
        return d;
    }

    public virtual void Reset()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        PhoneNumber = string.Empty;
        Street = string.Empty;
        Number = string.Empty;
        Country = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;
        IdImageLink = string.Empty;
    }
}


