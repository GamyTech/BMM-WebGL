using System;

namespace GT.Assets
{
    public class WidgetCategoriesData : CategoriesData
    {
        public override string DefaultPath
        {
            get
            {
                return "Common/";
            }
        }

        protected override string[] GetKeys()
        {
            return Enum.GetNames(typeof(Enums.WidgetId));
        }
    }
}
