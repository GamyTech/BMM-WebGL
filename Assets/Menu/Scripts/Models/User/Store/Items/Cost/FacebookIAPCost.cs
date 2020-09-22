namespace GT.Store
{
    public class FacebookIAPCost : BaseIAPCost
    {
        private string m_itemId;
        private float m_itemCost;

        public override string ItemId
        {
            get
            {
                return m_itemId;
            }
        }

        public override float Cost
        {
            get
            {
                return m_itemCost;
            }
        }

        public override string PurchaseMethod { get { return "FacebookIAP"; } }


        public FacebookIAPCost(bool consumable, string id, float cost) : base(consumable)
        {
            m_itemId = id;
            m_itemCost = cost;
        }
    }
}
