#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace HTTPOrg.BouncyCastle.Utilities
{
    /**
     * Exception to be thrown on a failure to reset an object implementing Memoable.
     * <p>
     * The exception extends InvalidCastException to enable users to have a single handling case,
     * only introducing specific handling of this one if required.
     * </p>
     */
    public class MemoableResetException
        : InvalidCastException
    {
        /**
         * Basic Constructor.
         *
         * @param msg message to be associated with this exception.
         */
        public MemoableResetException(string msg)
            : base(msg)
        {
        }
    }

}


#endif
