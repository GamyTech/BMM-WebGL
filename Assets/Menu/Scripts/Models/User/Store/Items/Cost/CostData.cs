using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace GT.Store
{
    public class CostData
    {
        public List<BaseGTCost> GTCosts { get; private set; }
        public BaseIAPCost IAPCost { get; private set; }

        public CostData(bool consumable, Dictionary<string, object> dict)
        {
            GTCosts = new List<BaseGTCost>();

            object o;
            if (dict.TryGetValue("LoyaltyCost", out o))
                GTCosts.Add(new LoyaltyGTCost(o.ParseInt()));

            if (dict.TryGetValue("CashCost", out o))
                GTCosts.Add(new CashGTCost(o.ParseFloat()));

            if (dict.TryGetValue("VirtualCost", out o))
                GTCosts.Add(new VirtualGTCost(o.ParseInt()));

            IAPCost = BaseIAPCost.CreateIAPCost(consumable, dict);
        }

        public bool IsFree()
        {
            if(IAPCost == null && GTCosts.Count == 0)
                return true;

            for (int x = 0; x < GTCosts.Count; ++x)
                if (GTCosts[x].GetCostFloat() != 0.0f)
                    return false;

            return true;
        }

        public string GetCostsString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < GTCosts.Count; i++)
            {
                if(i!= 0)
                {
                    sb.Append(" or ");
                }
                sb.Append(GTCosts[i].GetCostString());
            }

            if (IAPCost != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" or ");
                }
                sb.Append(IAPCost.GetIAPCostString());
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return GetCostsString();
        }
    }
}
