using UnityEngine;
using System.Collections.Generic;
using System;

namespace GT.User
{
    public class HistoryMatches : FragmentedList<FragmentedListDynamicElement>
    {
        public TourneyHistoryData lastTourney;

        public void InitMatches(string currentUserId)
        {
            List<FragmentedListDynamicElement> elements = new List<FragmentedListDynamicElement>();
            List<object> matches = LoadSavedElementData();

            FragmentedListDynamicElement matchData;
            for (int i = 0; i < matches.Count; ++i)
            {
                matchData = matches[i] is string ? GetNewFiller() : new MatchHistoryData((Dictionary<string, object>)matches[i], currentUserId);
                elements.Add(matchData);
            }

            InitElements(elements, matches);
        }

        public void AddMatches(object data)
        {
            NeedToUpdate = false;

            if (!(data is Dictionary<string, object>))
            {
                IsListComplete = true;
                return;
            }

            Dictionary<string, object> dict = (Dictionary<string, object>)data;

            object o;
            int newMatchDone = 0;
            if (dict.TryGetValue("HistoryMatchesAmount", out o))
                newMatchDone = o.ParseInt();
            else
                Debug.LogError("HistoryMatchesAmount missing");

            if (!ShouldUpdateNewElements(newMatchDone))
                return;

            if (dict.TryGetValue("HistoryMatches", out o))
            {
                List<object> newMatchesdata = (List<object>)o;
                List<FragmentedListDynamicElement> matchData = new List<FragmentedListDynamicElement>();

                for (int i = 0; i < newMatchesdata.Count; ++i)
                    matchData.Add(new MatchHistoryData((Dictionary<string, object>)newMatchesdata[i], UserController.Instance.gtUser.Id));

                AddElements(newMatchDone, matchData, newMatchesdata);
            }
            else
                Debug.LogError("HistoryMatches missing");
            
            if (dict.TryGetValue("LastTourney", out o) && (o as Dictionary<string, object>).Count > 0)
            {
                lastTourney = new TourneyHistoryData(o as Dictionary<string, object>);
            }
        }

        protected override List<object> LoadSavedElementData()
        {
            return SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id).MatchHistory;
        }

        protected override FragmentedListDynamicElement GetNewFiller()
        {
            return new DynamicFillerElement();
        }

        protected override void SaveNewElementsData(List<object> elementsToSave)
        {
            SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
            user.MatchHistory = elementsToSave;
            SavedUsers.SaveUserToFile(user);
        }
    }
}
