using UnityEngine;
using System.Collections.Generic;
using System;

namespace GT.Store
{
    public class CashGTCost : BaseGTCost
    {
        public float CashCost { get; private set; }
        public override string CurrencyName { get { return "Cash"; } }
        public override string PurchaseMethod { get { return "DefaultCash"; } }

        public CashGTCost(float cost)
        {
            CashCost = cost;
        }

        public override bool HaveEnoughMoney()
        {
            return CashCost <= UserController.Instance.wallet.TotalCash;
        }

        public override string GetCostString()
        {
            return Wallet.CashPostfix + Wallet.AmountToString(CashCost, 2);
        }

        public override float GetCostFloat()
        {
            return CashCost;
        }
    }
}
