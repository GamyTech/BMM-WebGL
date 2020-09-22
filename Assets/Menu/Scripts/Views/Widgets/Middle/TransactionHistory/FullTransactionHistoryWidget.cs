using GT.User;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FullTransactionHistoryWidget : PopupWidget
{
    public HorizontalOrVertcalDynamicContentLayoutGroup content;

    public GameObject PageLoadingPrefab;
    private IDynamicElement pageLoaderElement;

    public override void EnableWidget()
    {
        base.EnableWidget();

        UserController.Instance.gtUser.OnTransactionHistoryChanged += GtUser_OnTransactionHistoryChanged;
        InitList(UserController.Instance.gtUser.TransactionHistoryData);
    }

    public override void DisableWidget()
    {
        UserController.Instance.gtUser.OnTransactionHistoryChanged -= GtUser_OnTransactionHistoryChanged;
        base.DisableWidget();
    }

    protected void InitList(TransactionsHistory history)
    {
        content.SetElements(new List<IDynamicElement>(history.Transactions.ToArray()));

        if (!history.IsComplete)
        {
            pageLoaderElement = new PageLoaderData(PageLoadingPrefab, UserController.Instance.GetNextTransactionHistoryPage);
            content.AddElement(pageLoaderElement);
        }
    }

    #region Events
    private void GtUser_OnTransactionHistoryChanged(TransactionsHistory newValue)
    {
        InitList(newValue);
    }
    #endregion Events
}
