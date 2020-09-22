using UnityEngine;
using System;
using System.Collections.Generic;
using MiniJSON;
using GT.Encryption;

namespace GT.Database
{
    public enum GSResponseCode
    {
        // ------------------ Success ------------------
        OK = 0,
        // ------------------ Success ------------------

        // ------------------ Unknown ------------------
        UnrecognizedErrorCode = -1,
        // ------------------ Unknown ------------------

        // --------------- Client Errors ---------------
        ConnectionError = 1000,
        // --------------- Client Errors ---------------

        // ---------------- Game Errors ----------------
        IsInMaintenance = 2000,
        MissingMatchKind = 2006,
        GameIdDoesntExist = 2015,
        MissingGameId = 2046,
        // ---------------- Game Errors ----------------

        // ------------- Server Exceptions -------------
        UnknownException = 3000,
        NullReference = 3001,
        ThreadAbort = 3002,
        IndexOutOfRange = 3003,
        ObjectReferenceNotSet = 3004,
        // ------------- Server Exceptions -------------
    }

    public class GlobalServerResponseBase
    {
        public string rawResponse { get; protected set; }
        public GSResponseCode responseCode { get; protected set; }
        public Dictionary<string, object> ResponseDict { get; protected set; }

        #region Constractors
        public GlobalServerResponseBase(WWW w)
        {
            Debug.Log("ServerResponse " + w.text);
            if (!string.IsNullOrEmpty(w.error))
            {
                rawResponse = w.error;
                responseCode = GSResponseCode.ConnectionError;
                return;
            }

            if (string.IsNullOrEmpty(w.text))
                return;

            string decrypted = tryDecrypt(w.text);
            rawResponse = decrypted;

            ResponseDict = Json.Deserialize(decrypted) as Dictionary<string, object>;
            if (ResponseDict == null)
                return;

            object e;
            if (ResponseDict.TryGetValue("ErrorCode", out e))
            {
                GSResponseCode code;
                responseCode = Utils.TryParseEnum(e, out code) ? code : GSResponseCode.UnrecognizedErrorCode;
                return;
            }

            responseCode = GSResponseCode.OK;
        }

        private void Init()
        {

        }

        #endregion Constractors

        #region Generic Get API Variable
        /// <summary>
        /// Try getting and parsing a variable from response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetAPIVariable<T>(string var, out T result)
        {
            result = default(T);
            string str;
            object o;
            if (TryGetAPIVariable(var, out o) == false)
                return false;

            str = o.ToString();

            if (result is int)
            {
                int i;
                if (int.TryParse(str, out i) == false)
                    Debug.LogWarning("falied to parse " + str + " in response dictionary");

                result = (T)Convert.ChangeType(i, typeof(T));
            }
            else if (result is float)
            {
                float f;
                if (float.TryParse(str, out f) == false)
                    Debug.LogWarning("falied to parse " + str + " in response dictionary");

                result = (T)Convert.ChangeType(f, typeof(T));
            }
            else if (result is bool)
            {
                bool b;
                if (bool.TryParse(str, out b) == false)
                    Debug.LogWarning("falied to parse " + str + " in response dictionary");
                
                result = (T)Convert.ChangeType(b, typeof(T));
            }
            else if (typeof(T).IsEnum)
            {
                if (Utils.TryParseEnum(str, out result) == false)
                    Debug.LogWarning("falied to parse " + str + " in response dictionary");
                
            }
            else if(result is string)
                result = (T)Convert.ChangeType(str, typeof(T));
            else
                result = (T)o;
            return true;
        }

        /// <summary>
        /// Try getting a variable from response.
        /// </summary>
        /// <param name="var"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetVariable(string var, out object result)
        {
            if (ResponseDict.TryGetValue(var, out result) == false)
            {
                Debug.LogWarning(var + " is missing from response dictionary");
                return false;
            }
            return true;
        }
        #endregion Generic Get API Variable

        #region Aid Functions


        /// <summary>
        /// Try decrypting text.
        /// If seccessfull will return decrypted string.
        /// If failed will return original string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string tryDecrypt(string text)
        {
            try
            {
                return AesBase64Wrapper.Decrypt(text);
            }
            catch
            {
                //Debug.LogWarning("Failed to decrypt " + text);
                return text;
            }
        }
        #endregion Aid Functions

        #region Overrides
        public override string ToString()
        {
            return "Response Code: " + responseCode + ", Raw Data:[" + rawResponse + "]";
        }

        public string ToString(bool dict)
        {
            return dict ? "Response Code: " + responseCode + ", Data Dict:" +
                ResponseDict.Display<string, object>() : ToString();
        }
        #endregion Overrides
    }
}
