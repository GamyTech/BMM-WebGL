using UnityEngine;
using System.Collections;

namespace GT.Store
{
    public interface ILockable 
    {
        int UnlockRank { get; }

        // maybe need to add conditions for unlocking?
    }
}
