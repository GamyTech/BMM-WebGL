#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using HTTPOrg.BouncyCastle.Security;

namespace HTTPOrg.BouncyCastle.Crypto.Tls
{
    internal class TlsClientContextImpl
        :   AbstractTlsContext, TlsClientContext
    {
        internal TlsClientContextImpl(SecureRandom secureRandom, SecurityParameters securityParameters)
            :   base(secureRandom, securityParameters)
        {
        }

        public override bool IsServer
        {
            get { return false; }
        }
    }
}

#endif
