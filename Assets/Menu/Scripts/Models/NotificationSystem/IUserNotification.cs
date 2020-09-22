using UnityEngine;
using System.Collections;
using GT.User;

namespace GT.Notifications
{
    public interface IUserNotification
    {
        void SetUser(GTUser user);
    }
}
