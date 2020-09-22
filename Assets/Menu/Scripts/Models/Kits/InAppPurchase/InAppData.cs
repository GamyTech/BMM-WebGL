using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace GT.InAppPurchase
{
    public class InAppData
    {
        public string name;
        public string itemBundleId;
        public float value;
        public float price;
        public SpriteData imageData;

        public InAppData(string itemBundleId, string name, float item, float price, SpriteData imageData)
        {
            this.itemBundleId = itemBundleId;
            this.name = name;
            this.value = item;
            this.price = price;
            this.imageData = imageData;
        }

        public InAppData(Dictionary<string,object> data)
        {
            object o;
            if(data.TryGetValue("PackageId", out o))
            {
                this.itemBundleId = o.ToString(); // TODO change to AppInformation.BUNDLE_ID.ToLower() + "."
            }
            if (data.TryGetValue("Cost", out o))
            {
                this.price = o.ParseFloat();
            }
            if (data.TryGetValue("CurrencyValue", out o))
            {
                this.value = o.ParseInt();
            }
            if (data.TryGetValue("Name", out o))
            {
                this.name = o.ToString();
            }
            if (data.TryGetValue("PictureUrl", out o))
            {
                this.imageData = new SpriteData(o.ToString());
            }
        }

        public override string ToString()
        {
            return "ID:" + itemBundleId;
        }

        #region Constants
        public const string APPLE_CONSUMABLE = "com.unity3d.test.services.purchasing.consumable";           // Apple App Store identifier for the consumable product.
        public const string APPLE_NON_CONSUMABLE = "com.unity3d.test.services.purchasing.nonconsumable";    // Apple App Store identifier for the non-consumable product.
        public const string APPLE_SUBSCRIPTION = "com.unity3d.test.services.purchasing.subscription";       // Apple App Store identifier for the subscription product.
        public const string GOOGLE_CONSUMABLE = "com.unity3d.test.services.purchasing.consumable";          // Google Play Store identifier for the consumable product.
        public const string GOOGLE_NON_CONSUMABLE = "com.unity3d.test.services.purchasing.nonconsumable";   // Google Play Store identifier for the non-consumable product.
        public const string GOOGLE_SUBSCRIPTION = "com.unity3d.test.services.purchasing.subscription";      // Google Play Store identifier for the subscription product.
        #endregion Constants
    }
}
