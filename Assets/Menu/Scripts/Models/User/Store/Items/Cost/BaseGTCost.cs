using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GT.Store
{
    public abstract class BaseGTCost 
    {
        public abstract string CurrencyName { get; }
        public abstract string PurchaseMethod { get; }

        public abstract string GetCostString();
        public abstract float GetCostFloat();

        public abstract bool HaveEnoughMoney();
    }
}
