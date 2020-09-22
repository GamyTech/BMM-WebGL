
namespace GT.Websocket
{
    public enum APIResponseVariable
    {
        //----------------- UGLYNESS
        SetPassword,
        //----------- Common Variables -----------
        PurchasedItems,
        UpdatedItems,
        //----------- Common Variables -----------

        //------- Connect Request Variables ------
        UserDetails,
        Wallet,
        //------- Connect Request Variables ------

        //------------ User Variables ------------
        UpdateUserName,
        UpdateProfilePicture,
        Verification,
        IsVerified,
        CashOutPending,
        Score,
        TimelyBonus,
        DailyBonus,
        //------------ User Variables ------------

        //--------------- Cash In ----------------
        GetCards,
        DepositInfo,
        AutoDeposit,
        IsPromoCodeValid,
        //--------------- Cash In ----------------

        //------------ History -------------
        HistoryMatches,
        //------------ History -------------

        //------------ Friends Data --------------
        GamyTechFriends,
        //------------ Friends Data --------------

        //------------ Friends Data --------------
        UserTotalMatchesCount,
        //------------ Friends Data --------------


        //------------ Paypal Data --------------
        PayPalOrderLink,
        //------------ Paypal Data --------------
    }
}
