#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace HTTPOrg.BouncyCastle.Math.EC
{
    public interface ECPointMap
    {
        ECPoint Map(ECPoint p);
    }
}

#endif
