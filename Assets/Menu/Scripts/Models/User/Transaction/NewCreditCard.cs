using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

public class NewCreditCard
{
    public Enums.CreditCardType CardType { get; private set; }
    public string CardNumber { get; private set; }
    public string SecurityCode { get; private set; }
    public string FullName { get; private set; }
    public string expirationMonth { get; private set; }
    public string expirationYear { get; private set; }

    public int DigitLenghtRequired { get; private set; }
    public int CVVLenghtRequired { get; private set; }

    public NewCreditCard()
    {
        Reset();
    }

    public void Reset()
    {
        CardType = Enums.CreditCardType.Unknown;
        CardNumber = null;
        SecurityCode = null;
        FullName = null;
        expirationMonth = null;
        expirationYear = null;
    }

    public void setCardType(string number)
    {
        CardType = ParseCardType(number);

        int digits;
        int securityCodeLenght;
        GetCardLenthsFromType(CardType, out digits, out securityCodeLenght);
        DigitLenghtRequired = digits;
        CVVLenghtRequired = securityCodeLenght;
    }

    public bool SetCardNumbers(string numbers, out string error)
    {
        CardNumber = string.Empty;

        setCardType(numbers);

        error = null;
        if (numbers.Length != DigitLenghtRequired || !Mod10Check(numbers))
        {
            error = ("Invalid card number");
            return false;
        }

        CardNumber = numbers;
        return true;
    }

    public bool SetExpirationDate(string str, out string error)
    {
        expirationMonth = string.Empty;
        expirationYear = string.Empty;

        error = null;
        if (string.IsNullOrEmpty(str))
            return false;


        string monthStr = string.Empty;
        string yearStr = string.Empty;
        string exp = str.Replace(" ", string.Empty).Replace("/", string.Empty);
        if (exp.Length == 4)
        {
            monthStr = exp.Substring(0, 2);
            yearStr = exp.Substring(2, 2);
        }
        else
        {
            error = "Invalid date";
            return false;
        }

        int year;
        int month;
        if (!int.TryParse(monthStr, out month) || month < 1 || month > 12 || !int.TryParse(yearStr, out year))
        {
            error = "Invalid date";
            return false;
        }

        int y = year + 2000;
        DateTime date = new DateTime(y, month, DateTime.DaysInMonth(y, month));
        if ((date - DateTime.Now).TotalSeconds < 0)
        {
            error = "Card expired";
            return false;
        }

        expirationMonth = monthStr;
        expirationYear = yearStr;
        return true;
    }

    public bool SetSecurityCode(string str, out string error)
    {
        SecurityCode = string.Empty;
        error = null;
        if (string.IsNullOrEmpty(str))
            return false;

        if (str.Length != CVVLenghtRequired)
        {
            error = "CVV not Valid";
            return false;
        }

        SecurityCode = str;
        return true;
    }

    public bool SetOwnerName(string name)
    {
        FullName = string.Empty;
        if (string.IsNullOrEmpty(name))
            return false;

        FullName = name;
        return true;
    }

    public bool IsValidCard()
    {
        if (!string.IsNullOrEmpty(CardNumber) &&
            !string.IsNullOrEmpty(expirationMonth) &&
            !string.IsNullOrEmpty(expirationYear) &&
            !string.IsNullOrEmpty(SecurityCode) &&
            !string.IsNullOrEmpty(FullName))
            return true;
        return false;
    }

    public static Enums.CreditCardType ParseCardType(string num)
    {
        Enums.CreditCardType cardType = Enums.CreditCardType.Unknown;
        if (Regex.IsMatch(num, @"^(34|37)"))// American express
        {
            cardType = Enums.CreditCardType.AmericanExpress;
        }
        //if (Regex.IsMatch(str, @"^(62|88) ")) // China UnionPay
        //{
        //    Debug.Log("China UnionPay");
        //}
        if (Regex.IsMatch(num, @"^(4)")) //Visa
        {
            cardType = Enums.CreditCardType.Visa;
        }
        if (Regex.IsMatch(num, @"^(36|38|39|309|30[0-5])"))    //Diners Club
        {
            cardType = Enums.CreditCardType.DinersClub;
        }
        if (Regex.IsMatch(num, @"^(6011|622(12[6-9]|1[3-9][0-9]|[2-8][0-9]{2}|9[0-1][0-9]|92[0-5]|64[4-9])|65)")) //Discover Card 6011, 622126-622925, 644-649, 65
        {
            cardType = Enums.CreditCardType.Discover;
        }
        if (Regex.IsMatch(num, @"^(5[0-5])")) // Master Card
        {
            cardType = Enums.CreditCardType.Mastercard;
        }
        //if (Regex.IsMatch(str, @"^(5019)")) //Dankort
        //{
        //    Debug.Log("Dankort");
        //}
        //if (Regex.IsMatch(str, @"^(6304|6706|6771|6709)")) //Laser
        //{
        //    Debug.Log("Laser");
        //}
        if (Regex.IsMatch(num, @"^(35)")) //JCB
        {
            cardType = Enums.CreditCardType.JCB;
        }
        return cardType;
    }

    public static void GetCardLenthsFromType(Enums.CreditCardType type, out int digits, out int securityCodeLenght)
    {
        switch (type)
        {
            case Enums.CreditCardType.Visa:
                digits = 16;
                securityCodeLenght = 3;
                break;
            case Enums.CreditCardType.Mastercard:
                digits = 16;
                securityCodeLenght = 3;
                break;
            case Enums.CreditCardType.AmericanExpress:
                digits = 15;
                securityCodeLenght = 4;
                break;
            case Enums.CreditCardType.DinersClub:
                digits = 14;
                securityCodeLenght = 3;
                break;
            case Enums.CreditCardType.Discover:
                digits = 16;
                securityCodeLenght = 3;
                break;
            case Enums.CreditCardType.JCB:
                digits = 16;
                securityCodeLenght = 3;
                break;
            default:
                digits = 16;
                securityCodeLenght = 3;
                break;
        }
    }

    public static bool Mod10Check(string creditCardNumber)
    {
        //// check whether input string is null or empty
        if (string.IsNullOrEmpty(creditCardNumber))
        {
            return false;
        }

        //// 1.	Starting with the check digit double the value of every other digit 
        //// 2.	If doubling of a number results in a two digits number, add up
        ///   the digits to get a single digit number. This will results in eight single digit numbers                    
        //// 3. Get the sum of the digits
        int sumOfDigits = creditCardNumber.Where((e) => e >= '0' && e <= '9')
                        .Reverse()
                        .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                        .Sum((e) => e / 10 + e % 10);


        //// If the final sum is divisible by 10, then the credit card number
        //   is valid. If it is not divisible by 10, the number is invalid.            
        return sumOfDigits % 10 == 0;
    }

    public static string CardNumToString(string str)
    {
        int num;
        string card = str.Replace(" ", string.Empty);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < card.Length; i++)
        {
            if (int.TryParse(card[i].ToString(), out num))
            {
                builder.Append(card[i]);
                if ((i + 1) >= card.Length)
                    continue;
                if ((i + 1) % 4 == 0)
                {
                    builder.Append(' ');
                }
            }

            else
                return builder.ToString();
        }
        return builder.ToString();
    }

    public override string ToString()
    {
        return "[type:" + CardType + "], [num:" + CardNumber + "], [month:" + expirationMonth + "], [year:" + expirationYear +
            "], [cvv:" + SecurityCode + "], [name:" + FullName + "]. IsValid ---> " + IsValidCard();
    }
}