using System.Collections;
using GT.Websocket;
using System.Collections.Generic;
using UnityEngine;

namespace GT.User
{
    public class GTMails : FragmentedList<FragmentedListDynamicElement>
    {
        public void InitMails()
        {
            List<FragmentedListDynamicElement> elements = new List<FragmentedListDynamicElement>();
            List<object> mails = LoadSavedElementData();

            FragmentedListDynamicElement mailData;
            for (int i = 0; i < mails.Count; ++i)
            {
                mailData = mails[i] is string ? GetNewFiller() : new GTMail((Dictionary<string, object>)mails[i]);
                elements.Add(mailData);
            }

            InitElements(elements, mails);
        }

        public void AddMail(object data)
        {
            NeedToUpdate = false;

            if (!(data is Dictionary<string, object>))
            {
                IsListComplete = true;
                return;
            }

            Dictionary<string, object> dict = (Dictionary<string, object>)data;

            object o;
            int gotNewMail = 0;
            if (dict.TryGetValue("MailAmount", out o))
                gotNewMail = o.ParseInt();
            else
                Debug.LogError("MailAmount missing");

            if (!ShouldUpdateNewElements(gotNewMail))
                return;

            if (dict.TryGetValue("Mails", out o))
            {
                List<object> newMailsData = (List<object>)o;
                List<FragmentedListDynamicElement> mails = new List<FragmentedListDynamicElement>();

                for (int i = 0; i < newMailsData.Count; ++i)
                    mails.Add(new GTMail((Dictionary<string, object>)newMailsData[i]));

                AddElements(gotNewMail, mails, newMailsData);
            }
            else
                Debug.LogError("Mails missing");
        }

        protected override List<object> LoadSavedElementData()
        {
            string oldData = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Mails);
            return string.IsNullOrEmpty(oldData) ? new List<object>() : (List<object>)MiniJSON.Json.Deserialize(oldData);
        }

        protected override FragmentedListDynamicElement GetNewFiller()
        {
            return new DynamicFillerElement();
        }

        protected override void SaveNewElementsData(List<object> elementsToSave)
        {
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.Mails, MiniJSON.Json.Serialize(elementsToSave));
        }
    }
}
