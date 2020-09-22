using System;

namespace GT.Websocket
{
    [Flags]
    public enum APIGetVariable //: long // Change when values count is grater then 32
    {
        None                        = 0 << 0,

        Virtual                     = 1 << 1,
        Loyalty                     = 1 << 2,
        CashData                    = 1 << 3,

        CardDetails                 = 1 << 4,
        PurchasedItems              = 1 << 5,

        DepositInfo                 = 1 << 6,

        AutoDeposit                 = 1 << 7,

        Verification                = 1 << 8,

        TimelyBonus                 = 1 << 9,

        HistoryMatches              = 1 << 10,

        GamyTechFriends             = 1 << 11,

        GetCards                    = 1 << 12,

        Wallet                      = 1 << 13,
    }
}


