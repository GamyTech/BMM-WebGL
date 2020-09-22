public struct CashInData
{
    public enum TransactionMethod
    {
        Apco,
        EasyTransac,
        PayPal,
        PaySafeCard,
        Skrill,
    }

    public TransactionMethod method;
    public DepositAmount depositAmount;
    public string PromoCode;

    public NewCreditCard newCardData { get; private set; }
    public GT.User.GTUser.SavedCreditCard savedCardData { get; private set; }

    public string paymentId;
    public string transactionDate;
    public string transactionState;

    public void SetPayPalTransaction(string paymentID, string date, string state)
    {
        paymentId = paymentID;
        transactionDate = date;
        transactionState = state;
    }

    public void RemovePayPalTransaction()
    {
        paymentId = null;
        transactionDate = null;
        transactionState = null;
    }

    public void SetNewCard(NewCreditCard card)
    {
        newCardData = card;
    }

    public void SetSavedCard(GT.User.GTUser.SavedCreditCard card)
    {
        savedCardData = card;
    }

    public void RemoveNewCard()
    {
        newCardData = null;
    }
}
