using System.Collections.Generic;
using System.Net;

namespace NpuRozklad.LessonsProvider.Fetcher
{
    public class NpuServerFetcherRequestOptions
    {
        public string Code { get; set; }
        public string Faculty { get; set; }
        public KeyValuePair<string, string> Params { get; set; }
        public KeyValuePair<string, string> LoginPass { get; set; }
        
        public Dictionary<HttpRequestHeader, string> Headers { get; set; }
    }
}