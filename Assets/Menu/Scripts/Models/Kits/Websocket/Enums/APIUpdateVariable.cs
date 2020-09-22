using System;

namespace GT.Websocket
{
    [Flags]
    public enum APIUpdateVariable
    {
        None            = 0 << 0,
        SetTouchActive  = 1 << 0,
        UpdateUserName     = 1 << 1,
        UpdateProfilePicture  = 1 << 2,
        SetPassword     = 1 << 3,
        SetAutoDeposit  = 1 << 4,
        UpdateFacebook  = 1 << 5,
        DeleteApcoCard  = 1 << 6,
        ConsumeItem     = 1 << 7,
    }
}
