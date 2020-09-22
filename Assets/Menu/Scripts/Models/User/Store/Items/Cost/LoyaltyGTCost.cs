using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace GT.Store
{
    public class LoyaltyGTCost : BaseGTCost
    {
        public int LoyaltyCost { get; private set; }
        public override string CurrencyName { get { return "Loyalty Points"; } }
        public override string PurchaseMethod { get { return "Loyalty"; } }

        public LoyaltyGTCost(int cost)
        {
            LoyaltyCost = cost;
        }

        public override bool HaveEnoughMoney()
        {
            return LoyaltyCost <= UserController.Instance.wallet.LoyaltyPoints;
        }

        public override string GetCostString()
        {
            return Wallet.LoyaltyPointsPostfix + Wallet.AmountToString(LoyaltyCost, 1);
        }

        public override float GetCostFloat()
        {
            return (float)LoyaltyCost;
        }
    }
}
