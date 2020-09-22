using UnityEngine;
using System.Collections;

namespace GT.Store
{
    public interface IConsumable 
    {
        int OwnedCount { get; }
        int ConsumableAmount { get; }
        void ConsumeItem();
    }
}
