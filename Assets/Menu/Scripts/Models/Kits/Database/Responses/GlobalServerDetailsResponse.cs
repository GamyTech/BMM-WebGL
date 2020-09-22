using UnityEngine;
using System.Collections.Generic;

namespace GT.Database
{
    public class GlobalServerDetailsResponse : GlobalServerResponseBase
    {
        public string ServerIp { get; protected set; }
        public string HostIp { get; protected set; }
        public Dictionary<VersioningKeys, VersionData> NewData { get; protected set; }

        public GlobalServerDetailsResponse(WWW w) : base(w)
        {
            if (responseCode == GSResponseCode.OK)
                Init();
        }

        protected void Init()
        {
            if(ResponseDict == null)
            {
                responseCode = GSResponseCode.ConnectionError;
                return;
            }

            InitIps();
            InitVersionsData();
        }

        void InitIps()
        {
            object o;

            if (ResponseDict.TryGetValue("Globals", out o))
            {
                Dictionary<string, object> Ips = o as Dictionary<string, object>;
                if (Ips.TryGetValue("HostIp", out o))
                    HostIp = o.ToString();
                else
                {
                    responseCode = GSResponseCode.ConnectionError;
                    Debug.LogError("HostIp is missing in the dictionnary");
                }

                if (Ips.TryGetValue("ServerIp", out o))
                {
                    Dictionary<string, object> ServerIps = o as Dictionary<string, object>;
                    if (ServerIps.TryGetValue("ServerIp", out o))
                        ServerIp = o.ToString();
                    else
                    {
                        Debug.LogError("ServerIp is missing in the dictionnary");
                        responseCode = GSResponseCode.ConnectionError;
                    }
                }
                else
                {
                    Debug.LogError("ServerIp is missing in the dictionnary");
                    responseCode = GSResponseCode.ConnectionError;
                }
            }
            else
            {
                Debug.LogError("Globals is missing in the dictionnary");
                responseCode = GSResponseCode.ConnectionError;
            }
        }

        void InitVersionsData()
        {
            NewData = new Dictionary<VersioningKeys, VersionData>();
            object o;

            if (ResponseDict.TryGetValue("Versions", out o))
            {
                Dictionary<string, object> versionsData = o as Dictionary<string, object>;

                VersioningKeys key;
                foreach(var item in versionsData)
                    if (Utils.TryParseEnum(item.Key, out key, true))
                        NewData.Add(key, new VersionData(item.Value as Dictionary<string, object>));
            }
            else
            {
                Debug.LogError("Versions is missing in the dictionnary");
                responseCode = GSResponseCode.ConnectionError;
            }
        }

        public class VersionData
        {
            public object Data { get; private set; }
            public int Version { get; private set; }

            public VersionData(object dictRaw)
            {
                Dictionary<string, object> dict = dictRaw as Dictionary<string, object>;
                object o;
                if (dict.TryGetValue("Version", out o))
                    Version = o.ParseInt();
                else
                    Debug.LogError("Version is missing in the dictionnary");

                if (dict.TryGetValue("Data", out o))
                    Data = o;
            }
        }
    }
}
