#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace HTTPOrg.BouncyCastle.Crypto.Tls
{
    public abstract class CertificateStatusType
    {
        /*
         *  RFC 3546 3.6
         */
        public const byte ocsp = 1;
    }
}

#endif
