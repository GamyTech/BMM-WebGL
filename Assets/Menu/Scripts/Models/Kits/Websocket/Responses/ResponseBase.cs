using UnityEngine;
using System;
using System.Collections.Generic;
using MiniJSON;
using GT.Encryption;

namespace GT.Websocket
{
    public class ResponseBase : IServerResponse
    {
        internal enum ServerResponseKey
        {
            Response,
            ErrorCode,
            Errors,
        }

        public string rawResponse { get; private set; }
        public WSResponseCode responseCode { get; private set; }
        public Dictionary<APIResponseVariable, object> responseDict { get; private set; }

        #region Constractors

        public ResponseBase(string message)
        {
            rawResponse = null;
            responseCode = WSResponseCode.OK;
            responseDict = null;
            Init(message);
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
        public bool TryGetAPIVariable<T>(APIResponseVariable var, out T result)
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
        public bool TryGetAPIVariable(APIResponseVariable var, out object result)
        {
            result = null;
            if (responseDict.TryGetValue(var, out result) == false)
            {
                Debug.LogWarning(var + " is missing from response dictionary");
                return false;
            }
            return true;
        }
        #endregion Generic Get API Variable

        #region Aid Functions
        /// <summary>
        /// Initialize response
        /// </summary>
        /// <param name="error"></param>
        /// <param name="text"></param>
        private void Init(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            string decrypted = tryDecrypt(text);
            rawResponse = decrypted;
            Dictionary<string, object> dict = Json.Deserialize(decrypted) as Dictionary<string, object>;
            if (dict == null)
            {
                // failed to deserialize
                return;
            }
            responseDict = new Dictionary<APIResponseVariable, object>();

            object e;
            if (dict.TryGetValue(ServerResponseKey.ErrorCode.ToString(), out e))
            {
                WSResponseCode code;
                if (Utils.TryParseEnum(e.ToString(), out code))
                    responseCode = code;
                else
                    responseCode = WSResponseCode.UnrecognizedError;
                return;
            }

            foreach (ServerResponseKey item in Enum.GetValues(typeof(ServerResponseKey)))
            {
                if (dict.ContainsKey(item.ToString()))
                    dict.Remove(item.ToString());
            }

            foreach (KeyValuePair<string, object> responseItem in dict)
            {
                APIResponseVariable apiResponse;
                if (Utils.TryParseEnum(responseItem.Key, out apiResponse))
                    responseDict.Add(apiResponse, responseItem.Value);
            }
        }

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
                responseDict.Display<APIResponseVariable, object>() : ToString();
        }
        #endregion Overrides
    }
}
