using System.Collections.Generic;
using GT.User;
using UnityEngine;
using UnityEngine.UI;

public class SelectSavedCardView : MonoBehaviour
{
    public Text AmountText;
    public ObjectPool ItemPool;
    public ToggleGroup SelectGroup;

    private List<SavedCardItemView> cardList;
    private GTUser.SavedCreditCard selected;

    private void Start()
    {
        UserController.Instance.gtUser.OnCreditCardListChanged += OnCardListLoaded;

        cardList = new List<SavedCardItemView>();
        
        AmountText.text = Wallet.CashPostfix + UserController.Instance.gtUser.cashInData.depositAmount.amount.ToString();
        UpdateList(UserController.Instance.gtUser.savedCreditCards);
    }

    private void OnDestroy()
    {
        UserController.Instance.gtUser.OnCreditCardListChanged -= OnCardListLoaded;
    }

    public GTUser.SavedCreditCard GetSelected()
    {
        return selected;
    }

    private void UpdateList(List<GTUser.SavedCreditCard> savedCreditCards)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].OnSelected -= OnCardSelected;
            cardList[i].OnDelete -= OnCardDelete;
            ItemPool.PoolObject(cardList[i].gameObject);
        }
        cardList.Clear();

        if (savedCreditCards == null)
            return;

        for (int i = 0; i < savedCreditCards.Count; i++)
        {
            GameObject lastGo = ItemPool.GetObjectFromPool();
            lastGo.InitGameObjectAfterInstantiation(ItemPool.transform);
            SavedCardItemView cardView = lastGo.GetComponent<SavedCardItemView>();
            cardView.Populate(SelectGroup, savedCreditCards[i]);
            cardView.OnSelected += OnCardSelected;
            cardView.OnDelete += OnCardDelete;
            cardList.Add(cardView);
        }
        if (cardList.Count > 0)
            cardList[cardList.Count - 1].SelectionToggle.isOn = true;
    }

    public void ToggleListEdit(bool isEditMode)
    {
        for (int i = 0; i < cardList.Count; i++)
            cardList[i].ToggleEdit(isEditMode);
    }

    #region Input
    public void Edit(bool isOn)
    {
        ToggleListEdit(isOn);
    }
    #endregion Input

    #region Events
    private void OnCardListLoaded(List<GTUser.SavedCreditCard> cards)
    {
        UpdateList(cards);
    }

    private void OnCardSelected(GTUser.SavedCreditCard card)
    {
        selected = card;
    }

    private void OnCardDelete(GTUser.SavedCreditCard card)
    {
        UserController.Instance.DeleteCreditCard(card);
        Edit(false);
    }
    #endregion Events
}
