using SP.Dto.ProcessBreezeRequests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCountryCodeView : MonoBehaviour
{
    public delegate void SelectCountry(ISO3166Country item, int codeId);
    public SelectCountry OnSelect = (i, j) => { };

    public ObjectPool TitlePool;
    public ObjectPool ListItemPool;
    public ScrollRect ScrollList;

    private List<CountryCodeListItem> listItems;

    private void Start()
    {
        string[] alpha = new string[] 
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "K", "L", "M", "N", "O", "P", "Q", "R",
            "S", "T", "U", "V", "W", "X", "Y", "Z"
        };
        int curChar = 0;
        bool newChar = true;

        string savedAlpha3 = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.SMSCountryISO);

        listItems = new List<CountryCodeListItem>();
        bool hasSelected = false;
        float selectedItemIndex = 0;
        float linesCount = 0;

        IEnumerator ie = ISO3166.GetCollection().GetEnumerator();
        while (ie.MoveNext())
        {
            ISO3166Country item = (ISO3166Country)ie.Current;

            if (item.DialCodes == null || item.DialCodes.Length == 0)
                continue;

            while (curChar < alpha.Length && 
                item.Alpha2.Substring(0, 1) != alpha[curChar] && 
                item.Name.Substring(0, 1) != alpha[curChar])
            {
                newChar = true;
                ++curChar;
            }
            if (newChar)
            {
                GameObject title = TitlePool.GetObjectFromPool();
                title.InitGameObjectAfterInstantiation(ScrollList.content);
                title.transform.GetChild(0).GetComponent<Text>().text = alpha[curChar];
                newChar = false;
                if (!hasSelected) ++selectedItemIndex;
                ++linesCount;
            }

            for (int i = 0; i < item.DialCodes.Length; i++)
            {
                GameObject go = ListItemPool.GetObjectFromPool();
                go.InitGameObjectAfterInstantiation(ScrollList.content);
                CountryCodeListItem listItem = go.GetComponent<CountryCodeListItem>();
                bool isSelected = (!string.IsNullOrEmpty(savedAlpha3) && savedAlpha3 == item.Alpha3) ||
                                  (string.IsNullOrEmpty(savedAlpha3) && item.Alpha3 == UserController.Instance.gtUser.CountryISO);
                hasSelected = isSelected || hasSelected;
                if (!hasSelected) ++selectedItemIndex;
                ++linesCount;
                listItem.Populate(item, isSelected, i);
                listItem.OnSelect += CountryCodeListItem_OnSelect;
                listItems.Add(listItem);
            }
        }
        ie.Reset();

        if (!hasSelected && listItems.Count > 0)
        {
            selectedItemIndex = 0;
            listItems[0].SetSelected();
        }
        
        StartCoroutine(ScrollTo(selectedItemIndex / linesCount));
    }

    private IEnumerator ScrollTo(float position)
    {
        yield return new WaitForSeconds(0.1f);
        ScrollList.verticalNormalizedPosition = (1 - position);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < listItems.Count; i++)
            listItems[i].OnSelect -= CountryCodeListItem_OnSelect;
    }

    #region Events
    private void CountryCodeListItem_OnSelect(ISO3166Country country, int codeId)
    {
        OnSelect(country, codeId);
    }
    #endregion Events

    #region Input
    public void Close()
    {
        gameObject.SetActive(false);
    }
    #endregion Input
}
