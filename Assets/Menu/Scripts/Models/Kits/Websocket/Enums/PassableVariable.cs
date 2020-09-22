
namespace GT.Websocket
{
    public enum PassableVariable
    {
        UserId,
        UserName,
        PictureUrl,
        TouchActive,
        DateOfBirth,
        OldPassword,
        NewPassword,
        OS,

        // ----------- Facebook Update ------------
        FacebookId,
        FacebookName,
        FacebookAddtionalId,
        // ----------- Facebook Update ------------

        // ----------- Facebook Friends -----------
        GamyTechFriends,
        // ----------- Facebook Friends -----------

        // ------------- Verification -------------
        Amount,
        FirstName,
        LastName,
        Gender,
        PhoneNumber,
        Street,
        Number,
        Country,
        City,
        PostalCode,
        IdImageLink,
        PayPal,
        IBAN,
        SWIFT,
        // ------------- Verification -------------

        // ---------------- CashIn ----------------
        PaymentKind,
        CashToDeposit,
        Pspid,
        PromoCode,
        TransactionTime,
        TransactionState,
        // ---------------- CashIn ----------------

        // ----------------- Apco -----------------
        CardLastDigits,
        // ----------------- Apco -----------------

        // ---------------- Store -----------------
        PurchaseMethod,
        ItemId,
        PurchaseId,
        // ---------------- Store -----------------

        // ----------- In App Purchase ------------
        PackageId,
        TransactionId,
        // ----------- In App Purchase ------------

        // ------------- Auto Deposit -------------
        IsAutoDepositActive,
        DepositAmount,
        UnderAmount,
        // ------------- Auto Deposit -------------

        // ------------- Support Deposit -------------
        Email,
        Complaint,
        Message,
        Phone,
        // ------------- Support Deposit -------------

        // ------------- Match History ------------
        MatchKind,
        StartIndex,
        // ------------- Match History ------------
    }
}
