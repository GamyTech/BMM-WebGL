using GT.Store;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoreToggleItemData
{
    public string itemName;
    public List<StoreItem> items;
    public StoreToggleItemData(string name, List<StoreItem> items)
    {
        itemName = name;
        this.items = items;

    }
}