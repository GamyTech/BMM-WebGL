namespace GT.Store
{
    public class ItunesIAPCost : BaseIAPCost
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

        public override string PurchaseMethod { get { return "IosCash"; } }


        public ItunesIAPCost(bool consumable, string id, float cost) : base(consumable)
        {
            m_itemId = id;
            m_itemCost = cost;
        }
    }
}
