using System.Collections.Generic;

namespace GT.Websocket
{
    public interface IServerResponse
    {
        WSResponseCode responseCode { get; }

        string rawResponse { get; }

        Dictionary<APIResponseVariable, object> responseDict { get; }


        bool TryGetAPIVariable<T>(APIResponseVariable var, out T result);

        string ToString(bool dict);
    }
}
