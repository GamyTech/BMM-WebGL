using UnityEngine;
using System.Collections;
using System;
using GT.User;

namespace GT.Notifications
{
    public class HistoryNotification : Notification, IUserNotification
    {
        private GTUser user;

        public HistoryNotification() : base()
        {
            
        }

        ~HistoryNotification()
        {
            Unregister();
        }

        public override void ForceUpdate()
        {
            UpdateNotification();
        }

        public void SetUser(GTUser user)
        {
            this.user = user;
            if (user != null)
            {
                Register();
                UpdateNotification();
            }
            else Reset();
        }

        protected override void UpdateNotification()
        {
            if (user.MatchHistoryData == null)
            {
                Reset();
                return;
            }

            //SavedUser savedUser = SavedUsers.LoadOrCreateUserFromFile(user);
            //if (user.MatchHistoryData.ElementsList != null)
            //    notificationCount = Mathf.Max(0, user.MatchHistoryData.TotalElementCount - savedUser.MatchResultsViewed);
        }

        #region Register/Unregister Events
        private void Register()
        {
            user.OnMatchHistoryChanged += User_OnMatchHistoryChanged;
        }

        private void Unregister()
        {
            user.OnMatchHistoryChanged -= User_OnMatchHistoryChanged;
        }
        #endregion Register/Unregister Events

        #region Events
        private void User_OnMatchHistoryChanged(HistoryMatches newValue)
        {
            UpdateNotification();
        }
        #endregion Events
    }
}
