using System.Collections.Generic;
using System;
using UnityEngine;
using GT.Assets;

namespace GT.Store
{
    public class Stores
    {
        const string STORE_DATA_PATH = "StoreData";

        public Dictionary<Enums.StoreType, Store> storesDict { get; private set; }
        public List<Store> storesList { get; private set; }

        internal static Dictionary<Enums.StoreType, StoreData> StoresDataDict;
        private bool isDataInitialized = false;

        public delegate void Changed<T>(T newValue);
        public event Changed<Stores> OnStoresUpdated;

        public Stores(List<object> list)
        {
            if (isDataInitialized == false)
                InitStoreData();

            storesDict = new Dictionary<Enums.StoreType, Store>();
            storesList = new List<Store>();

            for (int i = 0; i < list.Count; i++)
            {
                Store store = new Store((Dictionary<string, object>)list[i]);
                storesDict.Add(store.storeType, store);
                storesList.Add(store);
            }
        }

        public void SetPurchasedItems(PurchasedItems purchasedItems)
        {
            if (purchasedItems == null || purchasedItems.items == null || purchasedItems.items.Count == 0)
                return;

            if (OnStoresUpdated != null)
                OnStoresUpdated(this);

            for (int i = 0; i < purchasedItems.items.Count; i++)
            {
                PurchasedItem item = purchasedItems.items[i];
                Store store = GetStore(item.storeType);
                if (store == null)
                    continue;
                StoreItem itemInStore = store.items.Find(x => x.Id == item.itemId);
                if(itemInStore != null)
                    itemInStore.UpdatePurchasedItem(item);
            }
        }

        public void UpdatePurchasedItems(PurchasedItems purchasedItems)
        {
            for (int i = 0; i < purchasedItems.items.Count; i++)
            {
                PurchasedItem item = purchasedItems.items[i];
                GetStore(item.storeType).items.Find(x => x.Id == item.itemId).UpdatePurchasedItem(item);
            }
        }

        public List<BaseIAPCost> GetIAPItemIds()
        {
            List<BaseIAPCost> list = new List<BaseIAPCost>();
            foreach (var store in storesDict)
            {
                for (int i = 0; i < store.Value.items.Count; i++)
                    if (store.Value.items[i].Cost.IAPCost != null)
                        list.Add(store.Value.items[i].Cost.IAPCost);
            }
            return list;
        }

        public Store GetStore(Enums.StoreType type)
        {
            Store store = null;
            storesDict.TryGetValue(type, out store);
            return store;
        }

        public StoreItem GetItem(Enums.StoreType type, string itemID)
        {
            Store store = GetStore(type);
            if(store == null)
            {
                Debug.LogError(type.ToString() + " doesn't exist in storeData");
                return null;
            }
            return store.GetItemById(itemID);
        }

        public Store GetStoreFromPage(Page page)
        {
            foreach (var item in storesDict)
            {
                if (item.Value.storeType == page.StoreType)
                    return item.Value;
            }
            return null;
        }

        public List<Enums.StoreType> GetExistingStoresTypes()
        {
            List<Enums.StoreType> types = new List<Enums.StoreType>();
            foreach (var item in storesDict)
            {
                if (types.Contains(item.Value.storeType) == false && !item.Value.isStandalone)
                    types.Add(item.Value.storeType);
            }
            return types;
        }

        private void InitStoreData()
        {
            isDataInitialized = true;
            StoresDataDict = new Dictionary<Enums.StoreType, StoreData>();
            StoreData[] stores = Resources.LoadAll<StoreData>(STORE_DATA_PATH);
            for (int i = 0; i < stores.Length; i++)
                StoresDataDict.Add(stores[i].storeType, stores[i]);
        }

        public override string ToString()
        {
            return storesDict.Display<Enums.StoreType, Store>(Environment.NewLine);
        }
    }
}
