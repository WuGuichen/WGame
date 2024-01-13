using System;
using System.Net;

namespace UosCdn
{ 
    [Serializable]
    public class HttpResponse
    {
        public WebHeaderCollection headers;
        public string responseBody;
    }
}