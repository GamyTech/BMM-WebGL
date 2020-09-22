using System.Collections;
using GT.Websocket;
using System.Collections.Generic;
using UnityEngine;

namespace GT.User
{
    public class TransactionsHistory
    {
        public List<TransactionData> Transactions { get; private set; }
        public int LastPage { get; private set; }
        public bool IsComplete { get; private set; }

        private int ITEMS_PER_PAGE = 20;
        private int MAX_PAGES = 5;

        public TransactionsHistory()
        {
            LastPage = 1;
            Transactions = new List<TransactionData>();
        }

        public void GetNewTransactions(object data)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)data;

            object o;
            int page = 0;
            if (dict.TryGetValue("Page", out o))
                page = (int)o.ParseFloat();
            else
                Debug.LogError("Page missing");

            if (dict.TryGetValue("Data", out o))
            {
                List<object> list = (List<object>)o;
                List<TransactionData> newTransactions = new List<TransactionData>();
                IsComplete = list.Count != ITEMS_PER_PAGE || page == MAX_PAGES;
                bool overlapse = false;

                for (int i = 0; i < list.Count; ++i)
                {
                    TransactionData transaction = new TransactionData((Dictionary<string, object>)list[i]);
                    if(Mathf.Abs(transaction.Amount) > 0.001f)
                    {
                        newTransactions.Add(transaction);
                        if (Transactions.Find(t => t.Id == transaction.Id) == null)
                            Transactions.Add(transaction);
                        else
                            overlapse = true;
                    }
                }

                if (page == 1)
                {
                    if(Transactions.Count > list.Count && !overlapse)
                    {
                        Transactions = newTransactions;
                        LastPage = page;
                    }
                }
                else
                    LastPage = page;

                newTransactions.Sort((a, b) => { return a.Id.CompareTo(b.Id); });
            }
            else
                Debug.LogError("Data is missing");
        }
    }
}
