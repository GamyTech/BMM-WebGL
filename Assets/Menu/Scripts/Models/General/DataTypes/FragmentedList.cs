using System.Collections.Generic;

public abstract class FragmentedList<T> where T : IFragmentedListElement
{
    public int TotalElementCount { get; protected set; }
    public List<T> ElementsList { get; protected set; }
    public bool IsListComplete { get; protected set; }
    public bool NeedToUpdate { get; protected set; }

    private int lastUpdatedIndex = 0;

    public FragmentedList()
    {
        ElementsList = new List<T>();
        SetNeedUpdate();
    }

    public void InitElements(List<T> elements, List<object> elementToSave)
    {
        elements.Sort((a, b) => b.Id.CompareTo(a.Id));
        ElementsList = elements;
        SaveNewElementsData(elementToSave);
    }

    public virtual void AddElements(int newTotalElement, List<T> newElements, List<object> elementToSave)
    {
        NeedToUpdate = false;

        if (!ShouldUpdateNewElements(newTotalElement))
            return;

        newElements.Sort((a, b) => b.Id.CompareTo(a.Id));
        TotalElementCount = newTotalElement;
        AddNewElementAndSave(newElements, elementToSave);
        IsListComplete = ElementsList.Count >= TotalElementCount;
    }

    public void AddNewElementAndSave(List<T> newElements, List<object> newElementsToSave)
    {
        if (newElements == null)
            return;

        int insertIndex = 0;
        int fillerOrEndIndex = 0;

        List<object> savedelementdata = LoadSavedElementData();

        int firstId = newElements[0].Id;
        int lastId = newElements[newElements.Count - 1].Id;
        int currentId = firstId;
        int savedElementsCount = ElementsList.Count;
        while (insertIndex < savedElementsCount)
        {
            if (!(ElementsList[insertIndex].IsFiller))
            {
                if (ElementsList[insertIndex].Id < firstId)
                    break;
                
                if (ElementsList[insertIndex].Id == currentId)
                    currentId = newElements[++fillerOrEndIndex].Id;
                
            }
            ++insertIndex;
        }
        if (insertIndex > 0 && insertIndex < savedElementsCount && ElementsList[insertIndex - 1].IsFiller)
        {
            if (lastId < ElementsList[insertIndex].Id)
            {
                savedelementdata.RemoveAt(insertIndex - 1);
                ElementsList.RemoveAt(insertIndex - 1);
            }
            --insertIndex;
        }

        if (insertIndex == 0 && savedElementsCount > 0 && lastId > ElementsList[0].Id)
            ElementsList.Insert(0, GetNewFiller());

        int index;
        for (int i = 0; i < newElements.Count; ++i)
        {
            index = insertIndex + i - fillerOrEndIndex;
            lastUpdatedIndex = index;

            if (index < ElementsList.Count && !ElementsList[index].IsFiller && ElementsList[index].Id == newElements[i].Id)
            {
                ElementsList[index] = newElements[i];
                savedelementdata[index] = newElementsToSave[i];
            }
            else
            {
                ElementsList.Insert(index, newElements[i]);
                savedelementdata.Insert(index, newElementsToSave[i]);
            }
        }
        SaveNewElementsData(savedelementdata);
    }

    protected abstract List<object> LoadSavedElementData();
    protected abstract void SaveNewElementsData(List<object> elementsToSave);
    protected abstract T GetNewFiller();

    public void SetNeedUpdate()
    {
        NeedToUpdate = true;
    }

    protected bool ShouldUpdateNewElements(int newTotalElementsCount)
    {
        return TotalElementCount != newTotalElementsCount || !IsListComplete || lastUpdatedIndex < ElementsList.Count - 1;
    }

    public int GetNextUnupdatedIndex()
    {
        for (int i = lastUpdatedIndex; i < ElementsList.Count; i++)
            if (ElementsList[i].NeedToUpdate)
                return i;
        
        return ElementsList.Count;
    }
}

public interface IFragmentedListElement
{
    bool IsFiller { get; }
    bool NeedToUpdate { get; }
    int Id { get; }
}
