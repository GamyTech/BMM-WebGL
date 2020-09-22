using System.Collections.Generic;
using GT.User;
using UnityEngine;

namespace GT.Store
{
    public delegate void SelectedForSwapChangedHandler(StoreItem item);
    public delegate void SelectedListChangedHandler(List<StoreItem> items);
    public delegate void SelectedSlotSetHandler(int slotIndex);

    public class Selected
    {
        public event SelectedForSwapChangedHandler OnSelectedForSwapChanged;
        public event SelectedListChangedHandler OnSelectedListChanged;
        public event SelectedSlotSetHandler OnSelectedSlotSetHandler;

        internal string[] selectedIds;

        private bool canBeUnselected;
        private Enums.StoreType storeType;
        private int count;
        private List<StoreItem> m_selectedItems;
        private StoreItem m_selectedForSwap;
        private bool loadedFromFile;

        public List<StoreItem> selectedItems
        {
            get { return m_selectedItems; }
            private set
            {
                if (Utils.SetProperty(ref m_selectedItems, value) && OnSelectedListChanged != null)
                        OnSelectedListChanged(m_selectedItems);
            }
        }

        public StoreItem selectedForSwap
        {
            get { return m_selectedForSwap; }
            private set
            {
                if (Utils.SetProperty(ref m_selectedForSwap, value) && OnSelectedForSwapChanged != null)
                    OnSelectedForSwapChanged(m_selectedForSwap);
            }
        }

        public StoreItem GetFirstSelected()
        {
            if (selectedItems != null && selectedItems.Count > 0 && selectedItems[0] != null)
                return selectedItems[0];

            return null;
        }

        public string GetFirstSelectedId()
        {
            if (selectedIds != null && selectedIds.Length > 0 && selectedIds[0] != null)
                return selectedIds[0];

            return null;
        }

        internal Selected(Enums.StoreType storeType, int count, bool canBeUnselected)
        {
            this.canBeUnselected = canBeUnselected;
            this.storeType = storeType;
            this.count = count;
            if (count == 0) return;
            selectedItems = new List<StoreItem>();
            for (int i = 0; i < count; i++)
                selectedItems.Add(null);

            SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
            if (user.selectedStoreItems.TryGetValue(storeType, out selectedIds))
                loadedFromFile = true;
            else
                selectedIds = new string[0];
        }

        internal void InitItems(List<StoreItem> items)
        {
            if (count == 0) return;

            for (int i = 0; i < items.Count; i++)
            {
                StoreItem item = items[i];
                item.isSelectable = true;
                if (canBeUnselected)
                {
                    item.SelectAction = () => SelectToggle(!item.Selected, item);
                }
                else
                {
                    item.SelectAction = () => SelectToggle(true, item);
                }
            }

            if (loadedFromFile == false)
            {
                SetDefaultSelected(items);
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                CheckItem(items[i]);
            }
        }

        private void SetDefaultSelected(List<StoreItem> items)
        {
            if (canBeUnselected) return;

            int runningIndex = 0;
            for (int i = 0; i < selectedItems.Count; i++)
            {
                while (runningIndex < items.Count)
                {
                    if (items[runningIndex].IsSelectable(1)) // TODO: change later when user has rank
                    {
                        SelectItem(true, i, items[runningIndex]);
                        runningIndex++;
                        break;
                    }
                    runningIndex++;
                }
            }
        }

        private void CheckItem(StoreItem item)
        {
            for (int i = 0; i < selectedIds.Length; i++)
            {
                if (selectedIds[i] == item.Id)
                {
                    SetItem(true, i, item);
                    return;
                }
            }
        }

        public void SelectItem(bool selected, StoreItem item)
        {
            if (selected == item.Selected) return;

            SelectToggle(selected, item);
        }

        public void DeselectItemForSwap()
        {
            SelectItem(false, selectedForSwap);
            selectedForSwap = null;
        }

        private void SelectToggle(bool isOn, StoreItem item)
        {
            if (count > 1)
            {
                if (selectedForSwap != null) // in case selected already from a list
                {
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        if (selectedItems[i] == null || selectedItems[i].Id.Equals(item.Id))
                        {
                            SelectItem(true, i, selectedForSwap);
                            selectedForSwap = null;
                            return;
                        }
                    }
                    selectedForSwap = item;
                }
                else // nothing was selected
                {
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        if (selectedItems[i] == null)
                        {
                            SelectItem(true, i, item);
                            return;
                        }
                        if (selectedItems[i].Id.Equals(item.Id))
                            return;
                    }
                    selectedForSwap = item;
                }
            }
            else
            {
                SelectItem(isOn, 0, item);
            }
        }

        private void SetItem(bool selected, int index, StoreItem item)
        {
            item.Selected = selected;
            if (selected && selectedItems[index] != null)
                selectedItems[index].Selected = false;
            selectedItems[index] = selected ? item : null;

            if (OnSelectedListChanged != null)
                OnSelectedListChanged(selectedItems);
            if (OnSelectedSlotSetHandler != null)
                OnSelectedSlotSetHandler(index);
        }

        private void SelectItem(bool selected, int index, StoreItem item)
        {
            if (selected == item.Selected) return;

            SetItem(selected, index, item);

            selectedIds = GetSelectedIds(selectedItems);
            SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
            user.selectedStoreItems.AddOrOverrideValue(storeType, selectedIds);
            SavedUsers.SaveUserToFile(user);
        }

        private string[] GetSelectedIds(List<StoreItem> items)
        {
            List<string> itemIds = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    itemIds.Add(items[i].Id);
                }
            }
            return itemIds.ToArray();
        }
    }
}
