using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomCollectionView : MonoBehaviour {

    public ToggleGroup group;
    public ObjectPool roomsPool;

    private List<GameObject> activeViews = new List<GameObject>();
    public List<BetRoom> rooms { get; private set; }

    void OnDisable()
    {
        roomsPool.PoolObjects(activeViews);
        activeViews.Clear();
    }

    public void Init(List<BetRoom> roomsList, bool multipleSelection)
    {
        this.rooms = roomsList;
        rooms.Sort((a, b) => a.BetAmount.CompareTo(b.BetAmount));

        for (int i = 0; i < rooms.Count; i++)
            InitRoom(rooms[i], multipleSelection);

        RefreshSelected();
        OnToggleChanged();
    }

    protected virtual void InitRoom(BetRoom room, bool multipleSelection)
    {
        GameObject go = roomsPool.GetObjectFromPool();
        go.InitGameObjectAfterInstantiation(roomsPool.transform);
        BetRoomView rv = go.GetComponent<BetRoomView>();
        if(multipleSelection)
            rv.Populate(room, null, OnToggleChanged);
        else
            rv.Populate(room, group);

        activeViews.Add(go);
    }

    private void RefreshSelected()
    {
        for (int i = 0; i < activeViews.Count; i++)
            activeViews[i].GetComponent<BetRoomView>().RefreshSelected();
    }

    private void OnToggleChanged()
    {
        for (int i = 0; i < rooms.Count; i++)
            if (rooms[i].Selected)
                return;
        if(rooms.Count > 0)
        {
            rooms[0].Selected = true;
            RefreshSelected();
        }
    }

    public string GetCurrentSelectedAmounts()
    {
        List<string> selected = new List<string>();
        for (int i = 0; i < rooms.Count; i++)
            if (rooms[i].Selected)
                selected.Add(rooms[i].BetAmount.ToString());
        
        return selected.ToSeparatedString(",");
    }

    internal void DeselectAll()
    {
        for (int i = 0; i < rooms.Count; i++)
            rooms[i].Selected = false;

        RefreshSelected();
    }
}
