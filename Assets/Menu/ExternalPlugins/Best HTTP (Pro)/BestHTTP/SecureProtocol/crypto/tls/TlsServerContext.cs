#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using HTTPOrg.BouncyCastle.Security;

namespace HTTPOrg.BouncyCastle.Crypto.Tls
{
    public interface TlsServerContext
        : TlsContext
    {
    }
}

#endif
