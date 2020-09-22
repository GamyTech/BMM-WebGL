using System;

namespace GT.Assets
{
    public class PageCategoriesData : CategoriesData
    {
        public override string DefaultPath
        {
            get
            {
                return "Single Page/Common/";
            }
        }

        protected override string[] GetKeys()
        {
            return Enum.GetNames(typeof(Enums.PageId));
        }
    }
}
