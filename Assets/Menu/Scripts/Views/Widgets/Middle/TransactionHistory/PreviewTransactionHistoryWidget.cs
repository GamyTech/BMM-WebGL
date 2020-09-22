using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GT.User;
using GT.Websocket;
using System;

public class PreviewTransactionHistoryWidget : Widget
{
    private const int PREVIEWED_ITEMS = 3;

    public VerticalLayoutGroup contentLayout;
    public Text HeadlineText;
    public Button MoreHistoryButton;
    public ObjectPool Pool;
    public SmoothLayoutElement element;

    private List<GameObject> activeObjectsList = new List<GameObject>();

    public override void EnableWidget()
    {
        base.EnableWidget();
        UserController.Instance.gtUser.OnTransactionHistoryChanged += GtUser_OnTransactionHistoryChanged;
        LoadingController.Instance.ShowPageLoading(transform as RectTransform);

        UserController.Instance.GetLastTransactionHistory();
    }

    public override void DisableWidget()
    {
        UserController.Instance.gtUser.OnTransactionHistoryChanged -= GtUser_OnTransactionHistoryChanged;

        DestroyActiveObjects();
        base.DisableWidget();
    }

    private void DestroyActiveObjects()
    {
        Pool.PoolObjects(activeObjectsList);
        activeObjectsList.Clear();
    }

    #region Events
    private void GtUser_OnTransactionHistoryChanged(TransactionsHistory newValue)
    {
        LoadingController.Instance.HidePageLoading();

        DestroyActiveObjects();
        MoreHistoryButton.gameObject.SetActive(newValue.Transactions.Count > PREVIEWED_ITEMS);

        if (newValue.Transactions.Count <= 0)
            HeadlineText.text = Utils.LocalizeTerm("Transaction History Is Empty").ToUpper();
        else
        {
            for (int x = 0; x < Mathf.Min(PREVIEWED_ITEMS, newValue.Transactions.Count); x++)
            {
                GameObject go = Pool.GetObjectFromPool();
                go.InitGameObjectAfterInstantiation(Pool.transform);
                go.GetComponent<TransactionView>().Populate(newValue.Transactions[x]);
                activeObjectsList.Add(go);
            }

            Canvas.ForceUpdateCanvases();
            element.SetVerticalStateUsingLayoutSize(SmoothLayoutElement.State.Max);
        }
    }
    #endregion Events

    #region Inputs
    public void ViewMoreHistory()
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.FullTransactionHistory);
    }
    #endregion Inputs
}
