using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace GT.Store
{
    public class VirtualGTCost : BaseGTCost
    {
        public int VirtualCost { get; private set; }
        public override string CurrencyName { get { return "Coin cost"; } }
        public override string PurchaseMethod { get { return "Virtual"; } }

        public VirtualGTCost(int cost)
        {
            VirtualCost = cost;
        }

        public override bool HaveEnoughMoney()
        {
            return VirtualCost <= UserController.Instance.wallet.VirtualCoins;
        }

        public override string GetCostString()
        {
            return Wallet.AmountToString(VirtualCost, Enums.MatchKind.Virtual);
        }

        public override float GetCostFloat()
        {
            return (float)VirtualCost;
        }
    }
}
