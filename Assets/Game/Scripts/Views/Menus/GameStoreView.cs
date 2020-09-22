using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GT.Store;
using System;

public class GameStoreView : MonoBehaviour
{

    #region Public Members
    public FadingElement fadingelement;
    public HorizontalDynamicContentLayoutGroup itemsContentLayout;
    public ObjectPool shopTogglesPool;
    public ToggleGroup toggleGroup;
    public Text balanceText;
    #endregion Public Members

    #region Private Members
    private bool isOpen = false;
    private Enums.StoreType selectedStoreType;
    private Dictionary<Enums.StoreType, StoreToggleItemView> storeTogglesDict = new Dictionary<Enums.StoreType, StoreToggleItemView>();
    #endregion Private Members

    #region Unity Methods
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        fadingelement.FadeOut(true);
    }

    void OnEnable()
    {
        UserController.Instance.gtUser.AddStoreListener(Enums.StoreType.Boards, Selected_OnSelectNewBoardItem);
        UserController.Instance.gtUser.AddStoreListener(Enums.StoreType.LuckyItems, Selected_OnSelectNewLuckyItem);
        UserController.Instance.gtUser.AddStoreListener(Enums.StoreType.Mascots, Selected_OnSelectNewMascot);
        UserController.Instance.gtUser.AddStoreListener(Enums.StoreType.Dices, Selected_OnSelectNewDice);
        UserController.Instance.gtUser.AddStoreListener(Enums.StoreType.Blessings, Selected_OnSelectNewBlessing);

        UserController.Instance.wallet.OnLoyaltyChanged += Wallet_OnLoyaltyChanged;

        InitBalance(UserController.Instance.wallet.LoyaltyPoints);
        InitStores(UserController.Instance.gtUser.StoresData.storesDict);
    }

    void OnDisable()
    {
        foreach (var item in storeTogglesDict)
            Destroy(item.Value.gameObject);

        storeTogglesDict.Clear();

        UserController.Instance.wallet.OnLoyaltyChanged -= Wallet_OnLoyaltyChanged;

        UserController.Instance.gtUser.RemoveStoreListener(Enums.StoreType.Boards, Selected_OnSelectNewBoardItem);
        UserController.Instance.gtUser.RemoveStoreListener(Enums.StoreType.LuckyItems, Selected_OnSelectNewLuckyItem);
        UserController.Instance.gtUser.RemoveStoreListener(Enums.StoreType.Mascots, Selected_OnSelectNewMascot);
        UserController.Instance.gtUser.RemoveStoreListener(Enums.StoreType.Dices, Selected_OnSelectNewDice);
        UserController.Instance.gtUser.RemoveStoreListener(Enums.StoreType.Blessings, Selected_OnSelectNewBlessing);
    }


    private void Selected_OnSelectNewBoardItem(List<StoreItem> items)
    {
        if (items == null || items[0] == null)
            return;
        RemoteGameController.Instance.ChangeBoard(items[0].Id);
    }

    private void Selected_OnSelectNewLuckyItem(List<StoreItem> items)
    {
        if (items == null || items[0] == null)
            return;
        RemoteGameController.Instance.SaveNewItemChanging(Enums.StoreType.LuckyItems, items[0].Id, false);
    }

    private void Selected_OnSelectNewMascot(List<StoreItem> items)
    {
        if (items == null || items[0] == null)
            return;
        RemoteGameController.Instance.SaveNewItemChanging(Enums.StoreType.Mascots, items[0].Id, false);
    }

    private void Selected_OnSelectNewDice(List<StoreItem> items)
    {
        if (items == null || items[0] == null)
            return;

        RemoteGameController.Instance.SavePendingRollingItem(Enums.StoreType.Dices, items[0].Id);
    }

    private void Selected_OnSelectNewBlessing(List<StoreItem> items)
    {
        if (items == null || items[0] == null)
            return;
        RemoteGameController.Instance.SavePendingRollingItem(Enums.StoreType.Blessings, items[0].Id);
    }

    #endregion Unity Methods

    public void SetActive(bool isOn)
    {
        isOpen = isOn;

        if (isOpen)
        {
            fadingelement.FadeIn();
            if (storeTogglesDict.ContainsKey(selectedStoreType))
            {
                storeTogglesDict[selectedStoreType].toggle.isOn = true;
                storeTogglesDict[selectedStoreType].animator.SetBool("IsOn", isOn);
            }
        }
        else
        {
            fadingelement.FadeOut();
            if(storeTogglesDict.ContainsKey(selectedStoreType))
                storeTogglesDict[selectedStoreType].toggle.isOn = false;
        }
    }

    public void ToggleActive()
    {
        SetActive(!isOpen);
    }

    #region Init Methods

    private void InitStores(Dictionary<Enums.StoreType, Store> storesDict)
    {
        foreach (var item in storesDict)
        {
            if (item.Value.inGameStore)
            {
                GameObject go = shopTogglesPool.GetObjectFromPool();
                go.InitGameObjectAfterInstantiation(shopTogglesPool.transform);
                StoreToggleItemView toggleView = go.GetComponent<StoreToggleItemView>();
                toggleView.Populate(item.Value.name, toggleGroup, b => ToggleClick(b, item.Key));
                storeTogglesDict.Add(item.Key, toggleView);
            }
        }

        selectedStoreType = storeTogglesDict.GetFirstKey();
    }

    private void InitBalance(int loyaltyPoints)
    {
        balanceText.text = Utils.LocalizeTerm("Balance") + ": <color=#D62223>" + Wallet.LoyaltyPointsPostfix + Wallet.AmountToString(loyaltyPoints) + "</color>";
    }
    #endregion Init Methods

    #region Content Methods
    private void SetStore(Enums.StoreType type)
    {
        itemsContentLayout.ClearElements();
        SetItems(UserController.Instance.gtUser.StoresData.GetStore(type).items.ToArray());
    }

    private void SetItems(IDynamicElement[] items)
    {
        List<IDynamicElement> list = new List<IDynamicElement>(items);
        itemsContentLayout.SetElements(list);
    }

    private void ToggleClick(bool isOn, Enums.StoreType type)
    {
        if (isOn)
        {
            selectedStoreType = type;
            SetStore(type);
        }
    }
    #endregion Content Methods

    private void Wallet_OnLoyaltyChanged(int newPoints)
    {
        InitBalance(newPoints);
    }
}
