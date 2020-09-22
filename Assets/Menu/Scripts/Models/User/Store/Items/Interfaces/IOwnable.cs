using UnityEngine;
using System.Collections;

namespace GT.Store
{
    public interface IOwnable
    {
        bool Owned { get; }
    }
}
