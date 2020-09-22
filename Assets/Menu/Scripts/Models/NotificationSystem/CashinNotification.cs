using UnityEngine;
using System.Collections;
using GT.User;

namespace GT.Notifications
{
    public class CashinNotification : Notification, IUserNotification
    {
        private MonoBehaviour mono;
        private bool deposit = false;
        private GTUser user;

        IEnumerator getDepositDataRoutine;

        public CashinNotification(MonoBehaviour mono) : base()
        {
            this.mono = mono;
            this.deposit = false;
        }

        ~CashinNotification()
        {
            Unregister();
        }

        public override void ForceUpdate()
        {
            SetDepositData(user.DepositInfo);
            UpdateNotification();
        }

        public void SetUser(GTUser user)
        {
            this.user = user;
            if (user != null)
            {
                Register();
                SetDepositData(user.DepositInfo);
                UpdateNotification();
            }
            else Reset();
        }

        protected override void UpdateNotification()
        {
            int tempCount = 0;
            if (deposit) tempCount++;
            notificationCount = tempCount;
        }

        protected override void Reset()
        {
            base.Reset();
            deposit = false;
        }

        #region Register/Unregister Events
        private void Register()
        {
            user.OnDepositInfoChanged += GtUser_OnDepositInfoChanged;
        }

        private void Unregister()
        {
            user.OnDepositInfoChanged -= GtUser_OnDepositInfoChanged;
        }
        #endregion Register/Unregister Events

        #region Aid Functions
        private void SetDepositData(DepositData depositData)
        {
            if (depositData == null || depositData.SpecialOffer == null)
                return;

            string id = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.SpecialOfferID);
            deposit = !id.Equals(depositData.SpecialOffer.Id);
            if (depositData.SpecialOffer.SecondsToExpiration > 0)
                StartTimerForDepositData();
        }

        private void StartTimerForDepositData()
        {
            if (getDepositDataRoutine != null)
                mono.StopCoroutine(getDepositDataRoutine);
            getDepositDataRoutine = Utils.Wait(user.DepositInfo.SpecialOffer.SecondsToExpiration, () => UserController.Instance.GetUserVarsFromServer(Websocket.APIGetVariable.DepositInfo));
            mono.StartCoroutine(getDepositDataRoutine);
        }
        #endregion Aid Functions

        #region Events
        private void GtUser_OnDepositInfoChanged(DepositData newValue)
        {
            SetDepositData(newValue);
            UpdateNotification();
        }
        #endregion Events
    }
}
