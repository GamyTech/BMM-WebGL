using System.Collections.Generic;
using System;
using UnityEngine;
using GT.Store;

namespace GT.User
{
    [Serializable]
    public class SavedUsers
    {
        static string FILE_NAME = "Users.bin";

        static SavedUsers m_instance;
        static SavedUsers Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = (SavedUsers)GTDataManagementKit.LoadSerializableClassFromFile(GTDataManagementKit.LocalFolder.Default, FILE_NAME);

                    if (m_instance == null)
                    {
                        GTDataManagementKit.DeleteFile(FILE_NAME);
                        m_instance = new SavedUsers();
                    }
                }
                return m_instance;
            }
        }

        private Dictionary<string, SavedUser> UsersDictionary;

        private SavedUsers()
        {
            UsersDictionary = new Dictionary<string, SavedUser>();
        }

        public static void SaveUserToFile(SavedUser user)
        {
            Instance.UsersDictionary.AddOrOverrideValue(user.userId, user);
            GTDataManagementKit.SaveSerializableClassToFile(GTDataManagementKit.LocalFolder.Default, FILE_NAME, Instance);
        }

        public static SavedUser LoadOrCreateUserFromFile(string id)
        {
            SavedUser user = null;

            if(Instance.UsersDictionary.TryGetValue(id, out user))
                user.CleanBrokenVaraiable();
            else
            {
                user = new SavedUser(id);
                SaveUserToFile(user);
            }
            return user;
        }
    }

    [Serializable]
    public class SavedUser
    {
        public string userId;
        public int LastSelectedGameType;
        public int LastSelectedCategory;
        public Dictionary<string, float[]> cashBetCategories = new Dictionary<string, float[]>();
        public Dictionary<string, float[]> virtualBetCategories = new Dictionary<string, float[]>();
        public Dictionary<Enums.StoreType, string[]> selectedStoreItems = new Dictionary<Enums.StoreType, string[]>();
        public List<Dictionary<string, object>> pendingInAppPurchase = new List<Dictionary<string, object>>();
        public List<object> MatchHistory = new List<object>();
        public List<float> CustomBets = new List<float>();

        public SavedUser(string id)
        {
            this.userId = id;
        }

        public void CleanBrokenVaraiable()
        {
            if (cashBetCategories == null)
                cashBetCategories = new Dictionary<string, float[]>();
            if (virtualBetCategories == null)
                virtualBetCategories = new Dictionary<string, float[]>();
            if (selectedStoreItems == null)
                selectedStoreItems = new Dictionary<Enums.StoreType, string[]>();
            if (pendingInAppPurchase == null)
                pendingInAppPurchase = new List<Dictionary<string, object>>();
            if (MatchHistory == null)
                MatchHistory = new List<object>();
            if (CustomBets == null)
                CustomBets = new List<float>();
        }

        public void UpdateBetCategory(string category, List<BetRoom> rooms)
        {
            if (rooms == null || rooms.Count == 0)
                return;

            List<float> selectedBets = new List<float>();
            for (int i = 0; i < rooms.Count; i++)
                if (rooms[i].Selected)
                    selectedBets.Add(rooms[i].BetAmount);

            Dictionary<string, float[]> SavedBets = GetSavedBetByMatchkind(rooms[0].Kind);
            if (SavedBets != null)
            {
                SavedBets.AddOrOverrideValue(category, selectedBets.ToArray());
                BetRooms.RefreshBetSelection(category, rooms[0].Kind, SavedBets);
            }
        }

        public Dictionary<string, float[]> GetSavedBetByMatchkind(Enums.MatchKind kind)
        {
            switch (kind)
            {
                case Enums.MatchKind.Cash:
                    return cashBetCategories;
                case Enums.MatchKind.Virtual:
                    return virtualBetCategories;
                default:
                    return null;
            }
        }

    }
}
